﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ShelfModel;
using System.Linq;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.ResponseModel;
using Microsoft.EntityFrameworkCore.Storage;

namespace UtNhanDrug_BE.Services.ShelfService
{
    public class ShelfSvc : IShelfSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        public ShelfSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }

        //public async Task<bool> CheckShelf(int shelfId)
        //{
        //    var shelf = await _context.Shelves.FirstOrDefaultAsync(x => x.Id == shelfId);
        //    if (shelf != null) return true;
        //    return false;
        //}

        public async Task<Response<bool>> CreateShelf(int userId, CreateShelfModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                Shelf shelf = new Shelf()
                {
                    Name = model.Name,
                    CreatedBy = userId,
                };
                _context.Shelves.Add(shelf);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new Response<bool>(true)
                {
                    Message = "Tạo kệ thuốc thành công"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tạo kệ thuốc không thành công"
                };
            }
            
        }

        public async Task<Response<bool>> DeleteShelf(int shelfId, int userId)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var category = await _context.Shelves.FirstOrDefaultAsync(x => x.Id == shelfId);
                if (category != null)
                {
                    if(category.IsActive == true)
                    {
                        category.IsActive = false;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Kệ thuốc ngưng hoạt động"
                        };
                    }
                    else
                    {
                        category.IsActive = true;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Kệ thuốc hoạt động"
                        };
                    } 
                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Kệ thuốc không tồn tại"
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

        public async Task<Response<List<ViewShelfModel>>> GetAllShelves()
        {
            try
            {
                var query = from c in _context.Shelves
                            select c;
                var result = await query.OrderByDescending(x => x.CreatedAt).Select(c => new ViewShelfModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = new ViewModel()
                    {
                        Id = c.CreatedByNavigation.Id,
                        Name = c.CreatedByNavigation.FullName
                    },
                    UpdatedAt = c.UpdatedAt,
                }).ToListAsync();
                if(result.Count > 0)
                {
                    return new Response<List<ViewShelfModel>>(result)
                    {
                        Message = "Thông tin tất cả các kệ thuốc"
                    };
                }
                else
                {
                    return new Response<List<ViewShelfModel>>(null)
                    {
                        Message = "Không tìm thấy thông tin kệ thuốc nào"
                    };
                }
            }catch (Exception)
            {
                return new Response<List<ViewShelfModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<ViewShelfModel>> GetShelfById(int shelfId)
        {
            try
            {
                var shelf = await _context.Shelves.FirstOrDefaultAsync(x => x.Id == shelfId);
                if (shelf != null)
                {
                    ViewShelfModel result = new ViewShelfModel()
                    {
                        Id = shelf.Id,
                        Name = shelf.Name,
                        IsActive = shelf.IsActive,
                        CreatedAt = shelf.CreatedAt,
                        CreatedBy = new ViewModel()
                        {
                            Id = shelf.CreatedByNavigation.Id,
                            Name = shelf.CreatedByNavigation.FullName
                        },
                    };
                    return new Response<ViewShelfModel>(result)
                    {
                        Message = "Thông tin kệ thuốc"
                    };
                }
                return new Response<ViewShelfModel>(null)
                {
                    StatusCode = 400,
                    Message = "Kệ thuốc không tồn tại"
                };
            }
            catch (Exception)
            {
                return new Response<ViewShelfModel>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<List<ViewProductModel>>> GetListProduct(int categoryId)
        {
            try
            {
                var query = from p in _context.Products
                            where p.ShelfId == categoryId
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
                }).ToListAsync();
                if(data.Count > 0)
                {
                    return new Response<List<ViewProductModel>>(data)
                    {
                        Message = "Thuốc có trong kệ"
                    };
                }
                else
                {
                    return new Response<List<ViewProductModel>>(null)
                    {
                        Message = "Không có thuốc nào trong kệ"
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

        public async Task<Response<bool>> UpdateShelf(int categoryId, int userId, UpdateShelfModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var shelf = await _context.Shelves.FirstOrDefaultAsync(x => x.Id == categoryId);
                if (shelf != null)
                {
                    shelf.Name = model.Name;
                    shelf.UpdatedAt = DateTime.Now;
                    shelf.UpdatedBy = userId;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Cập nhật kệ thuốc thành công"
                    };
                }
                else
                {
                    return new Response<bool>(false)
                    {
                        StatusCode = 400,
                        Message = "không tìm thấy kệ thuốc này"
                    };
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Cập nhật kệ thuốc thất bại"
                };
            }
        }
    }
}
