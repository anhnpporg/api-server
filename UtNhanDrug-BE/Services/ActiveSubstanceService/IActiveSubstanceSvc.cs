using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ActiveSubstanceModel;

namespace UtNhanDrug_BE.Services.ActiveSubstanceService
{
    public interface IActiveSubstanceSvc
    {
        Task<bool> CreateActiveSubstance(int userId, CreateActiveSubstanceModel model);
        Task<bool> UpdateActiveSubstance(int brandId, int userId, UpdateActiveSubstanceModel model);
        Task<bool> DeleteActiveSubstance(int brandId, int userId);
        Task<ViewActiveSubstanceModel> GetActiveSubstanceById(int brandId);
        Task<List<ViewActiveSubstanceModel>> GetAllActiveSubstance();
        Task<bool> CheckActiveSubstance(int brandId);
    }
}
