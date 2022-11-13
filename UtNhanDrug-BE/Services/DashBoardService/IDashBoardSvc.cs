using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.DashBoardModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.DashBoardService
{
    public interface IDashBoardSvc
    {
        Task<Response<List<TopSellingModel>>> GetTopSelling(SearchModel request);
        Task<Response<List<RecentSalesModel>>> GetRecentSales(SearchModel request);
    }
}
