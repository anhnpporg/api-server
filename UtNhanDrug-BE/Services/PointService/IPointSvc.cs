using System.Threading.Tasks;
using UtNhanDrug_BE.Models.PointModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.PointService
{
    public interface IPointSvc
    {
        Task<Response<PointViewModel>> GetPointManager();
        Task<Response<bool>> UpdateManagePoint(UpdatePointRequest request);
    }
}
