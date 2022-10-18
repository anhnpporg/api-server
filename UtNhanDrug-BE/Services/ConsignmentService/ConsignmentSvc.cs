using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ConsignmentModel;
using System.Linq;
using UtNhanDrug_BE.Hepper.GenaralBarcode;
using System;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Services.ConsignmentService
{
    public class ConsignmentSvc : IConsignmentSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;

        public ConsignmentSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckConsignment(int id)
        {
            var result = await _context.Consignments.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null) return true;
            return false;
        }

        public async Task<bool> CreateConsignment(int userId, CreateConsignmentModel model)
        {
            Consignment consignment = new Consignment()
            {
                Barcode = "#####",
                ProductId = model.ProductId,
                ManufacturingDate = model.ManufacturingDate,
                ExpiryDate = model.ExpiryDate,
                CreatedBy = userId,
            };
            _context.Consignments.Add(consignment);
            var result = await _context.SaveChangesAsync();
            if (result > 0) 
            {
                consignment.Barcode = GenaralBarcode.CreateEan13(consignment.Id + "");
                await _context.SaveChangesAsync();
                return true;
            } 
            return false;
        }

        public async Task<bool> DeleteConsignment(int id, int userId)
        {
            var result = await _context.Consignments.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null)
            {
                result.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ViewConsignmentModel>> GetAllConsignment()
        {
            var query = from c in _context.Consignments
                        select c;
            var data = await query.Select(x => new ViewConsignmentModel()
            {
                Id = x.Id,
                Barcode = x.Barcode,
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
                    Id = x.CreatedByNavigation.UserAccount.Id,
                    Name = x.CreatedByNavigation.UserAccount.FullName
                },
            }).ToListAsync();
            return data;
        }

        public async Task<ViewConsignmentModel> GetConsignmentById(int id)
        {
            var c = await _context.Consignments.FirstOrDefaultAsync(x => x.Id == id);
            if (c != null)
            {
                ViewConsignmentModel result = new ViewConsignmentModel()
                {
                    Id = c.Id,
                    Barcode = c.Barcode,
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
                        Id = c.CreatedByNavigation.UserAccount.Id,
                        Name = c.CreatedByNavigation.UserAccount.FullName
                    },
                };
                return result;
            }
            return null;
        }

        public async Task<bool> UpdateConsignment(int id, int userId, UpdateConsignmentModel model)
        {
            var c = await _context.Consignments.FirstOrDefaultAsync(x => x.Id == id);
            if (c != null)
            {
                c.ProductId = model.ProductId;
                c.ManufacturingDate = model.ManufacturingDate;
                c.ExpiryDate = model.ExpiryDate;
                c.IsActive = model.IsActive;
                c.UpdatedAt = DateTime.Now;
                c.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
