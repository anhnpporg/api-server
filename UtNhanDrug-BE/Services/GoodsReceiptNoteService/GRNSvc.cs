using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.GoodsReceiptNoteModel;
using System.Linq;
using System;
using UtNhanDrug_BE.Models.ModelHelper;

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

        public async Task<bool> CreateGoodsReceiptNote(int userId, CreateGoodsReceiptNoteModel model)
        {
            GoodsReceiptNote grn = new GoodsReceiptNote()
            {
                GoodsReceiptNoteTypeId = model.GoodsReceiptNoteTypeId,
                ConsignmentId = model.ConsignmentId,
                SupplierId = model.SupplierId,
                Quantity = model.Quantity,
                UnitId = model.UnitId,
                PurchasePrice = model.PurchasePrice,
                ConvertedQuantity = model.ConvertedQuantity,
                PurchasePriceBaseUnit = model.PurchasePriceBaseUnit,
                CreatedBy = userId
            };
            _context.GoodsReceiptNotes.Add(grn);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;
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
                Consignment = new ViewConsignment()
                {
                    Id = x.Consignment.Id,
                    Barcode = x.Consignment.Barcode,
                    ManufacturingDate = x.Consignment.ManufacturingDate,
                    ExpiryDate = x.Consignment.ExpiryDate
                },
                Supplier = new ViewModel()
                {
                    Id = x.Supplier.Id,
                    Name = x.Supplier.Name
                },
                Quantity = x.Quantity,
                Unit = new ViewModel()
                {
                    Id = x.Unit.Id,
                    Name = x.Unit.Name
                },
                PurchasePrice = x.PurchasePrice,
                ConvertedQuantity = x.ConvertedQuantity,
                PurchasePriceBaseUnit = x.PurchasePriceBaseUnit,
                CreatedBy = new ViewModel()
                {
                    Id = x.CreatedByNavigation.UserAccount.Id,
                    Name = x.CreatedByNavigation.UserAccount.FullName
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
                    Consignment = new ViewConsignment()
                    {
                        Id = c.Consignment.Id,
                        Barcode = c.Consignment.Barcode,
                        ManufacturingDate = c.Consignment.ManufacturingDate,
                        ExpiryDate = c.Consignment.ExpiryDate
                    },
                    Supplier = new ViewModel()
                    {
                        Id = c.Supplier.Id,
                        Name = c.Supplier.Name
                    },
                    Quantity = c.Quantity,
                    Unit = new ViewModel()
                    {
                        Id = c.Unit.Id,
                        Name = c.Unit.Name
                    },
                    PurchasePrice = c.PurchasePrice,
                    ConvertedQuantity = c.ConvertedQuantity,
                    PurchasePriceBaseUnit = c.PurchasePriceBaseUnit,
                    CreatedBy = new ViewModel()
                    {
                        Id = c.CreatedByNavigation.UserAccount.Id,
                        Name = c.CreatedByNavigation.UserAccount.FullName
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
                    Name = x.UpdatedByNavigation.UserAccount.FullName
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
            var c = await _context.GoodsReceiptNotes.FirstOrDefaultAsync(x => x.Id == id);
            if (c != null)
            {
                c.GoodsReceiptNoteTypeId = model.GoodsReceiptNoteTypeId;
                c.ConsignmentId = model.ConsignmentId;
                c.SupplierId = model.SupplierId;
                c.Quantity = model.Quantity;
                c.UnitId = model.UnitId;
                c.PurchasePrice = model.PurchasePrice;
                c.ConvertedQuantity = model.ConvertedQuantity;
                c.PurchasePriceBaseUnit = model.PurchasePriceBaseUnit;
                GoodsReceiptNoteLog g = new GoodsReceiptNoteLog()
                {
                    GoodsReceiptNoteId = id,
                    Note = model.Note,
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
