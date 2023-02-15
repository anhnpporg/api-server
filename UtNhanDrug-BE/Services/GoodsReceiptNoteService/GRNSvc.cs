using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.GoodsReceiptNoteModel;
using System.Linq;
using System;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Services.SupplierService;
using UtNhanDrug_BE.Services.BatchService;
using UtNhanDrug_BE.Hepper.GenaralBarcode;
using Microsoft.EntityFrameworkCore.Storage;
using UtNhanDrug_BE.Models.ResponseModel;
using UtNhanDrug_BE.Hepper;
using UtNhanDrug_BE.Services.ProductUnitService;

namespace UtNhanDrug_BE.Services.GoodsReceiptNoteService
{
    public class GRNSvc : IGRNSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly IProductUnitPriceSvc _pu;
        private readonly DateTime today = LocalDateTime.DateTimeNow();
        public GRNSvc(ut_nhan_drug_store_databaseContext context, IProductUnitPriceSvc pu)
        {
            _context = context;
            _pu = pu;
        }
        //public async Task<bool> CheckGoodsReceiptNote(int id)
        //{
        //    var result = await _context.GoodsReceiptNotes.FirstOrDefaultAsync(x => x.Id == id);
        //    if (result == null) return false;
        //    return true;
        //}

        public async Task<Response<List<GRNResponse>>> CreateGoodsReceiptNote(int userId, CreateGoodsReceiptNoteModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                if (model != null)
                {
                    if (model.GoodsReceiptNoteTypeId == 1)
                    {
                        if (model.CreateModel.Count <= 0)
                        {
                            return new Response<List<GRNResponse>>(null)
                            {
                                StatusCode = 400,
                                Message = "Vui lòng chọn sản phẩm để trả"
                            };
                        }
                        List<GRNResponse> grns = new List<GRNResponse>();
                        foreach (var m in model.CreateModel)
                        {


                            // add supplier id, if null add object supplier
                            if (m.SupplierId == null)
                            {
                                var suplierQuery = from su in _context.Suppliers
                                                   select su;
                                var supplierName = await suplierQuery.Where(x => x.Name.ToLower() == m.Supplier.Name.ToLower()).FirstOrDefaultAsync();
                                if (supplierName != null) return new Response<List<GRNResponse>>(null)
                                {
                                    StatusCode = 400,
                                    Message = "Nhà sản xuất đã tồn tại"
                                };
                                Supplier s = new Supplier()
                                {
                                    Name = m.Supplier.Name,
                                    PhoneNumber = m.Supplier.PhoneNumber,
                                    CreatedBy = userId,
                                    CreatedAt = today
                                };
                                var supplier = _context.Suppliers.Add(s);
                                await _context.SaveChangesAsync();
                                m.SupplierId = s.Id;
                            }
                            if (m.SupplierId == null)
                            {
                                await transaction.RollbackAsync();
                                return new Response<List<GRNResponse>>(null)
                                {
                                    StatusCode = 400,
                                    Message = "Vui lòng nhập nhà cung cấp"
                                };
                            }


                            //batches 
                            foreach (var b in m.Batches)
                            {
                                var unit = await _context.ProductUnitPrices.FirstOrDefaultAsync(x => x.Id == b.ProductUnitPriceId);
                                if (b.BatchId == null)
                                {
                                    if (b.Batch.ManufacturingDate > b.Batch.ExpiryDate || b.Batch.ManufacturingDate == b.Batch.ExpiryDate)
                                    {
                                        await transaction.RollbackAsync();
                                        return new Response<List<GRNResponse>>(null)
                                        {
                                            StatusCode = 400,
                                            Message = "Ngày sản xuất phải lớn hơn ngày hết hạn"
                                        };
                                    }
                                    var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == b.Batch.ProductId);
                                    if (product.IsManagedInBatches == false)
                                    {
                                        await transaction.RollbackAsync();
                                        return new Response<List<GRNResponse>>(null)
                                        {
                                            StatusCode = 400,
                                            Message = "Sản phẩm này không quản lí theo lô, không thể tạo thêm lô"
                                        };
                                    }

                                    var checkExitBatch = await CheckExitBath(b.Batch.ExpiryDate, (int)b.Batch.ProductId);
                                    if (checkExitBatch == true) return new Response<List<GRNResponse>>(null)
                                    {
                                        StatusCode = 400,
                                        Message = "Lô có ngày hết hạn này đã tồn tại"
                                    };
                                    Batch s = new Batch()
                                    {
                                        Barcode = "#####",
                                        ProductId = (int)b.Batch.ProductId,
                                        ManufacturingDate = b.Batch.ManufacturingDate,
                                        ExpiryDate = b.Batch.ExpiryDate,
                                        CreatedBy = userId,
                                        CreatedAt = today
                                    };
                                    var batch = _context.Batches.Add(s);
                                    await _context.SaveChangesAsync();
                                    s.Barcode = GenaralBarcode.CreateEan13Batch(s.Id + "");
                                    await _context.SaveChangesAsync();
                                    b.BatchId = s.Id;
                                }
                                int convertedQuantity = (int)(b.Quantity * unit.ConversionValue);
                                decimal baseUnitPrice = (decimal)(b.TotalPrice / convertedQuantity);
                                GoodsReceiptNote grn = new GoodsReceiptNote()
                                {
                                    GoodsReceiptNoteTypeId = model.GoodsReceiptNoteTypeId,
                                    BatchId = (int)b.BatchId,
                                    SupplierId = m.SupplierId,
                                    Quantity = (int)b.Quantity,
                                    Unit = unit.Unit,
                                    TotalPrice = (decimal)b.TotalPrice,
                                    ConvertedQuantity = convertedQuantity,
                                    BaseUnitPrice = baseUnitPrice,
                                    CreatedBy = userId,
                                    CreatedAt = today
                                };
                                _context.GoodsReceiptNotes.Add(grn);
                                await _context.SaveChangesAsync();
                                grns.Add(new GRNResponse { GRNId = grn.Id });
                                b.BatchId = null;
                            }
                        }
                        await transaction.CommitAsync();
                        return new Response<List<GRNResponse>>(grns)
                        {
                            Message = "Tạo phiếu nhập hàng từ nhà cung cấp thành công"
                        };
                    }
                    else if (model.GoodsReceiptNoteTypeId == 2)
                    {

                        if (model.IsFull == false)
                        {
                            if (model.CreateModel.Count <= 0)
                            {
                                return new Response<List<GRNResponse>>(null)
                                {
                                    StatusCode = 400,
                                    Message = "Vui lòng chọn sản phẩm để trả"
                                };
                            }
                            //Convert point
                            //decimal toMoney = 1000;
                            //decimal toPoint = (decimal)await _context.DataSettingSystemCustomerPoints.Select(x => x.ToPoint).FirstOrDefaultAsync();

                            //int point = (int)(totalPrice / toPoint);
                            var customer = await _context.Invoices.Where(x => x.Id == model.InvoiceId).Select(x => x.Customer).FirstOrDefaultAsync();

                            var queryGRN = from g in _context.GoodsReceiptNotes
                                           where g.InvoiceId == model.InvoiceId
                                           select g;
                            var dataGRN = await queryGRN.FirstOrDefaultAsync();
                            int point = 0;
                            int customerPoint = 0;
                            if (dataGRN == null)
                            {
                                if (customer != null)
                                {
                                    point = (int)await _context.CustomerPointTransactions.Where(x => x.InvoiceId == model.InvoiceId & x.IsReciept == true).Select(x => x.Point).FirstOrDefaultAsync();
                                    var query1 = from c in _context.Customers
                                                 where c.Id == customer.Id
                                                 select c;
                                    float totalPoint = (float)await query1.Select(x => x.TotalPoint).FirstOrDefaultAsync();

                                    CustomerPointTransaction cpt = new CustomerPointTransaction()
                                    {
                                        CustomerId = (int)customer.Id,
                                        InvoiceId = (int)model.InvoiceId,
                                        Point = point,
                                        IsReciept = false,
                                    };
                                    await _context.CustomerPointTransactions.AddAsync(cpt);

                                    totalPoint -= point;

                                    customer.TotalPoint = totalPoint;
                                    customerPoint = (int)totalPoint;
                                }
                            }

                            await _context.SaveChangesAsync();

                            decimal totalPrice = 0;
                            List<GRNResponse> grns = new List<GRNResponse>();
                            foreach (var m in model.CreateModel)
                            {
                                //batches 
                                foreach (var b in m.Batches)
                                {
                                    if (b.Quantity > 0)
                                    {
                                        var unit = await _context.ProductUnitPrices.FirstOrDefaultAsync(x => x.Id == b.ProductUnitPriceId);
                                        var query = from g in _context.GoodsIssueNotes
                                                    where g.OrderDetail.InvoiceId == model.InvoiceId
                                                    select g;
                                        var gin = await query.Where(x => x.BatchId == b.BatchId & x.GoodsIssueNoteTypeId == 1).FirstOrDefaultAsync();
                                        double unitPrice = (double)((await query.Where(x => x.BatchId == b.BatchId & x.GoodsIssueNoteTypeId == 1).Select(x => x.UnitPrice).FirstOrDefaultAsync() * await query.Where(x => x.BatchId == b.BatchId & x.GoodsIssueNoteTypeId == 1).Select(x => x.Quantity).FirstOrDefaultAsync()) / await query.Where(x => x.BatchId == b.BatchId & x.GoodsIssueNoteTypeId == 1).Select(x => x.ConvertedQuantity).FirstOrDefaultAsync());
                                        var invoiceQuery = from gn in _context.GoodsReceiptNotes
                                                           where gn.InvoiceId == model.InvoiceId & gn.GoodsReceiptNoteTypeId == 2 & gn.BatchId == b.BatchId
                                                           select gn;
                                        int grnQuantity = await invoiceQuery.Select(x => x.ConvertedQuantity).SumAsync();

                                        if (b.Quantity * unit.ConversionValue > (gin.ConvertedQuantity - grnQuantity))
                                        {
                                            await transaction.RollbackAsync();
                                            return new Response<List<GRNResponse>>(null)
                                            {
                                                StatusCode = 400,
                                                Message = "Số lượng nhập lại từ khách hàng không hợp lệ"
                                            };
                                        }

                                        var query2 = from batch in _context.Batches
                                                     where batch.Id == b.BatchId
                                                     select batch;
                                        var productId = await query2.Select(x => x.ProductId).FirstOrDefaultAsync();
                                        var baseUnit = await _pu.GetBaseUnit(productId);
                                        if (b.BatchId == null)
                                        {
                                            Batch s = new Batch()
                                            {
                                                Barcode = "#####",
                                                ProductId = (int)b.Batch.ProductId,
                                                ManufacturingDate = b.Batch.ManufacturingDate,
                                                ExpiryDate = b.Batch.ExpiryDate,
                                                CreatedBy = userId,
                                                CreatedAt = today
                                            };
                                            var batch = _context.Batches.Add(s);
                                            await _context.SaveChangesAsync();
                                            s.Barcode = GenaralBarcode.CreateEan13Batch(s.Id + "");
                                            await _context.SaveChangesAsync();
                                            b.BatchId = s.Id;
                                        }
                                        int convertedQuantity = (int)(b.Quantity * unit.ConversionValue);
                                        //decimal baseUnitPrice = (decimal)(b.TotalPrice / convertedQuantity);
                                        GoodsReceiptNote grn = new GoodsReceiptNote()
                                        {
                                            GoodsReceiptNoteTypeId = model.GoodsReceiptNoteTypeId,
                                            BatchId = (int)b.BatchId,
                                            InvoiceId = model.InvoiceId,
                                            Quantity = (int)b.Quantity,
                                            Unit = unit.Unit,
                                            //TotalPrice = (decimal)b.TotalPrice,
                                            TotalPrice = (decimal)(unitPrice * b.Quantity),
                                            ConvertedQuantity = convertedQuantity,
                                            BaseUnitPrice = (decimal)baseUnit.Data.BasePrice,
                                            CreatedBy = userId,
                                            CreatedAt = today
                                        };
                                        _context.GoodsReceiptNotes.Add(grn);
                                        await _context.SaveChangesAsync();
                                        grns.Add(new GRNResponse { GRNId = grn.Id, ReturnPoint = point, CustomerPoint = customerPoint });
                                        b.BatchId = null;
                                        totalPrice += grn.ConvertedQuantity * grn.BaseUnitPrice;
                                    }

                                }
                            }

                            await _context.SaveChangesAsync();

                            await transaction.CommitAsync();
                            return new Response<List<GRNResponse>>(grns)
                            {
                                Message = "Nhập hàng từ hoá đơn thành công"
                            };
                        }
                        else
                        {

                            var queryGRN = from g in _context.GoodsReceiptNotes
                                           where g.InvoiceId == model.InvoiceId
                                           select g;
                            var dataGRN = await queryGRN.FirstOrDefaultAsync();
                            if (dataGRN != null)
                            {
                                await transaction.RollbackAsync();
                                return new Response<List<GRNResponse>>(null)
                                {
                                    StatusCode = 400,
                                    Message = "Đã nhập hàng từ hoá đơn này, không thể nhập lại tất cả"
                                };
                            }

                            int point = 0;
                            int customerPoint = 0;

                            //Convert point
                            //decimal toMoney = 1000;
                            //decimal toPoint = 10000;

                            //int point = (int)(totalPrice / toPoint);
                            var customer = await _context.Invoices.Where(x => x.Id == model.InvoiceId).Select(x => x.Customer).FirstOrDefaultAsync();
                            if (customer != null)
                            {
                                point = (int)await _context.CustomerPointTransactions.Where(x => x.InvoiceId == model.InvoiceId & x.IsReciept == true).Select(x => x.Point).FirstOrDefaultAsync();
                                var query2 = from c in _context.Customers
                                             where c.Id == customer.Id
                                             select c;
                                float totalPoint = (float)await query2.Select(x => x.TotalPoint).FirstOrDefaultAsync();

                                CustomerPointTransaction cpt = new CustomerPointTransaction()
                                {
                                    CustomerId = (int)customer.Id,
                                    InvoiceId = (int)model.InvoiceId,
                                    Point = point,
                                    IsReciept = false,
                                };
                                await _context.CustomerPointTransactions.AddAsync(cpt);

                                totalPoint -= point;
                                customer.TotalPoint = totalPoint;
                                customerPoint = (int)totalPoint;
                            }
                            //get customer total point

                            await _context.SaveChangesAsync();

                            var query = from g in _context.GoodsIssueNotes
                                        where g.OrderDetail.InvoiceId == model.InvoiceId
                                        select g;
                            var gin = await query.ToListAsync();
                            decimal totalPrice = 0;
                            List<GRNResponse> grns = new List<GRNResponse>();
                            foreach (var o in gin)
                            {
                                if (o.Quantity != 0)
                                {
                                    var query1 = from b in _context.Batches
                                                 where b.Id == o.BatchId
                                                 select b;
                                    var productId = await query1.Select(x => x.ProductId).FirstOrDefaultAsync();
                                    var baseUnit = await _pu.GetBaseUnit(productId);
                                    GoodsReceiptNote grn = new GoodsReceiptNote()
                                    {
                                        GoodsReceiptNoteTypeId = model.GoodsReceiptNoteTypeId,
                                        BatchId = o.BatchId,
                                        InvoiceId = model.InvoiceId,
                                        Quantity = (int)o.Quantity,
                                        Unit = o.Unit,
                                        TotalPrice = (decimal)o.UnitPrice * o.Quantity,
                                        ConvertedQuantity = o.ConvertedQuantity,
                                        BaseUnitPrice = (decimal)baseUnit.Data.BasePrice,
                                        CreatedBy = userId,
                                        CreatedAt = today
                                    };
                                    _context.GoodsReceiptNotes.Add(grn);
                                    await _context.SaveChangesAsync();
                                    grns.Add(new GRNResponse { GRNId = grn.Id, ReturnPoint = point, CustomerPoint = customerPoint });
                                    totalPrice += grn.TotalPrice;
                                }
                            }


                            await _context.SaveChangesAsync();

                            await transaction.CommitAsync();
                            return new Response<List<GRNResponse>>(grns)
                            {
                                Message = "Nhập hàng từ hoá đơn thành công"
                            };

                        }
                    }
                    //else if (model.GoodsReceiptNoteTypeId == 3)
                    //{

                    //}
                }
                return new Response<List<GRNResponse>>(null)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy sản phẩm để nhập hàng"
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                if (e.InnerException.Message.Contains("Arithmetic overflow error converting numeric to data type money"))
                {
                    return new Response<List<GRNResponse>>(null)
                    {
                        StatusCode = 500,
                        Message = "Tiền nhập vào hệ thống không hợp lệ"
                    };
                }
                return new Response<List<GRNResponse>>(null)
                {
                    StatusCode = 500,
                    Message = "Tạo phiếu nhập hàng thất bại"
                };
            }
        }

        //private async Task<int> GetCurrentQuantity(int batchId)
        //{
        //    var query = from g in _context.GoodsReceiptNotes
        //                where g.BatchId == batchId
        //                select g;
        //    var totalQuantity = await query.Select(x => x.ConvertedQuantity).SumAsync();

        //    var productId = await query.Select(x => x.Batch.ProductId).SumAsync();
        //    var query2 = from g in _context.GoodsIssueNotes
        //                 where g.BatchId == batchId
        //                 select g;
        //    var saledQuantity = await query2.Select(x => x.ConvertedQuantity).SumAsync();
        //    var currentQuantity = totalQuantity - saledQuantity;
        //    return currentQuantity;
        //}
        private async Task<bool> CheckExitBath(DateTime? expiryDate, int productId)
        {
            var queryBatch = from b in _context.Batches
                             where b.ProductId == productId & b.IsActive == true & b.ExpiryDate == expiryDate
                             select b;
            var data = await queryBatch.FirstOrDefaultAsync();

            if (data != null) return true;
            return false;
        }

        public async Task<Response<List<ViewGoodsReceiptNoteModel>>> GetAllGoodsReceiptNote()
        {
            try
            {
                var query = from grn in _context.GoodsReceiptNotes
                            select grn;
                var data = await query.OrderByDescending(x => x.CreatedAt).Select(x => new ViewGoodsReceiptNoteModel()
                {
                    Id = x.Id,
                    GoodsReceiptNoteType = new ViewModel()
                    {
                        Id = x.GoodsReceiptNoteType.Id,
                        Name = x.GoodsReceiptNoteType.Name
                    },
                    Batch = new ViewBatch()
                    {
                        Id = x.Batch.Id,
                        Barcode = x.Batch.Barcode,
                        ManufacturingDate = x.Batch.ManufacturingDate,
                        ExpiryDate = x.Batch.ExpiryDate
                    },
                    Supplier = new ViewSupplierData()
                    {
                        Id = x.Supplier.Id,
                        Name = x.Supplier.Name,
                        IsActive = x.Supplier.IsActive
                    },
                    Quantity = x.Quantity,
                    Unit = x.Unit,
                    InvoiceId = x.InvoiceId,
                    ConvertedQuantity = x.ConvertedQuantity,
                    TotalPrice = x.TotalPrice,
                    BaseUnitPrice = x.BaseUnitPrice,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                }).ToListAsync();
                if (data.Count > 0)
                {
                    foreach (var d in data)
                    {
                        var notes = await GetListNoteLog(d.Id);
                        d.Note = notes.Data;
                    }
                    return new Response<List<ViewGoodsReceiptNoteModel>>(data)
                    {
                        Message = "Thông tin tất cả phiếu nhập hàng"
                    };
                }
                else
                {
                    return new Response<List<ViewGoodsReceiptNoteModel>>(null)
                    {
                        Message = "Không có phiếu nhập hàng nào"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewGoodsReceiptNoteModel>>(null)
                {
                    StatusCode = 500,
                    Message = "Đã có lỗi xảy ra"
                };
            }

        }

        public async Task<Response<ViewGoodsReceiptNoteModel>> GetGoodsReceiptNoteById(int id)
        {
            try
            {
                var query = from c in _context.GoodsReceiptNotes
                            where c.Id == id
                            select c;
                var data = await query.Select(c => new ViewGoodsReceiptNoteModel()
                {
                    Id = c.Id,
                    GoodsReceiptNoteType = new ViewModel()
                    {
                        Id = c.GoodsReceiptNoteType.Id,
                        Name = c.GoodsReceiptNoteType.Name
                    },
                    Batch = new ViewBatch()
                    {
                        Id = c.Batch.Id,
                        Barcode = c.Batch.Barcode,
                        ManufacturingDate = c.Batch.ManufacturingDate,
                        ExpiryDate = c.Batch.ExpiryDate
                    },
                    Supplier = new ViewSupplierData()
                    {
                        Id = c.Supplier.Id,
                        Name = c.Supplier.Name,
                        IsActive = c.Supplier.IsActive
                    },
                    Quantity = c.Quantity,
                    Unit = c.Unit,
                    InvoiceId = c.InvoiceId,
                    ConvertedQuantity = c.ConvertedQuantity,
                    TotalPrice = c.TotalPrice,
                    BaseUnitPrice = c.BaseUnitPrice,
                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt,
                }).FirstOrDefaultAsync();
                if (data != null)
                {
                    var notes = await GetListNoteLog(id);
                    data.Note = notes.Data;
                    return new Response<ViewGoodsReceiptNoteModel>(data)
                    {
                        Message = "Thông tin phiếu nhập hàng"
                    };
                }
                return new Response<ViewGoodsReceiptNoteModel>(null)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy phiếu nhập hàng này"
                };
            }
            catch (Exception)
            {
                return new Response<ViewGoodsReceiptNoteModel>(null)
                {
                    StatusCode = 500,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        private async Task<Response<List<NoteLog>>> GetListNoteLog(int id)
        {
            try
            {
                var query = from x in _context.GoodsReceiptNoteLogs
                            where x.GoodsReceiptNoteId == id
                            select x;
                var data = await query.OrderByDescending(x => x.UpdatedAt).Select(x => new NoteLog()
                {
                    Id = x.Id,
                    Note = x.Note,
                    UpdateAt = x.UpdatedAt,
                    UpdateBy = new ViewModel()
                    {
                        Id = x.UpdatedByNavigation.Id,
                        Name = x.UpdatedByNavigation.FullName
                    }

                }).ToListAsync();
                if (data.Count > 0)
                {
                    return new Response<List<NoteLog>>(data)
                    {
                        Message = "Thông tin đầy đủ của phiếu nhập hàng"
                    };
                }
                else
                {
                    return new Response<List<NoteLog>>(null)
                    {
                        Message = "Không tìm thấy thông tin của phiếu nhập hàng này"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<NoteLog>>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }

        }

        public async Task<Response<List<ViewModel>>> GetListNoteTypes()
        {
            try
            {
                var query = from x in _context.GoodsReceiptNoteTypes
                            select x;
                var data = await query.Select(x => new ViewModel()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();
                if (data.Count > 0)
                {
                    return new Response<List<ViewModel>>(data)
                    {
                        Message = "Phân loại nhập hàng"
                    };
                }
                else
                {
                    return new Response<List<ViewModel>>(null)
                    {
                        Message = "Không có thông tin"
                    };
                }
            }
            catch
            {
                return new Response<List<ViewModel>>()
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<bool>> UpdateGoodsReceiptNote(int id, int userId, UpdateGoodsReceiptNoteModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var unit = await _context.ProductUnitPrices.FirstOrDefaultAsync(x => x.Id == model.ProductUnitPriceId);
                var c = await _context.GoodsReceiptNotes.FirstOrDefaultAsync(x => x.Id == id);
                if (c != null)
                {
                    c.GoodsReceiptNoteTypeId = model.GoodsReceiptNoteTypeId;
                    c.BatchId = model.BatchId;
                    c.SupplierId = model.SupplierId;
                    c.Quantity = model.Quantity;
                    c.Unit = unit.Unit;
                    c.TotalPrice = model.TotalPrice;
                    c.ConvertedQuantity = (int)(model.Quantity * unit.ConversionValue);
                    c.BaseUnitPrice = model.TotalPrice / ((int)(model.Quantity * unit.ConversionValue));
                    GoodsReceiptNoteLog g = new GoodsReceiptNoteLog()
                    {
                        GoodsReceiptNoteId = id,
                        Note = model.Note,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = userId,
                    };
                    _context.GoodsReceiptNoteLogs.Add(g);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Cập nhật thành công"
                    };
                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy phiếu nhập hàng nào"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 500,
                    Message = "Cập nhật không thành công"
                };
            }
        }

        public async Task<Response<List<ViewGoodsReceiptNoteModel>>> GetGoodsReceiptNoteByType(int type)
        {
            try
            {
                var query = from grn in _context.GoodsReceiptNotes
                            select grn;
                var data = await query.OrderByDescending(x => x.CreatedAt).Where(x => x.GoodsReceiptNoteTypeId == type).Select(x => new ViewGoodsReceiptNoteModel()
                {
                    Id = x.Id,
                    GoodsReceiptNoteType = new ViewModel()
                    {
                        Id = x.GoodsReceiptNoteType.Id,
                        Name = x.GoodsReceiptNoteType.Name
                    },
                    Batch = new ViewBatch()
                    {
                        Id = x.Batch.Id,
                        Barcode = x.Batch.Barcode,
                        ManufacturingDate = x.Batch.ManufacturingDate,
                        ExpiryDate = x.Batch.ExpiryDate
                    },
                    Supplier = new ViewSupplierData()
                    {
                        Id = x.Supplier.Id,
                        Name = x.Supplier.Name,
                        IsActive = x.Supplier.IsActive
                    },
                    Quantity = x.Quantity,
                    Unit = x.Unit,
                    InvoiceId = x.InvoiceId,
                    ConvertedQuantity = x.ConvertedQuantity,
                    TotalPrice = x.TotalPrice,
                    BaseUnitPrice = x.BaseUnitPrice,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                }).ToListAsync();
                if (data.Count > 0)
                {
                    foreach (var d in data)
                    {
                        var notes = await GetListNoteLog(d.Id);
                        d.Note = notes.Data;
                    }
                    return new Response<List<ViewGoodsReceiptNoteModel>>(data)
                    {
                        Message = "Thông tin tất cả phiếu nhập hàng"
                    };
                }
                else
                {
                    return new Response<List<ViewGoodsReceiptNoteModel>>(null)
                    {
                        Message = "Không có phiếu nhập hàng nào"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewGoodsReceiptNoteModel>>(null)
                {
                    StatusCode = 500,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<List<ViewGoodsReceiptNoteModel>>> GetGoodsReceiptNoteByStaff(int staffId)
        {
            try
            {
                var query = from grn in _context.GoodsReceiptNotes
                            where grn.CreatedBy == staffId
                            select grn;
                var data = await query.OrderByDescending(x => x.CreatedAt).Select(x => new ViewGoodsReceiptNoteModel()
                {
                    Id = x.Id,
                    GoodsReceiptNoteType = new ViewModel()
                    {
                        Id = x.GoodsReceiptNoteType.Id,
                        Name = x.GoodsReceiptNoteType.Name
                    },
                    Batch = new ViewBatch()
                    {
                        Id = x.Batch.Id,
                        Barcode = x.Batch.Barcode,
                        ManufacturingDate = x.Batch.ManufacturingDate,
                        ExpiryDate = x.Batch.ExpiryDate
                    },
                    Supplier = new ViewSupplierData()
                    {
                        Id = x.Supplier.Id,
                        Name = x.Supplier.Name,
                        IsActive = x.Supplier.IsActive
                    },
                    Quantity = x.Quantity,
                    Unit = x.Unit,
                    InvoiceId = x.InvoiceId,
                    ConvertedQuantity = x.ConvertedQuantity,
                    TotalPrice = x.TotalPrice,
                    BaseUnitPrice = x.BaseUnitPrice,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                }).ToListAsync();
                if (data.Count > 0)
                {
                    foreach (var d in data)
                    {
                        var notes = await GetListNoteLog(d.Id);
                        d.Note = notes.Data;
                    }
                    return new Response<List<ViewGoodsReceiptNoteModel>>(data)
                    {
                        Message = "Thông tin tất cả phiếu nhập hàng"
                    };
                }
                else
                {
                    return new Response<List<ViewGoodsReceiptNoteModel>>(null)
                    {
                        Message = "Không có phiếu nhập hàng nào"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewGoodsReceiptNoteModel>>(null)
                {
                    StatusCode = 500,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }
    }
}
