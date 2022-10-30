using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.BatchModel;
using System.Linq;
using UtNhanDrug_BE.Hepper.GenaralBarcode;
using System;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Services.BatchService
{
    public class BatchSvc : IBatchSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;

        public BatchSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckBatch(int id)
        {
            var result = await _context.Batches.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null) return true;
            return false;
        }

        public async Task<bool> CreateBatch(int userId, CreateBatchModel model)
        {
            Batch consignment = new Batch()
            {
                BatchBarcode = "#####",
                ProductId = model.ProductId,
                ManufacturingDate = model.ManufacturingDate,
                ExpiryDate = model.ExpiryDate,
                CreatedBy = userId,
            };
            _context.Batches.Add(consignment);
            var result = await _context.SaveChangesAsync();
            if (result > 0) 
            {
                consignment.BatchBarcode = GenaralBarcode.CreateEan13(consignment.Id + "");
                await _context.SaveChangesAsync();
                return true;
            } 
            return false;
        }

        public async Task<bool> DeleteBatch(int id, int userId)
        {
            var result = await _context.Batches.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null)
            {
                result.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ViewBatchModel>> GetAllBatch()
        {
            var query = from c in _context.Batches
                        select c;
            var data = await query.Select(x => new ViewBatchModel()
            {
                Id = x.Id,
                BatchBarcode = x.BatchBarcode,
                Product = new ViewModel()
                {
                    Id = x.Product.Id,
                    Name = x.Product.Name
                },
                ManufacturingDate = x.ManufacturingDate,
                ExpiryDate = x.ExpiryDate,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                CreatedBy = new ViewModel()
                {
                    Id = x.CreatedByNavigation.Id,
                    Name = x.CreatedByNavigation.FullName
                },
            }).ToListAsync();
            return data;
        }

        public async Task<ViewBatchModel> GetBatchById(int id)
        {
            var c = await _context.Batches.FirstOrDefaultAsync(x => x.Id == id);
            if (c != null)
            {
                ViewBatchModel result = new ViewBatchModel()
                {
                    Id = c.Id,
                    BatchBarcode = c.BatchBarcode,
                    Product = new ViewModel()
                    {
                        Id = c.Product.Id,
                        Name = c.Product.Name
                    },
                    ManufacturingDate = c.ManufacturingDate,
                    ExpiryDate = c.ExpiryDate,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = new ViewModel()
                    {
                        Id = c.CreatedByNavigation.Id,
                        Name = c.CreatedByNavigation.FullName
                    },
                };
                return result;
            }
            return null;
        }

        public async Task<bool> UpdateBatch(int id, int userId, UpdateBatchModel model)
        {
            var c = await _context.Batches.FirstOrDefaultAsync(x => x.Id == id);
            if (c != null)
            {
                c.ProductId = model.ProductId;
                c.ManufacturingDate = model.ManufacturingDate;
                c.ExpiryDate = model.ExpiryDate;
                c.UpdatedAt = DateTime.Now;
                c.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
