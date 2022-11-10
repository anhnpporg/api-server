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

namespace UtNhanDrug_BE.Services.GoodsReceiptNoteService
{
    public class GRNSvc : IGRNSvc  
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        public GRNSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckGoodsReceiptNote(int id)
        {
            var result = await _context.GoodsReceiptNotes.FirstOrDefaultAsync(x => x.Id == id);
            if (result == null) return false;
            return true;
        }

        public async Task<Response<bool>> CreateGoodsReceiptNote(int userId, List<CreateGoodsReceiptNoteModel> model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                foreach(var m in model)
                {
                    // add supplier id, if null add object supplier
                    if (m.SupplierId == null)
                    {
                        Supplier s = new Supplier()
                        {
                            Name = m.Supplier.Name,
                            CreatedBy = userId
                        };
                        var supplier = _context.Suppliers.Add(s);
                        await _context.SaveChangesAsync();
                        m.SupplierId = s.Id;
                    }
                    
                    //batches 
                    foreach(var b in m.Batches)
                    {
                        var unit = await _context.ProductUnitPrices.FirstOrDefaultAsync(x => x.Id == b.ProductUnitPriceId);
                        if (b.BatchId == null)
                        {
                            Batch s = new Batch()
                            {
                                BatchBarcode = "#####",
                                ProductId = b.Batch.ProductId,
                                ManufacturingDate = b.Batch.ManufacturingDate,
                                ExpiryDate = b.Batch.ExpiryDate,
                                CreatedBy = userId,
                            };
                            var batch = _context.Batches.Add(s);
                            await _context.SaveChangesAsync();
                            s.BatchBarcode = GenaralBarcode.CreateEan13Batch(s.Id + "");
                            await _context.SaveChangesAsync();
                            b.BatchId = s.Id;

                            int convertedQuantity = (int)(b.Quantity * unit.ConversionValue);
                            decimal baseUnitPrice = b.TotalPrice / convertedQuantity;
                            GoodsReceiptNote grn = new GoodsReceiptNote()
                            {
                                GoodsReceiptNoteTypeId = m.GoodsReceiptNoteTypeId,
                                BatchId = b.BatchId,
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
                            await transaction.CommitAsync();
                        }
                    }

                }
                return new Response<bool>(true)
                {
                    Message = "Tạo phiếu nhập hàng thành công"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tạo phiếu nhập hàng thất bại"
                };
            }
        }

        public async Task<List<ViewGoodsReceiptNoteModel>> GetAllGoodsReceiptNote()
        {
            var query = from grn in _context.GoodsReceiptNotes
                        select grn;
            var data = await query.Select(x => new ViewGoodsReceiptNoteModel()
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
                Supplier = new ViewModel()
                {
                    Id = x.Supplier.Id,
                    Name = x.Supplier.Name
                },
                Quantity = x.Quantity,
                Unit = x.Unit,
                InvoiceId = x.InvoiceId,
                ConvertedQuantity = x.ConvertedQuantity,
                TotalPrice = x.TotalPrice,
                BaseUnitPrice = x.BaseUnitPrice,
                CreatedBy = new ViewModel()
                {
                    Id = x.CreatedByNavigation.Id,
                    Name = x.CreatedByNavigation.FullName
                },
                CreatedAt = x.CreatedAt,
            }).ToListAsync();

            return data;
        }

        public async Task<ViewGoodsReceiptNoteModel> GetGoodsReceiptNoteById(int id)
        {
            var c = await _context.GoodsReceiptNotes.FirstOrDefaultAsync(x => x.Id == id);
            if (c != null)
            {
                ViewGoodsReceiptNoteModel result = new ViewGoodsReceiptNoteModel()
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
                    Supplier = new ViewModel()
                    {
                        Id = c.Supplier.Id,
                        Name = c.Supplier.Name
                    },
                    Quantity = c.Quantity,
                    Unit = c.Unit,
                    InvoiceId = c.InvoiceId,
                    ConvertedQuantity = c.ConvertedQuantity,
                    TotalPrice = c.TotalPrice,
                    BaseUnitPrice = c.BaseUnitPrice,
                    CreatedBy = new ViewModel()
                    {
                        Id = c.CreatedByNavigation.Id,
                        Name = c.CreatedByNavigation.FullName
                    },
                    CreatedAt = c.CreatedAt,
                };
                return result;
            }
            return null;
        }

        public async Task<List<NoteLog>> GetListNoteLog(int id)
        {
            var query = from x in _context.GoodsReceiptNoteLogs
                        where x.GoodsReceiptNoteId == id
                        select x;
            var data = await query.Select(x => new NoteLog()
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
            return data;
        }

        public async Task<List<ViewModel>> GetListNoteTypes()
        {
            var query = from x in _context.GoodsReceiptNoteTypes
                        select x;
            var data = await query.Select(x => new ViewModel()
            {
                Id= x.Id,
                Name = x.Name
            }).ToListAsync();
            return data;
        }

        public async Task<bool> UpdateGoodsReceiptNote(int id, int userId, UpdateGoodsReceiptNoteModel model)
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
                var result = await _context.SaveChangesAsync();
                if(result >= 0) return true;
            }
            return false;
        }
    }
}
