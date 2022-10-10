using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.DosageUnitModel;

namespace UtNhanDrug_BE.Services.DosageUnitService
{
    public interface IDosageUnitSvc
    {
        Task<bool> CreateDosageUnit(CreateDosageUnitModel model);
        Task<bool> UpdateDosageUnit(int id, UpdateDosageUnitModel model);
        Task<ViewDosageUnitModel> GetDosageUnitById(int id);
        Task<List<ViewDosageUnitModel>> GetAllDosageUnit();
        Task<bool> CheckDosageUnit(int id);
    }
}
