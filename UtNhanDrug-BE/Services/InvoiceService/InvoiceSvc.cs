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

namespace UtNhanDrug_BE.Services.InvoiceService
{
    public class InvoiceSvc : IInvoiceSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly IUserSvc _userSvc;
        private readonly IProductUnitPriceSvc _unitSvc;
        public InvoiceSvc(ut_nhan_drug_store_databaseContext context, IUserSvc userSvc, IProductUnitPriceSvc unitSvc)
        {
            _context = context;
            _userSvc = userSvc;
            _unitSvc = unitSvc;
        }
        public async Task<Response<bool>> CreateInvoice(int UserId, CreateInvoiceModel model)
        {
            if (model.CustomerId == null)
            {
                var customer = await _userSvc.CreateCustomer(UserId, model.Customer);
                if (customer != null)
                {
                    model.CustomerId = customer.Data.Id;
                }
                else
                {
                    return new Response<bool>(false)
                    {
                        StatusCode = 400,
                        Message = "Khách hàng này đã tồn tại"
                    };
                }
            }
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                
                Invoice i = new Invoice()
                {
                    CustomerId = model.CustomerId,
                    BodyWeight = model.BodyWeight,
                    DayUse = model.DayUse,
                    TotalPrice = 0,
                    CreatedBy = UserId
                };
                _context.Invoices.Add(i);
                await _context.SaveChangesAsync();
                foreach (OrderDetailModel x in model.Product)
                {

                    decimal price = 0;
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == x.ProductId);

                    OrderDetail o = new OrderDetail()
                    {
                        InvoiceId = i.Id,
                        ProductId = x.ProductId,
                        Dose = x.Dose,
                        UnitDose = x.UnitDose,
                        Frequency = x.Frequency,
                        DayUse = x.DayUse,
                        Use = x.Use,
                    };
                    _context.OrderDetails.Add(o);
                    await _context.SaveChangesAsync();

                    foreach (var goods in x.GoodsIssueNote)
                    {
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

                        GoodsIssueNote g = new GoodsIssueNote()
                        {
                            GoodsIssueNoteTypeId = 1,
                            OrderDetailId = o.Id,
                            BatchId = goods.BatchId,
                            Quantity = goods.Quantity,
                            Unit = unit.Unit,
                            UnitPrice = price,
                            ConvertedQuantity = ConvertedQuantity
                        };
                        _context.GoodsIssueNotes.Add(g);
                        o.TotalPrice = g.UnitPrice * g.Quantity;
                        i.TotalPrice += o.TotalPrice;
                    }
                }
                var r = await _context.SaveChangesAsync();
                if (r > 0)
                {
                    await transaction.CommitAsync();
                }
                return new Response<bool>(true)
                {
                    Message = "Tạo hoá đơn thành công"
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tạo hoá đơn thất bại",
                    Errors = new string[]{
                        e.Message
                    }
                };
            }

        }

        public async Task<Response<List<ViewInvoiceModel>>> GetAllInvoice()
        {
            var query = from i in _context.Invoices
                        select i;
            var data = await query.Select(x => new ViewInvoiceModel()
            {
                Id = x.Id,
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

        public async Task<Response<List<ViewInvoiceModel>>> GetInvoiceCustomerId(int customerId)
        {
            var query = from i in _context.Invoices
                        where i.CustomerId == customerId
                        select i;
            var data = await query.Select(x => new ViewInvoiceModel()
            {
                Id = x.Id,
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
            return new Response<List<ViewOrderDetailModel>>(data);
        }
    }
}
