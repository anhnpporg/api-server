using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.BatchModel;
using System.Linq;
using UtNhanDrug_BE.Hepper.GenaralBarcode;
using System;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.ResponseModel;

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
                consignment.BatchBarcode = GenaralBarcode.CreateEan13Batch(consignment.Id + "");
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
            foreach (var x in data)
            {
                var currentQuantity = await GetCurrentQuantity(x.Id);
                x.CurrentQuantity = currentQuantity;
            }
            return data;
        }

        public async Task<List<ViewBatchModel>> GetBatchesByProductId(int id)
        {
            var query = from b in _context.Batches
                        where b.ProductId == id
                        select b;
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
            foreach (var x in data)
            {
                var currentQuantity = await GetCurrentQuantity(x.Id);
                x.CurrentQuantity = currentQuantity;
            }
            return data;
        }

        public async Task<ViewBatchModel> GetBatchById(int id)
        {
            var query = from b in _context.Batches
                        where b.Id == id
                        select b;
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
            }).FirstOrDefaultAsync();
            var currentQuantity = await GetCurrentQuantity(data.Id);
            data.CurrentQuantity = currentQuantity;
            return data;
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





        //quantity
        private async Task<List<ViewQuantityModel>> GetCurrentQuantity(int batchId)
        {
            var query = from g in _context.GoodsReceiptNotes
                        where g.BatchId == batchId
                        select g;
            var totalQuantity = await query.Select(x => x.ConvertedQuantity).SumAsync();

            var productId = await query.Select(x => x.Batch.ProductId).SumAsync();
            var query2 = from g in _context.GoodsIssueNotes
                         where g.BatchId == batchId
                         select g;
            var saledQuantity = await query2.Select(x => x.ConvertedQuantity).SumAsync();
            var currentQuantity = totalQuantity - saledQuantity;

            var query3 = from u in _context.ProductUnitPrices
                         where u.ProductId == productId
                         select u;
            var data = await query3.Select(x => new ViewQuantityModel()
            {
                Id = x.Id,
                Unit = x.Unit,
                CurrentQuantity = (int)(currentQuantity /x.ConversionValue)
            }).ToListAsync();
            return data;
        }

        public async Task<Response<ViewBatchModel>> GetBatchesByBarcode(string barcode)
        {
            var query = from b in _context.Batches
                        where b.BatchBarcode.Equals(barcode)
                        select b;
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
            }).FirstOrDefaultAsync();
            if(data != null)
            {
                var currentQuantity = await GetCurrentQuantity(data.Id);
                data.CurrentQuantity = currentQuantity;
                return new Response<ViewBatchModel>(data)
                {
                    Message = "Successfully"
                };
            }
            
            return new Response<ViewBatchModel>(data)
            {
                StatusCode = 400,
                Message = "Not found this batch"
            };
        }
    }
}
