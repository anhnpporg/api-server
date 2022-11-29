using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ProductActiveSubstance;
using System.Linq;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.ResponseModel;
using System;
using Microsoft.EntityFrameworkCore.Storage;
using UtNhanDrug_BE.Models.ActiveSubstanceModel;

namespace UtNhanDrug_BE.Services.ProductActiveSubstanceService
{
    public class PASSvc : IPASSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        public PASSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        //public async Task<Response<bool>> CheckPAS(int id)
        //{
        //    var result = await _context.ProductActiveSubstances.FirstOrDefaultAsync(x => x.Id == id);
        //    if (result != null) return true;
        //    return false;
        //}

        public async Task<Response<bool>> AddPAS(List<CreatePASModel> model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                foreach(var i in model)
                {
                    ProductActiveSubstance pas = new ProductActiveSubstance()
                    {
                        ProductId = i.ProductId,
                        ActiveSubstanceId = i.ActiveSubstanceId
                    };
                    _context.ProductActiveSubstances.Add(pas);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new Response<bool>(true)
                {
                    Message = "Thêm hoạt chất thành công"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 500,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<bool>> RemovePAS(RemoveActiveSubstanceModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var query = from acs in _context.ProductActiveSubstances
                            where acs.ProductId == model.ProductId & acs.ActiveSubstanceId == model.ActiveSubstanceId
                            select acs;
                var data = await query.FirstOrDefaultAsync();
                if (data != null)
                {
                    _context.ProductActiveSubstances.Remove(data);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Đã xoá hoạt chất khỏi thuốc"
                    };
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new Response<bool>(false)
                    {
                        StatusCode = 400,
                        Message = "Không tìm thấy hoạt chất này"
                    };
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 500,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        //public async Task<List<ViewPASModel>> GetAllPAS()
        //{
        //    var query = from pas in _context.ProductActiveSubstances
        //                select new { pas };
        //    var list = await query.Select(x => new ViewPASModel()
        //    {
        //       Id = x.pas.Id,
        //       ProductId = x.pas.ProductId,
        //       ActiveSubstanceId = x.pas.ActiveSubstanceId
        //    }).ToListAsync();
        //    return list;
        //}

        //public async Task<List<ViewPASModel>> GetPASById(int productId)
        //{
        //    var query = from pas in _context.ProductActiveSubstances
        //                where pas.ProductId == productId
        //                select new { pas };
        //    var list = await query.Select(x => new ViewPASModel()
        //    {
        //        Id = x.pas.Id,
        //        ProductId = x.pas.ProductId,
        //        ActiveSubstanceId = x.pas.ActiveSubstanceId
        //    }).ToListAsync();
        //    return list;
        //}

        //public async Task<bool> UpdatePASByProductId(int productId, List<UpdatePASModel> model)
        //{
        //    var query = from p in _context.ProductActiveSubstances
        //                where p.ProductId == productId
        //                select p;

        //    return true;
        //}
    }
}
