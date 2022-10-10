using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.UnitModel;

namespace UtNhanDrug_BE.Services.UnitService
{
    public interface IUnitSvc
    {
        Task<bool> CreateUnit(CreateUnitModel model);
        Task<bool> UpdateUnit(int id, UpdateUnitModel model);
        Task<ViewUnitModel> GetUnitById(int id);
        Task<List<ViewUnitModel>> GetAllUnit();
        Task<bool> CheckUnit(int id);
    }
}
