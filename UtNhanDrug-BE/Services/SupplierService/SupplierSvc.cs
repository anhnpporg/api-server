using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Models.SupplierModel;
using System.Linq;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Services.SupplierService
{
    public class SupplierSvc : ISupplierSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;

        public SupplierSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckSupplier(int supplierId)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == supplierId);
            if (supplier != null) return true;
            return false;
        }

        public async Task<bool> CreateSupplier(int userId, CreateSupplierModel model)
        {
            Supplier supplier = new Supplier()
            {
                Name = model.Name,
                CreatedBy = userId,
            };
            _context.Suppliers.Add(supplier);
            var result = await _context.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }

        public async Task<bool> DeleteSupplier(int supplierId, int userId)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == supplierId);
            if (supplier != null)
            {
                supplier.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ViewSupplierModel>> GetAllSupplier()
        {
            var query = from s in _context.Suppliers
                        select s;

            var result = await query.Select(s => new ViewSupplierModel()
            {
                Id = s.Id,
                Name = s.Name,
                CreatedAt = s.CreatedAt,
                CreatedBy = new ViewModel()
                {
                    Id = s.CreatedByNavigation.Id,
                    Name = s.CreatedByNavigation.FullName
                },
                UpdatedAt = s.UpdatedAt,
                UpdatedBy = s.UpdatedBy,
                IsActive = s.IsActive,
            }).ToListAsync();

            return result;
        }

        public Task<List<ViewProductModel>> GetListProduct(int supplierId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ViewSupplierModel> GetSupplierById(int supplierId)
        {
            var supplier = await _context.Brands.FirstOrDefaultAsync(x => x.Id == supplierId);
            if (supplier != null)
            {
                ViewSupplierModel result = new ViewSupplierModel()
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    IsActive = supplier.IsActive,
                    CreatedAt = supplier.CreatedAt,
                    CreatedBy = new ViewModel()
                    {
                        Id = supplier.CreatedByNavigation.Id,
                        Name = supplier.CreatedByNavigation.FullName
                    },
                    UpdatedAt = supplier.UpdatedAt,
                    UpdatedBy = supplier.UpdatedBy
                };
                return result;
            }
            return null;
        }

        public async Task<bool> UpdateSupplier(int supplierId, int userId, UpdateSupplierModel model)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == supplierId);
            if (supplier != null)
            {
                supplier.Name = model.Name;
                supplier.UpdatedAt = DateTime.Now;
                supplier.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
