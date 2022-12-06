using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.PointModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.PointService
{
    public class PointSvc : IPointSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        public PointSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        public async Task<Response<PointViewModel>> GetPointManager()
        {
            var pointManager = await _context.DataSettingSystemCustomerPoints.FirstOrDefaultAsync();
            PointViewModel p = new PointViewModel()
            {
                Id = pointManager.Id,
                ToMoney = pointManager.ToMoney,
                ToPoint = pointManager.ToPoint
            };
            return new Response<PointViewModel>(p)
            {
                Message = "Thông tin quản lí điểm"
            };
        }

        public async Task<Response<bool>> UpdateManagePoint(UpdatePointRequest request)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var data = await _context.DataSettingSystemCustomerPoints.FirstOrDefaultAsync();
                if(data != null)
                {
                    data.ToPoint = request.ToPoint;
                    data.ToMoney = request.ToMoney;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Cập nhật thông tin thành công"
                    };
                }
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không tim thấy thông tin quản lí điểm"
                };
            }catch(Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }
    }
}
