using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.SamplePrescriptionModel;
using System.Linq;
using System;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Services.ProfileUserService;
using UtNhanDrug_BE.Hepper;
using UtNhanDrug_BE.Models.ResponseModel;
using static UtNhanDrug_BE.Models.SamplePrescriptionModel.SamplePrescriptionViewModel;
using UtNhanDrug_BE.Services.SamplePrescriptionDetailService;

namespace UtNhanDrug_BE.Services.SamplePrescriptionService
{
    public class SamplePrescriptionSvc : ISamplePrescriptionSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly IProfileUserSvc _profileUserService;
        private readonly ISamplePrescriptionDetailSvc _samplePrescriptionDetail;

        private readonly DateTime today = LocalDateTime.DateTimeNow();

        public SamplePrescriptionSvc(ut_nhan_drug_store_databaseContext context, IProfileUserSvc profileUserService, ISamplePrescriptionDetailSvc samplePrescriptionDetail)
        {
            _context = context;
            _profileUserService = profileUserService;
            _samplePrescriptionDetail = samplePrescriptionDetail;
        }

        public async Task<Response<SamplePrescriptionForManager>> GetSamplePrescriptionForManager(int samplePrescriptionId)
        {
            // Get samplePrescription from db
            var samplePrescriptionEntity = await _context.SamplePrescriptions.FirstOrDefaultAsync(samplePrescription =>
            samplePrescription.Id.Equals(samplePrescriptionId)
            && samplePrescription.DeletedAt == null
            && samplePrescription.DeletedBy == null);

            // Check samplePrescription
            if (samplePrescriptionEntity == null)
            {
                return new Response<SamplePrescriptionForManager>(null)
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy đơn mẫu với mã " + samplePrescriptionId.ToString()
                };
            }

            // Map view model
            SamplePrescriptionForManager samplePrescription = new SamplePrescriptionForManager
            {
                Id = samplePrescriptionEntity.Id,
                Name = samplePrescriptionEntity.Name,
                CreatedAt = samplePrescriptionEntity.CreatedAt,
                CreatedByProfile = await _profileUserService.GetProfileUser(samplePrescriptionEntity.CreatedBy),
                UpdatedAt = samplePrescriptionEntity.UpdatedAt,
                UpdatedByProfile = await _profileUserService.GetProfileUser(samplePrescriptionEntity.UpdatedBy)
            };

            // Return view model
            return new Response<SamplePrescriptionForManager>(samplePrescription);
        }

        public async Task<Response<SamplePrescriptionForStaff>> GetSamplePrescriptionForStaff(int samplePrescriptionId)
        {
            // Get samplePrescription from db
            var samplePrescriptionEntity = await _context.SamplePrescriptions.FirstOrDefaultAsync(samplePrescription =>
            samplePrescription.Id.Equals(samplePrescriptionId)
            && samplePrescription.DeletedAt == null
            && samplePrescription.DeletedBy == null);

            // Check samplePrescription
            if (samplePrescriptionEntity == null)
            {
                return new Response<SamplePrescriptionForStaff>(null)
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy đơn mẫu với mã " + samplePrescriptionId.ToString()
                };
            }

            // Map view model
            SamplePrescriptionForStaff samplePrescription = new SamplePrescriptionForStaff
            {
                Id = samplePrescriptionEntity.Id,
                Name = samplePrescriptionEntity.Name
            };

            // Return view model
            return new Response<SamplePrescriptionForStaff>(samplePrescription);
        }

        public async Task<Response<List<SamplePrescriptionForManager>>> GetSamplePrescriptionsForManager(int diseaseId)
        {
            // Get samplePrescriptions from db
            var samplePrescriptionsEntity = await _context.SamplePrescriptions.Where(samplePrescription =>
            samplePrescription.DiseaseId.Equals(diseaseId)
            && samplePrescription.DeletedAt == null
            && samplePrescription.DeletedBy == null)
                .ToListAsync();

            List<SamplePrescriptionForManager> samplePrescriptions = new List<SamplePrescriptionForManager>();

            // Map view model
            foreach (SamplePrescription samplePrescriptionEntity in samplePrescriptionsEntity)
            {
                samplePrescriptions.Add(new SamplePrescriptionForManager
                {
                    Id = samplePrescriptionEntity.Id,
                    Name = samplePrescriptionEntity.Name,
                    CreatedAt = samplePrescriptionEntity.CreatedAt,
                    CreatedByProfile = await _profileUserService.GetProfileUser(samplePrescriptionEntity.CreatedBy),
                    UpdatedAt = samplePrescriptionEntity.UpdatedAt,
                    UpdatedByProfile = await _profileUserService.GetProfileUser(samplePrescriptionEntity.UpdatedBy)
                });
            }

            // Return view model
            return new Response<List<SamplePrescriptionForManager>>(samplePrescriptions);
        }

        public async Task<Response<List<SamplePrescriptionForStaff>>> GetSamplePrescriptionsForStaff(int diseaseId)
        {
            // Get samplePrescriptions from db
            var samplePrescriptionsEntity = await _context.SamplePrescriptions.Where(samplePrescription =>
            samplePrescription.DiseaseId.Equals(diseaseId)
            && samplePrescription.DeletedAt == null
            && samplePrescription.DeletedBy == null)
                .ToListAsync();

            List<SamplePrescriptionForStaff> samplePrescriptions = new List<SamplePrescriptionForStaff>();

            // Map view model
            foreach (SamplePrescription samplePrescriptionEntity in samplePrescriptionsEntity)
            {
                // Check samplePrescription
                bool isSamplePrescriptionDetailsError = false;

                // Get samplePrescriptionDetails from db
                var samplePrescriptionDetailsEntity = await _context.SamplePrescriptionDetails.Where(samplePrescriptionDetail =>
                samplePrescriptionDetail.SamplePrescriptionId.Equals(samplePrescriptionEntity.Id)
                && samplePrescriptionDetail.DeletedAt == null
                && samplePrescriptionDetail.DeletedBy == null)
                    .ToListAsync();

                // Check samplePrescriptionDetails
                foreach (SamplePrescriptionDetail samplePrescriptionDetailEntity in samplePrescriptionDetailsEntity)
                {
                    // Check samplePrescriptionDetail
                    var samplePrescriptionDetail = await _samplePrescriptionDetail.MapSamplePrescriptionDetail(samplePrescriptionDetailEntity);

                    if (samplePrescriptionDetail.ErrorProduct != ""
                        || samplePrescriptionDetail.ErrorBrand != ""
                        || samplePrescriptionDetail.ErrorActiveSubstance != ""
                        || samplePrescriptionDetail.ErrorSupplier != ""
                        || samplePrescriptionDetail.ErrorProductUnitPrice != "")
                    {
                        isSamplePrescriptionDetailsError = true;
                        break;
                    }
                }

                if (!isSamplePrescriptionDetailsError)
                {
                    samplePrescriptions.Add(new SamplePrescriptionForStaff
                    {
                        Id = samplePrescriptionEntity.Id,
                        Name = samplePrescriptionEntity.Name
                    });
                }
            }

            // Return view model
            return new Response<List<SamplePrescriptionForStaff>>(samplePrescriptions);
        }

        public async Task<bool> CheckSamplePrescriptionExist(int diseaseId, string samplePrescriptionName)
        {
            // Get samplePrescriptions from db
            var samplePrescriptionEntity = await _context.SamplePrescriptions.FirstOrDefaultAsync(samplePrescription =>
            samplePrescription.DiseaseId.Equals(diseaseId)
            && samplePrescription.Name.ToUpper().Equals(samplePrescriptionName.ToUpper())
            && samplePrescription.DeletedAt == null
            && samplePrescription.DeletedBy == null);

            return samplePrescriptionEntity != null;
        }

        public async Task<Response<SamplePrescriptionForManager>> CreateSamplePrescription(SamplePrescriptionForCreation newSamplePrescription, int userAccountId)
        {
            try
            {
                // Get disease from db
                var diseaseEntity = await _context.Diseases.FirstOrDefaultAsync(disease =>
                disease.Id.Equals(newSamplePrescription.DiseaseId)
                && disease.DeletedAt == null
                && disease.DeletedBy == null);

                // Check disease
                if (diseaseEntity == null)
                {
                    return new Response<SamplePrescriptionForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy bệnh với mã " + newSamplePrescription.DiseaseId.ToString()
                    };
                }

                // Check samplePrescription
                if (await CheckSamplePrescriptionExist(newSamplePrescription.DiseaseId, newSamplePrescription.Name))
                {
                    return new Response<SamplePrescriptionForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Tên đơn mẫu " + newSamplePrescription.Name + " đã tồn tại ở bệnh " + diseaseEntity.Name
                    };
                }

                // Map new data
                SamplePrescription samplePrescriptionEntity = new SamplePrescription()
                {
                    DiseaseId = newSamplePrescription.DiseaseId,
                    Name = newSamplePrescription.Name,
                    CreatedAt = today,
                    CreatedBy = userAccountId,
                };

                // Add new data
                _context.SamplePrescriptions.Add(samplePrescriptionEntity);

                // Save Change
                await _context.SaveChangesAsync();

                // Map view model
                SamplePrescriptionForManager createdSamplePrescription = new SamplePrescriptionForManager
                {
                    Id = samplePrescriptionEntity.Id,
                    Name = samplePrescriptionEntity.Name,
                    CreatedAt = samplePrescriptionEntity.CreatedAt,
                    CreatedByProfile = await _profileUserService.GetProfileUser(samplePrescriptionEntity.CreatedBy),
                    UpdatedAt = samplePrescriptionEntity.UpdatedAt,
                    UpdatedByProfile = await _profileUserService.GetProfileUser(samplePrescriptionEntity.UpdatedBy)
                };

                // Return view model
                return new Response<SamplePrescriptionForManager>(createdSamplePrescription)
                {
                    StatusCode = 201,
                    Message = "Tạo đơn mẫu thành công"
                };
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.InnerException.Message;

                int statusCode = 500;
                string message = "Tạo đơn mẫu thất bại. Lỗi hệ thống";
                List<string> errors = new List<string> { exceptionMessage };

                //if (exceptionMessage.Contains("Cannot insert duplicate key"))
                //{
                //    statusCode = 400;
                //    message = "Tên bệnh đã tồn tại";
                //}

                return new Response<SamplePrescriptionForManager>(null)
                {
                    Succeeded = false,
                    StatusCode = statusCode,
                    Message = message,
                    Errors = errors.ToArray()
                };
            }
        }

        public async Task<Response<SamplePrescriptionForManager>> UpdateSamplePrescription(int samplePrescriptionId, SamplePrescriptionForUpdate newSamplePrescription, int userAccountId)
        {
            try
            {
                // Get samplePrescriptions from db
                var samplePrescriptionEntity = await _context.SamplePrescriptions.FirstOrDefaultAsync(samplePrescription =>
                samplePrescription.Id.Equals(samplePrescriptionId)
                && samplePrescription.DeletedAt == null
                && samplePrescription.DeletedBy == null);

                // Check samplePrescription
                if (samplePrescriptionEntity == null)
                {
                    return new Response<SamplePrescriptionForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy đơn mẫu với mã " + samplePrescriptionId.ToString()
                    };
                }
                else if (await CheckSamplePrescriptionExist(samplePrescriptionEntity.DiseaseId, newSamplePrescription.Name))
                {
                    // Get disease from db
                    var diseaseEntity = await _context.Diseases.FirstOrDefaultAsync(disease =>
                    disease.Id.Equals(samplePrescriptionEntity.DiseaseId)
                    && disease.DeletedAt == null
                    && disease.DeletedBy == null);

                    return new Response<SamplePrescriptionForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Tên đơn mẫu " + newSamplePrescription.Name + " đã tồn tại ở bệnh " + diseaseEntity.Name
                    };
                }

                // Map new data
                samplePrescriptionEntity.Name = newSamplePrescription.Name;
                samplePrescriptionEntity.UpdatedAt = today;
                samplePrescriptionEntity.UpdatedBy = userAccountId;

                // Update new data
                _context.SamplePrescriptions.Update(samplePrescriptionEntity);

                // Save Change
                await _context.SaveChangesAsync();

                // Map view model
                SamplePrescriptionForManager updatedSamplePrescription = new SamplePrescriptionForManager
                {
                    Id = samplePrescriptionEntity.Id,
                    Name = samplePrescriptionEntity.Name,
                    CreatedAt = samplePrescriptionEntity.CreatedAt,
                    CreatedByProfile = await _profileUserService.GetProfileUser(samplePrescriptionEntity.CreatedBy),
                    UpdatedAt = samplePrescriptionEntity.UpdatedAt,
                    UpdatedByProfile = await _profileUserService.GetProfileUser(samplePrescriptionEntity.UpdatedBy)
                };

                // Return view model
                return new Response<SamplePrescriptionForManager>(updatedSamplePrescription)
                {
                    StatusCode = 200,
                    Message = "Cập nhật đơn mẫu thành công"
                };
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.InnerException.Message;

                int statusCode = 500;
                string message = "Cập nhật đơn mẫu thất bại. Lỗi hệ thống";
                List<string> errors = new List<string> { exceptionMessage };

                //if (exceptionMessage.Contains("Cannot insert duplicate key"))
                //{
                //    statusCode = 400;
                //    message = "Tên bệnh đã tồn tại";
                //}

                return new Response<SamplePrescriptionForManager>(null)
                {
                    Succeeded = false,
                    StatusCode = statusCode,
                    Message = message,
                    Errors = errors.ToArray()
                };
            }
        }

        public async Task<Response<bool>> DeleteSamplePrescription(int samplePrescriptionId, int userAccountId)
        {
            try
            {
                // Get samplePrescriptions from db
                var samplePrescriptionEntity = await _context.SamplePrescriptions.FirstOrDefaultAsync(samplePrescription =>
                samplePrescription.Id.Equals(samplePrescriptionId)
                && samplePrescription.DeletedAt == null
                && samplePrescription.DeletedBy == null);

                // Check samplePrescription
                if (samplePrescriptionEntity == null)
                {
                    return new Response<bool>(false)
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy đơn mẫu với mã " + samplePrescriptionId.ToString()
                    };
                }

                // Map new data
                samplePrescriptionEntity.DeletedAt = today;
                samplePrescriptionEntity.DeletedBy = userAccountId;

                // Delete data
                _context.SamplePrescriptions.Update(samplePrescriptionEntity);

                // Get Sample Prescription Detail same Sample Prescription
                var samplePrescriptionDetailsEntity = await _context.SamplePrescriptionDetails.Where(samplePrescriptionDetail =>
                samplePrescriptionDetail.SamplePrescriptionId.Equals(samplePrescriptionId)
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

                // Save Change
                await _context.SaveChangesAsync();

                // Return view model
                return new Response<bool>(true)
                {
                    StatusCode = 200,
                    Message = "Xóa đơn mẫu thành công"
                };
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.InnerException.Message;

                int statusCode = 500;
                string message = "Xóa đơn mẫu thất bại. Lỗi hệ thống";
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
