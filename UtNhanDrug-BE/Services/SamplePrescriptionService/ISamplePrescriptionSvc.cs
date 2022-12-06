using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ResponseModel;
using UtNhanDrug_BE.Models.SamplePrescriptionModel;
using static UtNhanDrug_BE.Models.SamplePrescriptionModel.SamplePrescriptionViewModel;

namespace UtNhanDrug_BE.Services.SamplePrescriptionService
{
    public interface ISamplePrescriptionSvc
    {
        Task<Response<SamplePrescriptionForManager>> GetSamplePrescriptionForManager(int samplePrescriptionId);
        Task<Response<SamplePrescriptionForStaff>> GetSamplePrescriptionForStaff(int samplePrescriptionId);

        Task<Response<List<SamplePrescriptionForManager>>> GetSamplePrescriptionsForManager(int diseaseId);
        Task<Response<List<SamplePrescriptionForStaff>>> GetSamplePrescriptionsForStaff(int diseaseId);

        Task<Response<SamplePrescriptionForManager>> CreateSamplePrescription(SamplePrescriptionForCreation newSamplePrescription, int userAccountId);
        Task<Response<SamplePrescriptionForManager>> UpdateSamplePrescription(int samplePrescriptionId, SamplePrescriptionForUpdate newSamplePrescription, int userAccountId);
        Task<Response<bool>> DeleteSamplePrescription(int samplePrescriptionId, int userAccountId);
    }
}
