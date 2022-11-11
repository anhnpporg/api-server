using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ActiveSubstanceModel;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.ActiveSubstanceService
{
    public interface IActiveSubstanceSvc
    {
        Task<Response<bool>> CreateActiveSubstance(int userId, CreateActiveSubstanceModel model);
        Task<Response<bool>> UpdateActiveSubstance(int brandId, int userId, UpdateActiveSubstanceModel model);
        Task<Response<bool>> DeleteActiveSubstance(int brandId, int userId);
        Task<Response<ViewProductActiveSubstanceModel>> GetActiveSubstanceById(int brandId);
        Task<Response<List<ViewProductActiveSubstanceModel>>> GetAllActiveSubstance();
        //Task<Response<bool>> CheckActiveSubstance(int brandId);
        Task<Response<List<ViewProductModel>>> GetListProducts(int activeSubstanceId);
    }
}
