using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.PointModel;
using UtNhanDrug_BE.Services.PointService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/point-management")]
    public class PointController : ControllerBase
    {
        private readonly IPointSvc _pointSvc;

        public PointController(IPointSvc pointSvc)
        {
            _pointSvc = pointSvc;
        }

        [Authorize]
        [Route("points/information")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetInvoiceById()
        {
            var result = await _pointSvc.GetPointManager();
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("points")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateGoodsReceiptNote([FromBody] UpdatePointRequest model)
        {
            var result = await _pointSvc.UpdateManagePoint(model);
            return StatusCode(result.StatusCode, result);
        }
    }
}
