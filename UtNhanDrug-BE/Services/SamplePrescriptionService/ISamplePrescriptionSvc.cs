using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.SamplePrescriptionModel;

namespace UtNhanDrug_BE.Services.SamplePrescriptionService
{
    public interface ISamplePrescriptionSvc
    {
        Task<bool> CreateSamplePrescription(int userId, CreateSamplePrescriptionModel model);
        Task<bool> UpdateSamplePrescription(int id, int userId, UpdateSamplePrescriptionModel model);
        Task<bool> DeleteSamplePrescription(int id, int userId);
        Task<ViewSamplePrescriptionModel> GetSamplePrescriptionById(int id);
        Task<List<ViewSamplePrescriptionModel>> GetAllSamplePrescription();
        Task<bool> CheckSamplePrescription(int id);
    }
}
