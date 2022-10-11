using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.DiseaseModel;
using System.Linq;
using System;

namespace UtNhanDrug_BE.Services.DiseaseService
{
    public class DiseaseSvc : IDiseaseSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;

        public DiseaseSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckDisease(int id)
        {
            var result = await _context.Diseases.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null) return true;
            return false;
        }

        public async Task<bool> CreateDisease(int userId, CreateDiseaseModel model)
        {
            Disease disease = new Disease()
            {
                Name = model.Name,
                CreatedBy = userId,
            };
            _context.Diseases.Add(disease);
            var result = await _context.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }

        public async Task<bool> DeleteDisease(int id, int userId)
        {
            var result = await _context.Diseases.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null)
            {
                result.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ViewDiseaseModel>> GetAllDisease()
        {
            var query = from b in _context.Diseases
                        select b;

            var result = await query.Select(b => new ViewDiseaseModel()
            {
                Id = b.Id,
                Name = b.Name,
                CreatedAt = b.CreatedAt,
                CreatedBy = b.CreatedBy,
                UpdatedAt = b.UpdatedAt,
                UpdatedBy = b.UpdatedBy,
                IsActive = b.IsActive,
            }).ToListAsync();

            return result;
        }

        public async Task<ViewDiseaseModel> GetDiseaseById(int id)
        {
            var disease = await _context.Diseases.FirstOrDefaultAsync(x => x.Id == id);
            if (disease != null)
            {
                ViewDiseaseModel result = new ViewDiseaseModel()
                {
                    Id = disease.Id,
                    Name = disease.Name,
                    IsActive = disease.IsActive,
                    CreatedAt = disease.CreatedAt,
                    CreatedBy = disease.CreatedBy,
                    UpdatedAt = disease.UpdatedAt,
                    UpdatedBy = disease.UpdatedBy
                };
                return result;
            }
            return null;
        }

        public async Task<bool> UpdateDisease(int id, int userId, UpdateDiseaseModel model)
        {
            var disease = await _context.Diseases.FirstOrDefaultAsync(x => x.Id == id);
            if (disease != null)
            {
                disease.Name = model.Name;
                disease.UpdatedAt = DateTime.Now;
                disease.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
