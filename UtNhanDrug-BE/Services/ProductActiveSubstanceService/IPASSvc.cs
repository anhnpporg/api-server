using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ProductActiveSubstance;

namespace UtNhanDrug_BE.Services.ProductActiveSubstanceService
{
    public interface IPASSvc
    {
        Task<bool> CreatePAS(CreatePASModel model);
        Task<bool> UpdatePAS(int id, UpdatePASModel model);
        Task<List<ViewPASModel>> GetPASById(int productId);
        Task<List<ViewPASModel>> GetAllPAS();
        Task<bool> CheckPAS(int id);
    }
}
