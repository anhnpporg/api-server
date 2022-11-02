using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ShelfModel;
using System.Linq;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Services.ShelfService
{
    public class ShelfSvc : IShelfSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        public ShelfSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckShelf(int shelfId)
        {
            var shelf = await _context.Shelves.FirstOrDefaultAsync(x => x.Id == shelfId);
            if (shelf != null) return true;
            return false;
        }

        public async Task<bool> CreateShelf(int userId, CreateShelfModel model)
        {
            Shelf shelf = new Shelf()
            {
                Name = model.Name,
                CreatedBy = userId,
            };
            _context.Shelves.Add(shelf);
            var result = await _context.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }

        public async Task<bool> DeleteShelf(int shelfId, int userId)
        {
            var category = await _context.Shelves.FirstOrDefaultAsync(x => x.Id == shelfId);
            if (category != null)
            {
                category.UpdatedAt = DateTime.Now;
                category.UpdatedBy = userId;
                category.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ViewShelfModel>> GetAllShelves()
        {
            var query = from c in _context.Shelves
                        select c;

            var result = await query.Select(c => new ViewShelfModel()
            {
                Id = c.Id,
                Name = c.Name,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                CreatedBy = new ViewModel()
                {
                    Id = c.CreatedByNavigation.Id,
                    Name = c.CreatedByNavigation.FullName
                },
                UpdatedAt = c.UpdatedAt,
            }).ToListAsync();

            return result;
        }

        public async Task<ViewShelfModel> GetShelfById(int shelfId)
        {
            var shelf = await _context.Shelves.FirstOrDefaultAsync(x => x.Id == shelfId);
            if (shelf != null)
            {
                ViewShelfModel result = new ViewShelfModel()
                {
                    Id = shelf.Id,
                    Name = shelf.Name,
                    IsActive = shelf.IsActive,
                    CreatedAt = shelf.CreatedAt,
                    CreatedBy = new ViewModel()
                    {
                        Id = shelf.CreatedByNavigation.Id,
                        Name = shelf.CreatedByNavigation.FullName
                    },
                };
                return result;
            }
            return null;
        }

        public async Task<List<ViewProductModel>> GetListProduct(int categoryId)
        {
            var query = from p in _context.Products
                        where p.ShelfId == categoryId
                        select p;

            var data = await query.Select(p => new ViewProductModel()
            {
                Id = p.Id,
                DrugRegistrationNumber = p.DrugRegistrationNumber,
                Barcode = p.Barcode,
                Name = p.Name,
                Brand = new ViewModel()
                {
                    Id = p.Brand.Id,
                    Name = p.Brand.Name
                },
                Shelf = new ViewModel()
                {
                    Id = p.Shelf.Id,
                    Name = p.Shelf.Name
                },
                MininumInventory = p.MininumInventory,
                RouteOfAdministration = new ViewModel()
                {
                    Id = p.RouteOfAdministration.Id,
                    Name = p.RouteOfAdministration.Name
                },
                IsUseDose = p.IsUseDose,
                IsManagedInBatches = p.IsManagedInBatches,
                CreatedAt = p.CreatedAt,
                CreatedBy = new ViewModel()
                {
                    Id = p.CreatedByNavigation.Id,
                    Name = p.CreatedByNavigation.FullName
                },
                UpdatedAt = p.UpdatedAt,
                UpdatedBy = p.UpdatedBy,
                IsActive = p.IsActive,
            }).ToListAsync();

            return data;
        }

        public async Task<bool> UpdateShelf(int categoryId, int userId, UpdateShelfModel model)
        {
            var shelf = await _context.Shelves.FirstOrDefaultAsync(x => x.Id == categoryId);
            if (shelf != null)
            {
                shelf.Name = model.Name;
                shelf.UpdatedAt = DateTime.Now;
                shelf.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
