using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.DiseaseModel;
using UtNhanDrug_BE.Models.ResponseModel;
using static UtNhanDrug_BE.Models.DiseaseModel.DiseaseViewModel;

namespace UtNhanDrug_BE.Services.DiseaseService
{
    public interface IDiseaseSvc
    {
        Task<Response<DiseaseForManager>> GetDiseaseForManager(int diseaseId);
        Task<Response<DiseaseForStaff>> GetDiseaseForStaff(int diseaseId);

        Task<Response<List<DiseaseForManager>>> GetDiseasesForManager();
        Task<Response<List<DiseaseForStaff>>> GetDiseasesForStaff();

        Task<Response<DiseaseForManager>> CreateDisease(DiseaseForCreation newDisease, int userAccountId);
        Task<Response<DiseaseForManager>> UpdateDisease(int diseaseId, DiseaseForUpdate newDisease, int userAccountId);
        Task<Response<bool>> DeleteDisease(int diseaseId, int userAccountId);
    }
}
