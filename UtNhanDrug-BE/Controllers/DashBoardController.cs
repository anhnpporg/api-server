using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.DashBoardModel;
using UtNhanDrug_BE.Services.DashBoardService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/dashboard")]
    public class DashBoardController : ControllerBase
    {
        private readonly IDashBoardSvc _dashboardSvc;
        public DashBoardController(IDashBoardSvc dashboardSvc)
        {
            _dashboardSvc = dashboardSvc;
        }

        [Authorize]
        [Route("recent-sales")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetRecentSales([FromQuery] SearchModel model)
        {
            var result = await _dashboardSvc.GetRecentSales(model);
            return StatusCode(result.StatusCode, result);
        }
        
        [Authorize]
        [Route("top-selling")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetTopSelling([FromQuery] SearchModel model)
        {
            var result = await _dashboardSvc.GetTopSelling(model);
            return StatusCode(result.StatusCode, result);
        }
        
        [Authorize]
        [Route("sale-information")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetSale()
        {
            var result = await _dashboardSvc.GetSale();
            return StatusCode(result.StatusCode, result);
        }
    }
}
