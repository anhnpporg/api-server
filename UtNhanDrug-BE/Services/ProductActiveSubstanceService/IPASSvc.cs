using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ActiveSubstanceModel;
using UtNhanDrug_BE.Models.ProductActiveSubstance;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.ProductActiveSubstanceService
{
    public interface IPASSvc
    {
        Task<Response<bool>> AddPAS(List<CreatePASModel> model);
        Task<Response<bool>> RemovePAS(RemoveActiveSubstanceModel model);
        //Task<bool> UpdatePASByProductId(int ProductId, List<UpdatePASModel> model);
        //Task<List<ViewPASModel>> GetPASById(int productId);
        //Task<List<ViewPASModel>> GetAllPAS();
        //Task<bool> CheckPAS(int id);
    }
}
