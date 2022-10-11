using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.SamplePrescriptionModel;
using System.Linq;
using System;

namespace UtNhanDrug_BE.Services.SamplePrescriptionService
{
    public class SamplePrescriptionSvc : ISamplePrescriptionSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;

        public SamplePrescriptionSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckSamplePrescription(int id)
        {
            var brand = await _context.SamplePrescriptions.FirstOrDefaultAsync(x => x.Id == id);
            if (brand != null) return true;
            return false;
        }

        public async Task<bool> CreateSamplePrescription(int userId, CreateSamplePrescriptionModel model)
        {
            SamplePrescription sp = new SamplePrescription()
            {
                DiseaseId = model.DiseaseId,
                CustomerWeight = model.CustomerWeight,
                CreatedBy = userId,
            };
            _context.SamplePrescriptions.Add(sp);
            var result = await _context.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }

        public async Task<bool> DeleteSamplePrescription(int id, int userId)
        {
            var sp = await _context.SamplePrescriptions.FirstOrDefaultAsync(x => x.Id ==id);
            if (sp != null)
            {
                sp.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ViewSamplePrescriptionModel>> GetAllSamplePrescription()
        {
            var query = from b in _context.SamplePrescriptions
                        select b;

            var result = await query.Select(b => new ViewSamplePrescriptionModel()
            {
                Id = b.Id,
                DiseaseId = b.DiseaseId,
                CustomerWeight = b.CustomerWeight,
                CreatedAt = b.CreatedAt,
                CreatedBy = b.CreatedBy,
                UpdatedAt = b.UpdatedAt,
                UpdatedBy = b.UpdatedBy,
                IsActive = b.IsActive,
            }).ToListAsync();

            return result;
        }

        public async Task<ViewSamplePrescriptionModel> GetSamplePrescriptionById(int id)
        {
            var sp = await _context.SamplePrescriptions.FirstOrDefaultAsync(x => x.Id == id);
            if (sp != null)
            {
                ViewSamplePrescriptionModel result = new ViewSamplePrescriptionModel()
                {
                    Id = sp.Id,
                    DiseaseId = sp.DiseaseId,
                    CustomerWeight = sp.CustomerWeight,
                    IsActive = sp.IsActive,
                    CreatedAt = sp.CreatedAt,
                    CreatedBy = sp.CreatedBy,
                    UpdatedAt = sp.UpdatedAt,
                    UpdatedBy = sp.UpdatedBy
                };
                return result;
            }
            return null;
        }

        public async Task<bool> UpdateSamplePrescription(int id, int userId, UpdateSamplePrescriptionModel model)
        {
            var sp = await _context.SamplePrescriptions.FirstOrDefaultAsync(x => x.Id == id);
            if (sp != null)
            {
                sp.DiseaseId = model.DiseaseId;
                sp.CustomerWeight = model.CustomerWeight;
                sp.UpdatedAt = DateTime.Now;
                sp.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
