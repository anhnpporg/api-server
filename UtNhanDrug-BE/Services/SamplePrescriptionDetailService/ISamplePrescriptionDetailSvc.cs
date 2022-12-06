using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ResponseModel;
using UtNhanDrug_BE.Models.SamplePrescriptionDetailModel;
using static UtNhanDrug_BE.Models.SamplePrescriptionDetailModel.SamplePrescriptionDetailViewModel;

namespace UtNhanDrug_BE.Services.SamplePrescriptionDetailService
{
    public interface ISamplePrescriptionDetailSvc
    {
        Task<SamplePrescriptionDetailForManager> MapSamplePrescriptionDetail(SamplePrescriptionDetail samplePrescriptionDetailEntity);

        Task<Response<List<SamplePrescriptionDetailForManager>>> GetSamplePrescriptionDetailsForManager(int samplePrescriptionId);
        Task<Response<List<SamplePrescriptionDetailForStaff>>> GetSamplePrescriptionDetailsForStaff(int samplePrescriptionId, SamplePrescriptionDetailFilter filter);

        Task<Response<SamplePrescriptionDetailForManager>> CreateSamplePrescriptionDetail(SamplePrescriptionDetailForCreation newSamplePrescriptionDetail, int userAccountId);
        Task<Response<SamplePrescriptionDetailForManager>> UpdateSamplePrescriptionDetail(int samplePrescriptionDetailId, SamplePrescriptionDetailForUpdate newSamplePrescriptionDetail, int userAccountId);
        Task<Response<bool>> DeleteSamplePrescriptionDetail(int samplePrescriptionDetailId, int userAccountId);
    }
}
