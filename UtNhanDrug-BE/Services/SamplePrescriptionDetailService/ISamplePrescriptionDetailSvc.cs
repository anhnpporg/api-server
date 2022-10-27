using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.SamplePrescriptionDetailModel;

namespace UtNhanDrug_BE.Services.SamplePrescriptionDetailService
{
    public interface ISamplePrescriptionDetailSvc
    {
        Task<bool> CreateSamplePrescriptionDetail(int userId, CreateSPDModel model);
        Task<bool> UpdateSamplePrescriptionDetail(int id, int userId, UpdateSPDModel model);
        Task<bool> DeleteSamplePrescriptionDetail(int id, int userId);
        Task<ViewSPDModel> GetSamplePrescriptionDetailById(int id);
        Task<List<ViewSPDModel>> GetAllSamplePrescriptionDetail();
        Task<bool> CheckSamplePrescriptionDetail(int id);
    }
}
