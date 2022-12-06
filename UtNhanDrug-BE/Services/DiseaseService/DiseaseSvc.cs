using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.DiseaseModel;
using System.Linq;
using System;
using UtNhanDrug_BE.Models.ResponseModel;
using static UtNhanDrug_BE.Models.DiseaseModel.DiseaseViewModel;
using UtNhanDrug_BE.Hepper;
using UtNhanDrug_BE.Services.ProfileUserService;

namespace UtNhanDrug_BE.Services.DiseaseService
{
    public class DiseaseSvc : IDiseaseSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly IProfileUserSvc _profileUserService;

        private readonly DateTime today = LocalDateTime.DateTimeNow();

        public DiseaseSvc(ut_nhan_drug_store_databaseContext context, IProfileUserSvc profileUserService)
        {
            _context = context;
            _profileUserService = profileUserService;
        }

        public async Task<Response<DiseaseForManager>> GetDiseaseForManager(int diseaseId)
        {
            // Get disease from db
            var diseaseEntity = await _context.Diseases.FirstOrDefaultAsync(disease =>
            disease.Id.Equals(diseaseId)
            && disease.DeletedAt == null
            && disease.DeletedBy == null);

            // Check disease
            if (diseaseEntity == null)
            {
                return new Response<DiseaseForManager>(null)
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy bệnh với mã " + diseaseId.ToString()
                };
            }

            // Map view model
            DiseaseForManager disease = new DiseaseForManager
            {
                Id = diseaseEntity.Id,
                Name = diseaseEntity.Name,
                CreatedAt = diseaseEntity.CreatedAt,
                CreatedByProfile = await _profileUserService.GetProfileUser(diseaseEntity.CreatedBy),
                UpdatedAt = diseaseEntity.UpdatedAt,
                UpdatedByProfile = await _profileUserService.GetProfileUser(diseaseEntity.UpdatedBy)
            };

            // Return view model
            return new Response<DiseaseForManager>(disease);
        }

        public async Task<Response<DiseaseForStaff>> GetDiseaseForStaff(int diseaseId)
        {
            // Get disease from db
            var diseaseEntity = await _context.Diseases.FirstOrDefaultAsync(disease =>
            disease.Id.Equals(diseaseId)
            && disease.DeletedAt == null
            && disease.DeletedBy == null);

            // Check disease
            if (diseaseEntity == null)
            {
                return new Response<DiseaseForStaff>(null)
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy bệnh với mã " + diseaseId.ToString()
                };
            }

            // Map view model
            DiseaseForStaff disease = new DiseaseForStaff
            {
                Id = diseaseEntity.Id,
                Name = diseaseEntity.Name
            };

            // Return view model
            return new Response<DiseaseForStaff>(disease);
        }

        public async Task<Response<List<DiseaseForManager>>> GetDiseasesForManager()
        {
            // Get diseases from db
            var diseasesEntity = await _context.Diseases.Where(disease =>
            disease.DeletedAt == null
            && disease.DeletedBy == null)
                .ToListAsync();

            List<DiseaseForManager> diseases = new List<DiseaseForManager>();

            // Map view model
            foreach (Disease diseaseEntity in diseasesEntity)
            {
                diseases.Add(new DiseaseForManager()
                {
                    Id = diseaseEntity.Id,
                    Name = diseaseEntity.Name,
                    CreatedAt = diseaseEntity.CreatedAt,
                    CreatedByProfile = await _profileUserService.GetProfileUser(diseaseEntity.CreatedBy),
                    UpdatedAt = diseaseEntity.UpdatedAt,
                    UpdatedByProfile = await _profileUserService.GetProfileUser(diseaseEntity.UpdatedBy)
                });
            }

            // Return view model
            return new Response<List<DiseaseForManager>>(diseases);
        }

        public async Task<Response<List<DiseaseForStaff>>> GetDiseasesForStaff()
        {
            // Get diseases from db
            var diseasesEntity = await _context.Diseases.Where(disease =>
            disease.DeletedAt == null
            && disease.DeletedBy == null)
                .ToListAsync(); ;

            List<DiseaseForStaff> diseases = new List<DiseaseForStaff>();

            // Map view model
            foreach (Disease diseaseEntity in diseasesEntity)
            {
                diseases.Add(new DiseaseForStaff()
                {
                    Id = diseaseEntity.Id,
                    Name = diseaseEntity.Name
                });
            }

            // Map view model
            //var diseases = await query.Select(diseaseEntity => new DiseaseForStaff()
            //{
            //    Id = diseaseEntity.Id,
            //    Name = diseaseEntity.Name
            //}).ToListAsync();

            // Return view model
            return new Response<List<DiseaseForStaff>>(diseases);
        }

        public async Task<bool> CheckDiseaseExist(string diseaseName)
        {
            var diseaseEntity = await _context.Diseases.FirstOrDefaultAsync(disease =>
            disease.Name.ToUpper().Equals(diseaseName.ToUpper())
            && disease.DeletedAt == null
            && disease.DeletedBy == null);

            return diseaseEntity != null;
        }

        public async Task<Response<DiseaseForManager>> CreateDisease(DiseaseForCreation newDisease, int userAccountId)
        {
            try
            {
                // Check disease
                if (await CheckDiseaseExist(newDisease.Name))
                {
                    return new Response<DiseaseForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Tên bệnh " + newDisease.Name + " đã tồn tại"
                    };
                }

                // Map new data
                Disease diseaseEntity = new Disease()
                {
                    Name = newDisease.Name,
                    CreatedAt = today,
                    CreatedBy = userAccountId,
                };

                // Add new data
                _context.Diseases.Add(diseaseEntity);

                // Save Change
                await _context.SaveChangesAsync();

                // Map view model
                DiseaseForManager createdDisease = new DiseaseForManager
                {
                    Id = diseaseEntity.Id,
                    Name = diseaseEntity.Name,
                    CreatedAt = diseaseEntity.CreatedAt,
                    CreatedByProfile = await _profileUserService.GetProfileUser(diseaseEntity.CreatedBy),
                    UpdatedAt = diseaseEntity.UpdatedAt,
                    UpdatedByProfile = await _profileUserService.GetProfileUser(diseaseEntity.UpdatedBy)
                };

                // Return view model
                return new Response<DiseaseForManager>(createdDisease)
                {
                    StatusCode = 201,
                    Message = "Tạo bệnh thành công"
                };
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.InnerException.Message;

                int statusCode = 500;
                string message = "Tạo bệnh thất bại. Lỗi hệ thống";
                List<string> errors = new List<string> { exceptionMessage };

                //if (exceptionMessage.Contains("Cannot insert duplicate key"))
                //{
                //    statusCode = 400;
                //    message = "Tên bệnh đã tồn tại";
                //}

                return new Response<DiseaseForManager>(null)
                {
                    Succeeded = false,
                    StatusCode = statusCode,
                    Message = message,
                    Errors = errors.ToArray()
                };
            }
        }

        public async Task<Response<DiseaseForManager>> UpdateDisease(int diseaseId, DiseaseForUpdate newDisease, int userAccountId)
        {
            try
            {
                // Get disease from db
                var diseaseEntity = await _context.Diseases.FirstOrDefaultAsync(disease =>
                disease.Id.Equals(diseaseId)
                && disease.DeletedAt == null
                && disease.DeletedBy == null);

                // Check disease
                if (diseaseEntity == null)
                {
                    return new Response<DiseaseForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy bệnh với mã " + diseaseId.ToString()
                    };
                }
                else if (await CheckDiseaseExist(newDisease.Name))
                {
                    return new Response<DiseaseForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Tên bệnh " + newDisease.Name + " đã tồn tại"
                    };
                }

                // Map new data
                diseaseEntity.Name = newDisease.Name;
                diseaseEntity.UpdatedAt = today;
                diseaseEntity.UpdatedBy = userAccountId;

                // Update new data
                _context.Diseases.Update(diseaseEntity);

                // Save Change
                await _context.SaveChangesAsync();

                // Map view model
                DiseaseForManager updatedDisease = new DiseaseForManager
                {
                    Id = diseaseEntity.Id,
                    Name = diseaseEntity.Name,
                    CreatedAt = diseaseEntity.CreatedAt,
                    CreatedByProfile = await _profileUserService.GetProfileUser(diseaseEntity.CreatedBy),
                    UpdatedAt = diseaseEntity.UpdatedAt,
                    UpdatedByProfile = await _profileUserService.GetProfileUser(diseaseEntity.UpdatedBy)
                };

                // Return view model
                return new Response<DiseaseForManager>(updatedDisease)
                {
                    StatusCode = 200,
                    Message = "Cập nhật bệnh thành công"
                };
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.InnerException.Message;

                int statusCode = 500;
                string message = "Cập nhật bệnh thất bại. Lỗi hệ thống";
                List<string> errors = new List<string> { exceptionMessage };

                //if (exceptionMessage.Contains("Cannot insert duplicate key"))
                //{
                //    statusCode = 400;
                //    message = "Tên bệnh đã tồn tại";
                //}

                return new Response<DiseaseForManager>(null)
                {
                    Succeeded = false,
                    StatusCode = statusCode,
                    Message = message,
                    Errors = errors.ToArray()
                };
            }
        }

        public async Task<Response<bool>> DeleteDisease(int diseaseId, int userAccountId)
        {
            try
            {
                // Get disease from db
                var diseaseEntity = await _context.Diseases.FirstOrDefaultAsync(disease =>
                disease.Id.Equals(diseaseId)
                && disease.DeletedAt == null
                && disease.DeletedBy == null);

                // Check disease
                if (diseaseEntity == null)
                {
                    return new Response<bool>(false)
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy bệnh với mã " + diseaseId.ToString()
                    };
                }

                // Map new data
                diseaseEntity.DeletedAt = today;
                diseaseEntity.DeletedBy = userAccountId;

                // Delete data
                _context.Diseases.Update(diseaseEntity);

                // Get Sample Prescription same Disease
                var samplePrescriptionsEntity = await _context.SamplePrescriptions.Where(samplePrescription =>
                samplePrescription.DiseaseId.Equals(diseaseId)
                && samplePrescription.DeletedAt == null
                && samplePrescription.DeletedBy == null)
                    .ToListAsync();

                // Delete Sample Prescription same Disease
                foreach (SamplePrescription samplePrescriptionEntity in samplePrescriptionsEntity)
                {
                    // Map new data
                    samplePrescriptionEntity.DeletedAt = today;
                    samplePrescriptionEntity.DeletedBy = userAccountId;

                    // Delete data
                    _context.SamplePrescriptions.Update(samplePrescriptionEntity);

                    // Get Sample Prescription Detail same Sample Prescription
                    var samplePrescriptionDetailsEntity = await _context.SamplePrescriptionDetails.Where(samplePrescriptionDetail =>
                    samplePrescriptionDetail.SamplePrescriptionId.Equals(samplePrescriptionEntity.Id)
                    && samplePrescriptionDetail.DeletedAt == null
                    && samplePrescriptionDetail.DeletedBy == null)
                        .ToListAsync();

                    // Delete Sample Prescription Detail same Sample Prescription
                    foreach (SamplePrescriptionDetail samplePrescriptionDetailEntity in samplePrescriptionDetailsEntity)
                    {
                        // Map new data
                        samplePrescriptionDetailEntity.DeletedAt = today;
                        samplePrescriptionDetailEntity.DeletedBy = userAccountId;

                        // Delete data
                        _context.SamplePrescriptionDetails.Update(samplePrescriptionDetailEntity);
                    }
                }

                // Save Change
                await _context.SaveChangesAsync();

                // Return view model
                return new Response<bool>(true)
                {
                    StatusCode = 200,
                    Message = "Xóa bệnh thành công"
                };
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.InnerException.Message;

                int statusCode = 500;
                string message = "Xóa bệnh thất bại. Lỗi hệ thống";
                List<string> errors = new List<string> { exceptionMessage };

                //if (exceptionMessage.Contains("Cannot insert duplicate key"))
                //{
                //    statusCode = 400;
                //    message = "Tên bệnh đã tồn tại";
                //}

                return new Response<bool>(false)
                {
                    Succeeded = false,
                    StatusCode = statusCode,
                    Message = message,
                    Errors = errors.ToArray()
                };
            }
        }
    }
}
