using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ActiveSubstanceModel;
using System.Linq;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Models.ModelHelper;
using Microsoft.EntityFrameworkCore.Storage;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.ActiveSubstanceService
{
    public class ActiveSubstanceSvc : IActiveSubstanceSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;

        public ActiveSubstanceSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }

        //public async Task<Response<bool>> CheckActiveSubstance(int id)
        //{
        //    var result = await _context.ActiveSubstances.FirstOrDefaultAsync(x => x.Id == id);
        //    if (result != null) return true;
        //    return false;
        //}

        public async Task<Response<bool>> CreateActiveSubstance(int userId, CreateActiveSubstanceModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                ActiveSubstance a = new ActiveSubstance()
                {
                    Name = model.Name,
                    CreatedBy = userId,
                };
                _context.ActiveSubstances.Add(a);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new Response<bool>(true)
                {
                    Message = "Tạo hoạt chất mới thành công"
                };
            }
            catch (Exception)
            {
                transaction.Rollback();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tạo hoạt chất mới thất bại"
                };
            }
        }

        public async Task<Response<bool>> DeleteActiveSubstance(int id, int userId)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var result = await _context.ActiveSubstances.FirstOrDefaultAsync(x => x.Id == id);
                if (result != null)
                {
                    if(result.IsActive == false)
                    {
                        result.IsActive = true;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Hoạt chất đã hoạt dộng"
                        };
                    }
                    else
                    {
                        result.IsActive = false;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Hoạt chất ngưng hoạt động"
                        };
                    }
                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy hoạt chất này"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Xoá hoạt chất thất bại"
                };
            }
            
        }

        public async Task<Response<ViewProductActiveSubstanceModel>> GetActiveSubstanceById(int id)
        {
            try
            {
                var activeSubstance = await _context.ActiveSubstances.FirstOrDefaultAsync(x => x.Id == id);
                if (activeSubstance != null)
                {
                    ViewProductActiveSubstanceModel result = new ViewProductActiveSubstanceModel()
                    {
                        Id = activeSubstance.Id,
                        Name = activeSubstance.Name,
                        IsActive = activeSubstance.IsActive,
                        CreatedAt = activeSubstance.CreatedAt,
                        CreatedBy = activeSubstance.CreatedBy,
                        UpdatedAt = activeSubstance.UpdatedAt,
                        UpdatedBy = activeSubstance.UpdatedBy
                    };
                    return new Response<ViewProductActiveSubstanceModel>(result)
                    {
                        Message = "Thông tin hoạt chất"
                    };
                }
                return new Response<ViewProductActiveSubstanceModel>(null)
                {
                    StatusCode = 400,
                    Message = "Không có hoạt chất nào"
                };
            }
            catch (Exception)
            {
                return new Response<ViewProductActiveSubstanceModel>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<List<ViewProductActiveSubstanceModel>>> GetAllActiveSubstance()
        {
            try
            {
                var query = from b in _context.ActiveSubstances
                            select b;

                var result = await query.Select(a => new ViewProductActiveSubstanceModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    CreatedAt = a.CreatedAt,
                    CreatedBy = a.CreatedBy,
                    UpdatedAt = a.UpdatedAt,
                    UpdatedBy = a.UpdatedBy,
                    IsActive = a.IsActive,
                }).ToListAsync();
                if(result.Count > 0)
                {
                    return new Response<List<ViewProductActiveSubstanceModel>>(result)
                    {
                        Message = "Tất cả thông tin hoạt chất"
                    };
                }
                else
                {
                    return new Response<List<ViewProductActiveSubstanceModel>>(result)
                    {
                        Message = "Không tìm thấy thông tin hoạt chất này"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewProductActiveSubstanceModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<List<ViewProductModel>>> GetListProducts(int activeSubstanceId)
        {
            try
            {
                var query = from a in _context.ProductActiveSubstances
                            where a.ActiveSubstanceId == activeSubstanceId
                            select a;
                var data = await query.Select(p => new ViewProductModel()
                {
                    Id = p.Product.Id,
                    DrugRegistrationNumber = p.Product.DrugRegistrationNumber,
                    Barcode = p.Product.Barcode,
                    Name = p.Product.Name,
                    Brand = new ViewModel()
                    {
                        Id = p.Product.Brand.Id,
                        Name = p.Product.Brand.Name
                    },
                    Shelf = new ViewModel()
                    {
                        Id = p.Product.Shelf.Id,
                        Name = p.Product.Shelf.Name
                    },
                    MininumInventory = p.Product.MininumInventory,
                    RouteOfAdministration = new ViewModel()
                    {
                        Id = p.Product.RouteOfAdministration.Id,
                        Name = p.Product.RouteOfAdministration.Name
                    },
                    IsUseDose = p.Product.IsUseDose,
                    IsManagedInBatches = p.Product.IsManagedInBatches,
                    CreatedAt = p.Product.CreatedAt,
                    CreatedBy = new ViewModel()
                    {
                        Id = p.Product.CreatedByNavigation.Id,
                        Name = p.Product.CreatedByNavigation.FullName
                    },
                    UpdatedAt = p.Product.UpdatedAt,
                    UpdatedBy = p.Product.UpdatedBy,
                    IsActive = p.Product.IsActive,
                }).ToListAsync();
                if(data.Count > 0)
                {
                    return new Response<List<ViewProductModel>>(data)
                    {
                        Message = "Danh sách sản phẩm theo hoạt chất"
                    };
                }
                else
                {
                    return new Response<List<ViewProductModel>>(null)
                    {
                        Message = "không có sản phẩm nào có hoạt chất này"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewProductModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<bool>> UpdateActiveSubstance(int id, int userId, UpdateActiveSubstanceModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var result = await _context.ActiveSubstances.FirstOrDefaultAsync(x => x.Id == id);
                if (result != null)
                {
                    result.Name = model.Name;
                    result.UpdatedAt = DateTime.Now;
                    result.UpdatedBy = userId;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Cập nhật thông tin hoạt chất thành công"
                    };
                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy hoạt chất này"
                };
            }
            catch (Exception)
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
