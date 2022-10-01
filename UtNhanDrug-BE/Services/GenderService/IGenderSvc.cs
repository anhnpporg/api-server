using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.GenderModel;

namespace UtNhanDrug_BE.Services.GenderService
{
    public interface IGenderSvc
    {
        Task<List<GenderViewModel>> GetGender();
        Task<GenderViewModel> GetGender(int id);
    }
}
