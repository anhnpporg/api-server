using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.BrandModel;
using System.Linq;
using System;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Models.ModelHelper;
using Microsoft.EntityFrameworkCore.Storage;
using UtNhanDrug_BE.Models.ResponseModel;
using UtNhanDrug_BE.Services.ProductService;
using UtNhanDrug_BE.Hepper;

namespace UtNhanDrug_BE.Services.BrandService
{
    public class BrandSvc : IBrandSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly IProductSvc _productSvc;
        private readonly DateTime today = LocalDateTime.DateTimeNow();

        public BrandSvc(ut_nhan_drug_store_databaseContext context, IProductSvc productSvc)
        {
            _context = context;
            _productSvc = productSvc;
        }
        private async Task<bool> CheckName (string name)
        {
            var result = await _context.Brands.FirstOrDefaultAsync(x => x.Name == name);
            if(result == null)
            {
                return true;
            }
            return false;
        }
        public async Task<Response<bool>> CreateBrand(int userId, CreateBrandModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                if (await CheckName(model.Name) == false) return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tên nhà sản xuất này đã tồn tại"
                };
                Brand brand = new Brand()
                {
                    Name = model.Name,
                    CreatedBy = userId,
                    CreatedAt = today
                };
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new Response<bool>(true)
                {
                    Message = "Tạo nhà sản xuất thành công"
                };
            }
            catch
            {
                transaction.Rollback();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Nhà sản xuất thất bại"
                };
            }
        }

        public async Task<Response<bool>> UpdateBrand(int brandId,int userId, UpdateBrandModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == brandId);
                if (brand != null)
                {
                    if (await CheckName(model.Name) == false) return new Response<bool>(false)
                    {
                        StatusCode = 400,
                        Message = "Tên nhà sản xuất này đã tồn tại"
                    };
                    brand.Name = model.Name;
                    brand.UpdatedAt = today;
                    brand.UpdatedBy = userId;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Cập nhật thông tin thành công"
                    };
                }
                return new Response<bool>(false)
                {
                    Message = "Không tìm thấy nhà xản xuất",
                    StatusCode = 400
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

        public async Task<Response<bool>> DeleteBrand(int brandId, int userId)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == brandId);
                if (brand != null)
                {
                    if(brand.IsActive == false)
                    {
                        brand.IsActive = true;
                        brand.UpdatedBy = userId;
                        brand.UpdatedAt = DateTime.Now;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Nhà sản xuất đang hoạt động"
                        };
                    }
                    else
                    {
                        brand.IsActive = false;
                        brand.UpdatedBy = userId;
                        brand.UpdatedAt = DateTime.Now;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Nhà sản xuất ngưng hoạt động"
                        };
                    }
                    
                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy nhà sản xuất này"
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

        public async Task<Response<ViewBrandModel>> GetBrandById(int brandId)
        {
            try
            {
                var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == brandId);
                if (brand != null)
                {
                    ViewBrandModel result = new ViewBrandModel()
                    {
                        Id = brand.Id,
                        Name = brand.Name,
                        IsActive = brand.IsActive,
                        CreatedAt = brand.CreatedAt,
                        CreatedBy = brand.CreatedBy,
                        UpdatedAt = brand.UpdatedAt,
                        UpdatedBy = brand.UpdatedBy,
                    };
                    return new Response<ViewBrandModel>(result)
                    {
                        Message = "Thông tin nhà sản xuất"
                    };
                }
                return new Response<ViewBrandModel>(null)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy nhà sản xuất"
                };
            }
            catch (Exception)
            {
                return new Response<ViewBrandModel>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<List<ViewBrandModel>>> GetAllBrand()
        {
            try
            {
                var query = from b in _context.Brands
                            select b;

                var result = await query.OrderByDescending(x => x.CreatedAt).Select(b => new ViewBrandModel()
                {
                    Id = b.Id,
                    Name = b.Name,
                    CreatedAt = b.CreatedAt,
                    CreatedBy = b.CreatedBy,
                    UpdatedAt = b.UpdatedAt,
                    UpdatedBy = b.UpdatedBy,
                    IsActive = b.IsActive,
                }).ToListAsync();
                if(result.Count > 0)
                {
                    return new Response<List<ViewBrandModel>>(result)
                    {
                        Message = "Thông tin tất cả nhà sản xuất"
                    };
                }
                else
                {
                    return new Response<List<ViewBrandModel>>(null)
                    {
                        Message = "Không tồn tại nhà sản xuất"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewBrandModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            } 
        }

        //public async Task<bool> CheckBrand(int brandId)
        //{
        //    var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == brandId);
        //    if (brand != null) return true;
        //    return false;
        //}

        public async Task<Response<List<ViewProductModel>>> GetListProduct(int brandId)
        {
            try
            {
                var query = from p in _context.Products
                            where p.BrandId == brandId
                            select p;

                var data = await query.Select(p => new ViewProductModel()
                {
                    Id = p.Id,
                    DrugRegistrationNumber = p.DrugRegistrationNumber,
                    Barcode = p.Barcode,
                    Name = p.Name,
                    Brand = new ViewModel()
                    {
                        Id = p.Brand.Id,
                        Name = p.Brand.Name
                    },
                    Shelf = new ViewModel()
                    {
                        Id = p.Shelf.Id,
                        Name = p.Shelf.Name
                    },
                    MininumInventory = p.MininumInventory,
                    RouteOfAdministration = new ViewModel()
                    {
                        Id = p.RouteOfAdministration.Id,
                        Name = p.RouteOfAdministration.Name
                    },
                    IsUseDose = p.IsUseDose,
                    IsManagedInBatches = p.IsManagedInBatches,
                    CreatedAt = p.CreatedAt,
                    CreatedBy = new ViewModel()
                    {
                        Id = p.CreatedByNavigation.Id,
                        Name = p.CreatedByNavigation.FullName
                    },
                    UpdatedAt = p.UpdatedAt,
                    UpdatedBy = p.UpdatedBy,
                    IsActive = p.IsActive,
                }).OrderByDescending(p => p.IsActive).ToListAsync();
                if(data.Count > 0)
                {
                    foreach (var product in data)
                    {
                        var activeSubstance = await _productSvc.GetListActiveSubstances(product.Id);
                        product.ActiveSubstances = activeSubstance.Data;
                    }
                    return new Response<List<ViewProductModel>>(data)
                    {
                        Message = "Danh sách sản phẩm"
                    };
                }
                else
                {
                    return new Response<List<ViewProductModel>>(null)
                    {
                        Message = "Không có sản phẩm nào"
                    };
                }
            }
            catch(Exception){
                return new Response<List<ViewProductModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
            
        }

        public async Task<Response<List<ViewModel>>> GetListBrand()
        {
            try
            {
                var query = from b in _context.Brands
                            select b;

                var result = await query.Where(x => x.IsActive == true).OrderByDescending(x => x.CreatedAt).Select(b => new ViewModel()
                {
                    Id = b.Id,
                    Name = b.Name,
                }).ToListAsync();
                if (result.Count > 0)
                {
                    return new Response<List<ViewModel>>(result)
                    {
                        Message = "Thông tin tất cả nhà sản xuất"
                    };
                }
                else
                {
                    return new Response<List<ViewModel>>(null)
                    {
                        Message = "Không tồn tại nhà sản xuất"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }
    }
}
