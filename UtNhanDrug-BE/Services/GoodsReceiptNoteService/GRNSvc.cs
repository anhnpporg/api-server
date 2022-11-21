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

namespace UtNhanDrug_BE.Services.GoodsReceiptNoteService
{
    public class GRNSvc : IGRNSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly DateTime today = LocalDateTime.DateTimeNow();
        public GRNSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        //public async Task<bool> CheckGoodsReceiptNote(int id)
        //{
        //    var result = await _context.GoodsReceiptNotes.FirstOrDefaultAsync(x => x.Id == id);
        //    if (result == null) return false;
        //    return true;
        //}

        public async Task<Response<bool>> CreateGoodsReceiptNote(int userId, List<CreateGoodsReceiptNoteModel> model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                if (model.Count > 0)
                {
                    foreach (var m in model)
                    {
                        // add supplier id, if null add object supplier
                        if(m.GoodsReceiptNoteTypeId == 1){
                            if (m.SupplierId == null)
                            {
                                Supplier s = new Supplier()
                                {
                                    Name = m.Supplier.Name,
                                    CreatedBy = userId,
                                    CreatedAt = today
                                };
                                var supplier = _context.Suppliers.Add(s);
                                await _context.SaveChangesAsync();
                                m.SupplierId = s.Id;
                            }
                        }
                        //batches 
                        foreach (var b in m.Batches)
                        {
                            var unit = await _context.ProductUnitPrices.FirstOrDefaultAsync(x => x.Id == b.ProductUnitPriceId);
                            if (b.BatchId == null)
                            {
                                Batch s = new Batch()
                                {
                                    BatchBarcode = "#####",
                                    ProductId = (int)b.Batch.ProductId,
                                    ManufacturingDate = b.Batch.ManufacturingDate,
                                    ExpiryDate = b.Batch.ExpiryDate,
                                    CreatedBy = userId,
                                };
                                var batch = _context.Batches.Add(s);
                                await _context.SaveChangesAsync();
                                s.BatchBarcode = GenaralBarcode.CreateEan13Batch(s.Id + "");
                                await _context.SaveChangesAsync();
                                b.BatchId = s.Id;
                            }
                            int convertedQuantity = (int)(b.Quantity * unit.ConversionValue);
                            decimal baseUnitPrice = b.TotalPrice / convertedQuantity;
                            GoodsReceiptNote grn = new GoodsReceiptNote()
                            {
                                GoodsReceiptNoteTypeId = m.GoodsReceiptNoteTypeId,
                                BatchId = (int)b.BatchId,
                                InvoiceId = m.InvoiceId,
                                SupplierId = m.SupplierId,
                                Quantity = b.Quantity,
                                Unit = unit.Unit,
                                TotalPrice = b.TotalPrice,
                                ConvertedQuantity = convertedQuantity,
                                BaseUnitPrice = baseUnitPrice,
                                CreatedBy = userId
                            };
                            _context.GoodsReceiptNotes.Add(grn);
                            await _context.SaveChangesAsync();
                            b.BatchId = null;
                        }
                    }
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Tạo phiếu nhập hàng thành công"
                    };
                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy sản phẩm để nhập hàng"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
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
                        Barcode = x.Batch.BatchBarcode,
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
                    StatusCode = 400,
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
                        Barcode = c.Batch.BatchBarcode,
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
                    StatusCode = 400,
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
                    StatusCode = 400,
                    Message = "Cập nhật không thành công"
                };
            }
        }
    }
}
