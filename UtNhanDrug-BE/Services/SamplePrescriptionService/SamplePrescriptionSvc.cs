using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.SamplePrescriptionModel;
using System.Linq;
using System;
using UtNhanDrug_BE.Models.ModelHelper;

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
            var sp = await _context.SamplePrescriptions.FirstOrDefaultAsync(x => x.Id == id);
            if (sp != null) return true;
            return false;
        }

        public async Task<bool> CreateSamplePrescription(int userId, CreateSamplePrescriptionModel model)
        {
            SamplePrescription sp = new SamplePrescription()
            {
                DiseaseId = model.DiseaseId,
                Name = model.Name,
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
                Disease = new ViewModel()
                {
                    Id = b.Disease.Id,
                    Name = b.Disease.Name
                },
                Name = b.Name,
                CreatedAt = b.CreatedAt,
                CreatedBy = new ViewModel()
                {
                    Id = b.CreatedByNavigation.Id,
                    Name = b.CreatedByNavigation.FullName
                },
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
                    Disease = new ViewModel()
                    {
                        Id = sp.Disease.Id,
                        Name = sp.Disease.Name
                    },
                    Name = sp.Name,
                    IsActive = sp.IsActive,
                    CreatedAt = sp.CreatedAt,
                    CreatedBy = new ViewModel()
                    {
                        Id = sp.CreatedByNavigation.Id,
                        Name = sp.CreatedByNavigation.FullName
                    },
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
                sp.Name = model.Name;
                sp.IsActive = model.IsActive;
                sp.UpdatedAt = DateTime.Now;
                sp.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
