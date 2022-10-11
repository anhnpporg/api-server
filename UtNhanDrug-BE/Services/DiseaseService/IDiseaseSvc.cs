using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.DiseaseModel;

namespace UtNhanDrug_BE.Services.DiseaseService
{
    public interface IDiseaseSvc
    {
        Task<bool> CreateDisease(int userId, CreateDiseaseModel model);
        Task<bool> UpdateDisease(int id, int userId, UpdateDiseaseModel model);
        Task<bool> DeleteDisease(int id, int userId);
        Task<ViewDiseaseModel> GetDiseaseById(int id);
        Task<List<ViewDiseaseModel>> GetAllDisease();
        Task<bool> CheckDisease(int id);
    }
}
