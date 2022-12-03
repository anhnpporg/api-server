using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.InvoiceModel;
using UtNhanDrug_BE.Models.ProductUnitModel;
using UtNhanDrug_BE.Models.ResponseModel;
using UtNhanDrug_BE.Services.ManagerService;
using UtNhanDrug_BE.Services.ProductUnitService;
using System.Linq;
using UtNhanDrug_BE.Models.ModelHelper;
using Microsoft.EntityFrameworkCore.Storage;
using UtNhanDrug_BE.Hepper.GenaralBarcode;
using UtNhanDrug_BE.Hepper;
using UtNhanDrug_BE.Models.BatchModel;
using UtNhanDrug_BE.Services.ProductService;
using UtNhanDrug_BE.Models.ProductModel;

namespace UtNhanDrug_BE.Services.InvoiceService
{
    public class InvoiceSvc : IInvoiceSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly IUserSvc _userSvc;
        private readonly IProductUnitPriceSvc _unitSvc;
        private readonly IProductSvc _pSvc;
        private readonly DateTime today = LocalDateTime.DateTimeNow();
        public InvoiceSvc(ut_nhan_drug_store_databaseContext context, IUserSvc userSvc, IProductUnitPriceSvc unitSvc, IProductSvc pSvc)
        {
            _context = context;
            _userSvc = userSvc;
            _unitSvc = unitSvc;
            _pSvc = pSvc;
        }
        public async Task<Response<InvoiceResponse>> CreateInvoice(int UserId, CreateInvoiceModel model)
        {
            if (model == null) return new Response<InvoiceResponse>(null)
            {
                StatusCode = 400,
                Message = "Vui lòng chọn sản phẩm để bán"
            };
            if (model.CustomerId == null & model.Customer != null)
            {
                var customer = await _userSvc.CreateCustomer(UserId, model.Customer);
                if (customer != null)
                {
                    model.CustomerId = customer.Data.Id;
                }
                else
                {
                    return new Response<InvoiceResponse>(new InvoiceResponse() { InvoiceId = -1 })
                    {
                        StatusCode = 400,
                        Message = "Khách hàng này đã tồn tại"
                    };
                }
            }
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                if(model.GoodsIssueNoteTypeId == 1)
                {
                    //create invoice
                    Invoice i = new Invoice()
                    {
                        CustomerId = model.CustomerId,
                        BodyWeight = model.BodyWeight,
                        Barcode = "####",
                        DayUse = model.DayUse,
                        Discount = 0,
                        TotalPrice = 0,
                        CreatedBy = UserId,
                        CreatedAt = today
                    };
                    _context.Invoices.Add(i);
                    await _context.SaveChangesAsync();

                    if (i != null)
                    {
                        var barcode = GenaralBarcode.CreateEan13Invoice(i.Id + "");
                        i.Barcode = barcode;
                        await _context.SaveChangesAsync();
                    }

                    //add product ro order detail
                    foreach (OrderDetailModel x in model.Product)
                    {

                        var products = await _pSvc.GetAllProduct(new FilterProduct{ IsProductActive = true });
                        if(products.Data != null)
                        {
                            foreach (var product in products.Data)
                            {
                                if (x.ProductId != product.Id)
                                {
                                    await transaction.RollbackAsync();
                                    return new Response<InvoiceResponse>(null)
                                    {
                                        StatusCode = 400,
                                        Message = "Sản phẩm " + product.Name + " đang bị lỗi, không thể bán"
                                    };
                                }

                            }
                        }
                        OrderDetail o = new OrderDetail()
                        {
                            InvoiceId = i.Id,
                            ProductId = x.ProductId,
                            Dose = x.Dose,
                            UnitDose = x.UnitDose,
                            Frequency = x.Frequency,
                            DayUse = x.DayUse,
                            Use = x.Use,
                            TotalPrice = 0,
                        };
                        _context.OrderDetails.Add(o);
                        await _context.SaveChangesAsync();
                        if (x.GoodsIssueNote.Count() == 0)
                        {
                            await transaction.RollbackAsync();
                            return new Response<InvoiceResponse>(null)
                            {
                                StatusCode = 400,
                                Message = "Vui lòng thêm lô sản phẩm"
                            };
                        }
                        foreach (var goods in x.GoodsIssueNote)
                        {
                            decimal price = 0;
                            var currentQuantity = await GetCurrentQuantity(goods.BatchId);
                            if (currentQuantity.Where(x => x.Id == goods.Unit).Select(x => x.CurrentQuantity).FirstOrDefault() < goods.Quantity)
                            {
                                await transaction.RollbackAsync();
                                return new Response<InvoiceResponse>(null)
                                {
                                    StatusCode = 400,
                                    Message = "Số lượng thuốc trong lô không đủ"
                                };
                            }
                            var unit = await _context.ProductUnitPrices.FirstOrDefaultAsync(p => p.Id == goods.Unit);
                            int ConvertedQuantity = (int)(goods.Quantity * unit.ConversionValue);

                            if (unit.Price == null)
                            {
                                var units = await _unitSvc.GetProductUnitByProductId(x.ProductId);
                                foreach (var u in units.Data)
                                {
                                    if (u.IsBaseUnit)
                                    {
                                        price = (decimal)(u.Price * ConvertedQuantity);
                                    }
                                }
                            }
                            else
                            {
                                price = (decimal)unit.Price;
                            }
                            //if (model.GoodsIssueNoteTypeId != 1) model.GoodsIssueNoteTypeId = 1;
                            GoodsIssueNote g = new GoodsIssueNote()
                            {
                                GoodsIssueNoteTypeId = model.GoodsIssueNoteTypeId,
                                OrderDetailId = o.Id,
                                BatchId = goods.BatchId,
                                Quantity = goods.Quantity,
                                Unit = unit.Unit,
                                UnitPrice = price,
                                ConvertedQuantity = ConvertedQuantity,
                            };
                            _context.GoodsIssueNotes.Add(g);
                            await _context.SaveChangesAsync();
                            o.TotalPrice += g.UnitPrice * g.Quantity;
                            //i.TotalPrice += o.TotalPrice;
                            await _context.SaveChangesAsync();
                        }
                        i.TotalPrice += o.TotalPrice;
                        await _context.SaveChangesAsync();
                    }
                    //Convert point
                    decimal toMoney = 1000;
                    decimal toPoint = 10000;

                    //get customer total point
                    var query = from c in _context.Customers
                                where c.Id == model.CustomerId
                                select c;
                    float totalPoint = (float)await query.Select(x => x.TotalPoint).FirstOrDefaultAsync();
                    //add point to create invoice 
                    if (model.GoodsIssueNoteTypeId == 1)
                    {

                        if (model.UsePoint > 0)
                        {
                            if (model.CustomerId == null)
                            {
                                await transaction.RollbackAsync();
                                return new Response<InvoiceResponse>()
                                {
                                    StatusCode = 400,
                                    Message = "Không có thông tin khách hàng để sử dụng điểm"
                                };
                            }

                            if (totalPoint - model.UsePoint < 0)
                            {
                                await transaction.RollbackAsync();
                                return new Response<InvoiceResponse>(null)
                                {
                                    StatusCode = 400,
                                    Message = "Điểm tích luỹ không đủ"
                                };
                            }

                            if (model.UsePoint * toMoney > i.TotalPrice)
                            {
                                await transaction.RollbackAsync();
                                return new Response<InvoiceResponse>(null)
                                {
                                    StatusCode = 400,
                                    Message = "Điểm tích luỹ quá giá trị đơn hàng"
                                };
                            }
                            // save transaction use point
                            CustomerPointTransaction cpt = new CustomerPointTransaction()
                            {
                                CustomerId = (int)model.CustomerId,
                                InvoiceId = i.Id,
                                Point = (double)model.UsePoint,
                                IsReciept = false
                            };
                            await _context.CustomerPointTransactions.AddAsync(cpt);
                            await _context.SaveChangesAsync();
                            //var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == model.CustomerId);
                            i.Discount = (int)cpt.Point * toMoney;
                            totalPoint = (float)(totalPoint - model.UsePoint);
                            await _context.SaveChangesAsync();
                        }
                    }
                    //add point
                    if (model.CustomerId != null)
                    {
                        int point = (int)(i.TotalPrice / toPoint);
                        // save transaction use point
                        CustomerPointTransaction cpt1 = new CustomerPointTransaction()
                        {
                            CustomerId = (int)model.CustomerId,
                            InvoiceId = i.Id,
                            Point = point,
                            IsReciept = true,
                        };
                        await _context.CustomerPointTransactions.AddAsync(cpt1);
                        await _context.SaveChangesAsync();

                        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == model.CustomerId);

                        totalPoint = (float)(totalPoint + cpt1.Point);
                        customer.TotalPoint = totalPoint;
                        await _context.SaveChangesAsync();
                    }
                    i.TotalPrice -= i.Discount;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return new Response<InvoiceResponse>(new InvoiceResponse() { InvoiceId = i.Id })
                    {
                        Message = "Tạo hoá đơn thành công"
                    };
                }else if (model.GoodsIssueNoteTypeId == 2)
                {
                    //add product ro order detail
                    foreach (OrderDetailModel x in model.Product)
                    {
                        if (x.GoodsIssueNote.Count() == 0)
                        {
                            await transaction.RollbackAsync();
                            return new Response<InvoiceResponse>(null)
                            {
                                StatusCode = 400,
                                Message = "Vui lòng thêm lô sản phẩm"
                            };
                        }
                        decimal price = 0;
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == x.ProductId);
                        foreach (var goods in x.GoodsIssueNote)
                        {
                            var currentQuantity = await GetCurrentQuantity(goods.BatchId);
                            if (currentQuantity.Where(x => x.Id == goods.Unit).Select(x => x.CurrentQuantity).FirstOrDefault() < goods.Quantity)
                            {
                                await transaction.RollbackAsync();
                                return new Response<InvoiceResponse>(null)
                                {
                                    StatusCode = 400,
                                    Message = "Số lượng thuốc trong lô không đủ"
                                };
                            }
                            var unit = await _context.ProductUnitPrices.FirstOrDefaultAsync(p => p.Id == goods.Unit);
                            int ConvertedQuantity = (int)(goods.Quantity * unit.ConversionValue);

                            if (unit.Price == null)
                            {
                                var units = await _unitSvc.GetProductUnitByProductId(x.ProductId);
                                foreach (var u in units.Data)
                                {
                                    if (u.IsBaseUnit)
                                    {
                                        price = (decimal)(u.Price * ConvertedQuantity);
                                    }
                                }
                            }
                            else
                            {
                                price = (decimal)unit.Price;
                            }
                            //if (model.GoodsIssueNoteTypeId != 1) model.GoodsIssueNoteTypeId = 1;
                            GoodsIssueNote g = new GoodsIssueNote()
                            {
                                GoodsIssueNoteTypeId = model.GoodsIssueNoteTypeId,
                                BatchId = goods.BatchId,
                                Quantity = goods.Quantity,
                                Unit = unit.Unit,
                                UnitPrice = price,
                                ConvertedQuantity = ConvertedQuantity,
                            };
                            _context.GoodsIssueNotes.Add(g);
                            await _context.SaveChangesAsync();
                        }
                    }
                    await transaction.CommitAsync();
                    return new Response<InvoiceResponse>(null)
                    {
                        Message = "Tạo hoá đơn thành công"
                    };
                }
                await transaction.RollbackAsync();
                return new Response<InvoiceResponse>(null)
                {
                    StatusCode = 400,
                    Message = "Thất bại, không có gì để thanh toán"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<InvoiceResponse>(new InvoiceResponse() { InvoiceId = -1 })
                {
                    StatusCode = 500,
                    Message = "Tạo hoá đơn thất bại",
                };
            }

        }

        private async Task<List<ViewQuantityModel>> GetCurrentQuantity(int batchId)
        {
            var query = from g in _context.GoodsReceiptNotes
                        where g.BatchId == batchId
                        select g;
            var totalQuantity = await query.Select(x => x.ConvertedQuantity).SumAsync();

            var productId = await query.Select(x => x.Batch.ProductId).FirstOrDefaultAsync();
            var query2 = from g in _context.GoodsIssueNotes
                         where g.BatchId == batchId
                         select g;
            var saledQuantity = await query2.Select(x => x.ConvertedQuantity).SumAsync();
            var currentQuantity = totalQuantity - saledQuantity;

            var query3 = from u in _context.ProductUnitPrices
                         where u.ProductId == productId
                         select u;
            var data = await query3.Where(x => x.IsDoseBasedOnBodyWeightUnit == false).Select(x => new ViewQuantityModel()
            {
                Id = x.Id,
                Unit = x.Unit,
                CurrentQuantity = (int)(currentQuantity / x.ConversionValue)
            }).ToListAsync();
            return data;
        }

        public async Task<Response<List<ViewInvoiceModel>>> GetAllInvoice()
        {
            var query = from i in _context.Invoices
                        select i;
            var data = await query.OrderByDescending(x => x.CreatedAt).Select(x => new ViewInvoiceModel()
            {
                Id = x.Id,
                Barcode = x.Barcode,
                CreatedBy = new ViewModel()
                {
                    Id = x.CreatedByNavigation.Id,
                    Name = x.CreatedByNavigation.FullName
                },
                BodyWeight = x.BodyWeight,
                CreatedAt = x.CreatedAt,
                Customer = new ViewCustomer()
                {
                    Id = x.Customer.Id,
                    PhoneNumber = x.Customer.PhoneNumber,
                    FullName = x.Customer.FullName
                },
                DayUse = x.DayUse,
                Discount = x.Discount,
                TotalPrice = x.TotalPrice
            }).ToListAsync();
            return new Response<List<ViewInvoiceModel>>(data);
        }

        public async Task<Response<List<ViewInvoiceModel>>> GetInvoiceByUserId(int userId)
        {
            var query = from i in _context.Invoices
                        where i.CreatedBy == userId
                        select i;
            var data = await query.Select(x => new ViewInvoiceModel()
            {
                Id = x.Id,
                Barcode = x.Barcode,
                CreatedBy = new ViewModel()
                {
                    Id = x.CreatedByNavigation.Id,
                    Name = x.CreatedByNavigation.FullName
                },
                BodyWeight = x.BodyWeight,
                CreatedAt = x.CreatedAt,
                Customer = new ViewCustomer()
                {
                    Id = x.Customer.Id,
                    PhoneNumber = x.Customer.PhoneNumber,
                    FullName = x.Customer.FullName
                },
                DayUse = x.DayUse,
                Discount = x.Discount,
                TotalPrice = x.TotalPrice
            }).ToListAsync();
            return new Response<List<ViewInvoiceModel>>(data);
        }

        public async Task<Response<ViewInvoiceModel>> GetInvoiceByInvoiceBarcode(string invoiceBarcode)
        {
            var query = from i in _context.Invoices
                        where i.Barcode == invoiceBarcode
                        select i;
            var data = await query.Select(x => new ViewInvoiceModel()
            {
                Id = x.Id,
                Barcode = x.Barcode,
                CreatedBy = new ViewModel()
                {
                    Id = x.CreatedByNavigation.Id,
                    Name = x.CreatedByNavigation.FullName
                },
                BodyWeight = x.BodyWeight,
                CreatedAt = x.CreatedAt,
                Customer = new ViewCustomer()
                {
                    Id = x.Customer.Id,
                    PhoneNumber = x.Customer.PhoneNumber,
                    FullName = x.Customer.FullName
                },
                DayUse = x.DayUse,
                Discount = x.Discount,
                TotalPrice = x.TotalPrice
            }).FirstOrDefaultAsync();
            if(data != null)
            {
                return new Response<ViewInvoiceModel>(data)
                {
                    Message = "Thông tin hoá đơn"
                };
            }
            else
            {
                return new Response<ViewInvoiceModel>(null)
                {
                    StatusCode = 400,
                    Message = "Mã vạch hoá đơn không tồn tại"
                };
            }
        }

        public async Task<Response<List<ViewInvoiceModel>>> GetInvoiceCustomerId(int customerId)
        {
            var query = from i in _context.Invoices
                        where i.CustomerId == customerId
                        select i;
            var data = await query.OrderByDescending(x => x.CreatedAt).Select(x => new ViewInvoiceModel()
            {
                Id = x.Id,
                Barcode = x.Barcode,
                CreatedBy = new ViewModel()
                {
                    Id = x.CreatedByNavigation.Id,
                    Name = x.CreatedByNavigation.FullName
                },
                BodyWeight = x.BodyWeight,
                CreatedAt = x.CreatedAt,
                Customer = new ViewCustomer()
                {
                    Id = x.Customer.Id,
                    PhoneNumber = x.Customer.PhoneNumber,
                    FullName = x.Customer.FullName
                },
                DayUse = x.DayUse,
                Discount = x.Discount,
                TotalPrice = x.TotalPrice
            }).ToListAsync();
            return new Response<List<ViewInvoiceModel>>(data);
        }

        public async Task<Response<ViewInvoiceModel>> ViewInvoiceById(int Id)
        {
            var query = from i in _context.Invoices
                        where i.Id == Id
                        select i;
            var data = await query.Select(x => new ViewInvoiceModel()
            {
                Id = x.Id,
                Barcode = x.Barcode,
                CreatedBy = new ViewModel()
                {
                    Id = x.CreatedByNavigation.Id,
                    Name = x.CreatedByNavigation.FullName
                },
                BodyWeight = x.BodyWeight,
                CreatedAt = x.CreatedAt,

                Customer = new ViewCustomer()
                {
                    Id = x.Customer.Id,
                    PhoneNumber = x.Customer.PhoneNumber,
                    FullName = x.Customer.FullName
                },
                DayUse = x.DayUse,
                Discount = x.Discount,
                TotalPrice = x.TotalPrice
            }).FirstOrDefaultAsync();
            if (data != null)
            {
                return new Response<ViewInvoiceModel>(data);
            }
            return new Response<ViewInvoiceModel>(null)
            {
                StatusCode = 400,
                Message = "Không tìm thấy hoá đơn này"
            };
        }

        public async Task<Response<List<ViewOrderDetailModel>>> ViewOrderDetailByInvoiceId(int id)
        {
            var query = from o in _context.OrderDetails
                        join g in _context.GoodsIssueNotes on o.Id equals g.OrderDetailId
                        where o.InvoiceId == id
                        select new { o, g };
            var data = await query.Select(x => new ViewOrderDetailModel()
            {
                Id = x.o.Id,
                Batch = new ViewModel()
                {
                    Id = x.g.Batch.Id,
                    Name = x.g.Batch.BatchBarcode
                },
                Quantity = x.g.Quantity,
                Unit = x.g.Unit,
                UnitPrice = x.g.UnitPrice,
                ConvertedQuantity = x.g.ConvertedQuantity,
                TotalPrice = x.o.TotalPrice,
                DayUse = x.o.DayUse,
                Dose = x.o.Dose,
                Frequency = x.o.Frequency,
                GoodsIssueNoteType = new ViewModel()
                {
                    Id = x.g.GoodsIssueNoteType.Id,
                    Name = x.g.GoodsIssueNoteType.Name
                },
                Product = new ViewModel()
                {
                    Id = x.o.Product.Id,
                    Name = x.o.Product.Name
                },
                UnitDose = x.o.UnitDose,
                Use = x.o.Use

            }).ToListAsync();
            foreach (var d in data)
            {
                d.ReturnedQuantity = await GetReturnedQuantityOfInvoice(d.Batch.Id, id);
                d.ViewBaseProductUnit = await GetBaseUnit(d.Product.Id);
            }
            return new Response<List<ViewOrderDetailModel>>(data);
        }

        public async Task<Response<List<ViewOrderDetailModel>>> ViewOrderDetailByBarcode(string barcode)
        {
            var query = from o in _context.OrderDetails
                        join g in _context.GoodsIssueNotes on o.Id equals g.OrderDetailId
                        where o.Invoice.Barcode == barcode
                        select new { o, g };
            var invoiceQuery = from i in _context.Invoices
                               where i.Barcode == barcode
                               select i;
            int invoiceId = await invoiceQuery.Select(x => x.Id).FirstOrDefaultAsync();
            var data = await query.Select(x => new ViewOrderDetailModel()
            {
                Id = x.o.Id,
                OrderDetailId = x.g.OrderDetailId,
                Batch = new ViewModel()
                {
                    Id = x.g.Batch.Id,
                    Name = x.g.Batch.BatchBarcode
                },
                Quantity = x.g.Quantity,
                Unit = x.g.Unit,
                UnitPrice = x.g.UnitPrice,
                ConvertedQuantity = x.g.ConvertedQuantity,
                TotalPrice = x.o.TotalPrice,
                DayUse = x.o.DayUse,
                Dose = x.o.Dose,
                Frequency = x.o.Frequency,
                GoodsIssueNoteType = new ViewModel()
                {
                    Id = x.g.GoodsIssueNoteType.Id,
                    Name = x.g.GoodsIssueNoteType.Name
                },
                Product = new ViewModel()
                {
                    Id = x.o.Product.Id,
                    Name = x.o.Product.Name
                },
                UnitDose = x.o.UnitDose,
                Use = x.o.Use

            }).ToListAsync();
            foreach(var d in data)
            {
                d.ReturnedQuantity = await GetReturnedQuantityOfInvoice(d.Batch.Id, invoiceId);
                d.ViewBaseProductUnit = await GetBaseUnit(d.Product.Id);
            }
            return new Response<List<ViewOrderDetailModel>>(data);
        }

        private async Task<int> GetReturnedQuantityOfInvoice(int? batchId, int? invoiceId)
        {
            var query = from grn in _context.GoodsReceiptNotes
                        where grn.InvoiceId == invoiceId
                        select grn;
            var returnedQuantity = await query.Where(x => x.BatchId == batchId & x.GoodsReceiptNoteTypeId == 2).Select(x => x.ConvertedQuantity).SumAsync();
            return returnedQuantity;
        }

        private async Task<ViewBaseProductUnit> GetBaseUnit(int? productId)
        {
            var query = from pu in _context.ProductUnitPrices
                        where pu.ProductId == productId
                        select pu;
            var data = await query.Where(x => x.IsBaseUnit == true).Select(x => new ViewBaseProductUnit()
            {
                BaseUnit = x.Unit,
                BaseUnitPrice = x.Price
            }).FirstOrDefaultAsync();
            return data;
        }
    }
}
