using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.CategoryModel;
using System.Linq;

namespace UtNhanDrug_BE.Services.CategoryService
{
    public class CategorySvc : ICategorySvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        public CategorySvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckCategory(int categoryId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);
            if (category != null) return true;
            return false;
        }

        public async Task<bool> CreateCategory(int userId, CreateCategoryModel model)
        {
            Category category = new Category()
            {
                Name = model.Name,
                CreatedBy = userId,
            };
            _context.Categories.Add(category);
            var result = await _context.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }

        public async Task<bool> DeleteCategory(int categoryId, int userId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);
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

        public async Task<List<ViewCategoryModel>> GetAllCategory()
        {
            var query = from c in _context.Categories
                        select c;

            var result = await query.Select(c => new ViewCategoryModel()
            {
                Id = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsActive = c.IsActive,
            }).ToListAsync();

            return result;
        }

        public async Task<ViewCategoryModel> GetCategoryById(int categoryId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);
            if (category != null)
            {
                ViewCategoryModel result = new ViewCategoryModel()
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    CreatedBy = category.CreatedBy,
                    UpdatedAt = category.UpdatedAt,
                    UpdatedBy = category.UpdatedBy
                };
                return result;
            }
            return null;
        }

        public async Task<bool> UpdateCategory(int categoryId, int userId, UpdateCategoryModel model)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);
            if (category != null)
            {
                category.Name = model.Name;
                category.UpdatedAt = DateTime.Now;
                category.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
