using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ActiveSubstanceModel;
using System.Linq;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Models.ModelHelper;
using Microsoft.EntityFrameworkCore.Storage;

namespace UtNhanDrug_BE.Services.ActiveSubstanceService
{
    public class ActiveSubstanceSvc : IActiveSubstanceSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;

        public ActiveSubstanceSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckActiveSubstance(int id)
        {
            var result = await _context.ActiveSubstances.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null) return true;
            return false;
        }

        public async Task<bool> CreateActiveSubstance(int userId, CreateActiveSubstanceModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                ActiveSubstance a = new ActiveSubstance()
                {
                    Name = model.Name,
                    CreatedBy = userId,
                };
                _context.ActiveSubstances.Add(a);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<bool> DeleteActiveSubstance(int id, int userId)
        {
            var result = await _context.ActiveSubstances.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null)
            {
                result.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<ViewProductActiveSubstanceModel> GetActiveSubstanceById(int id)
        {
            var activeSubstance = await _context.ActiveSubstances.FirstOrDefaultAsync(x => x.Id == id);
            if (activeSubstance != null)
            {
                ViewProductActiveSubstanceModel result = new ViewProductActiveSubstanceModel()
                {
                    Id = activeSubstance.Id,
                    Name = activeSubstance.Name,
                    IsActive = activeSubstance.IsActive,
                    CreatedAt = activeSubstance.CreatedAt,
                    CreatedBy = activeSubstance.CreatedBy,
                    UpdatedAt = activeSubstance.UpdatedAt,
                    UpdatedBy = activeSubstance.UpdatedBy
                };
                return result;
            }
            return null;
        }

        public async Task<List<ViewProductActiveSubstanceModel>> GetAllActiveSubstance()
        {
            var query = from b in _context.ActiveSubstances
                        select b;

            var result = await query.Select(a => new ViewProductActiveSubstanceModel()
            {
                Id = a.Id,
                Name = a.Name,
                CreatedAt = a.CreatedAt,
                CreatedBy = a.CreatedBy,
                UpdatedAt = a.UpdatedAt,
                UpdatedBy = a.UpdatedBy,
                IsActive = a.IsActive,
            }).ToListAsync();

            return result;
        }

        public async Task<List<ViewProductModel>> GetListProducts(int activeSubstanceId)
        {
            var query = from a in _context.ProductActiveSubstances
                        where a.ActiveSubstanceId == activeSubstanceId
                        select a;
            var data = await query.Select(p => new ViewProductModel()
            {
                Id = p.Product.Id,
                DrugRegistrationNumber = p.Product.DrugRegistrationNumber,
                Barcode = p.Product.Barcode,
                Name = p.Product.Name,
                Brand = new ViewModel()
                {
                    Id = p.Product.Brand.Id,
                    Name = p.Product.Brand.Name
                },
                Shelf = new ViewModel()
                {
                    Id = p.Product.Shelf.Id,
                    Name = p.Product.Shelf.Name
                },
                MininumInventory = p.Product.MininumInventory,
                RouteOfAdministration = new ViewModel()
                {
                    Id = p.Product.RouteOfAdministration.Id,
                    Name = p.Product.RouteOfAdministration.Name
                },
                IsUseDose = p.Product.IsUseDose,
                IsManagedInBatches = p.Product.IsManagedInBatches,
                CreatedAt = p.Product.CreatedAt,
                CreatedBy = new ViewModel()
                {
                    Id = p.Product.CreatedByNavigation.Id,
                    Name = p.Product.CreatedByNavigation.FullName
                },
                UpdatedAt = p.Product.UpdatedAt,
                UpdatedBy = p.Product.UpdatedBy,
                IsActive = p.Product.IsActive,
            }).ToListAsync();

            return data;
        }

        public async Task<bool> UpdateActiveSubstance(int id, int userId, UpdateActiveSubstanceModel model)
        {
            var result = await _context.ActiveSubstances.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null)
            {
                result.Name = model.Name;
                result.UpdatedAt = DateTime.Now;
                result.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
