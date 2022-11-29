using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ProductUnitModel;
using System.Linq;
using UtNhanDrug_BE.Models.ResponseModel;
using System;
using Microsoft.EntityFrameworkCore.Storage;
using UtNhanDrug_BE.Hepper;
using UtNhanDrug_BE.Models.ProductUnitPriceModel;

namespace UtNhanDrug_BE.Services.ProductUnitService
{
    public class ProductUnitPriceSvc : IProductUnitPriceSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly DateTime today = LocalDateTime.DateTimeNow();
        public ProductUnitPriceSvc(ut_nhan_drug_store_databaseContext context)
        {

            _context = context;
        }
        //public async Task<bool> CheckProductUnit(int productUnitId)
        //{
        //    var pu = await _context.ProductUnitPrices.FirstOrDefaultAsync(x => x.Id == productUnitId);
        //    if (pu == null) return false;
        //    return true;
        //}

        public async Task<Response<bool>> AddProductUnit(int userId, List<CreateProductUnitPriceModel> model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                if (model.Count > 0)
                {
                    foreach (var pu in model)
                    {
                        //_context.ProductUnitPrices.Add(du);
                        var unit = await _context.ProductUnitPrices.Where(x => x.ProductId == pu.ProductId).ToListAsync();
                        foreach (var unitPrice in unit)
                        {
                            if (unitPrice.Unit == pu.Unit) return new Response<bool>(false) { StatusCode = 400, Message = "Đơn vị này đã tồn tại" };
                        }

                        ProductUnitPrice x = new ProductUnitPrice()
                        {
                            ProductId = pu.ProductId,
                            Unit = pu.Unit,
                            ConversionValue = pu.ConversionValue,
                            Price = pu.Price,
                            IsBaseUnit = false,
                            IsDoseBasedOnBodyWeightUnit = false,
                            IsPackingSpecification = true,
                            CreatedBy = userId,
                            CreatedAt = today
                        };
                        _context.ProductUnitPrices.Add(x);
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Thêm đơn vị mới thành công"
                    };
                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không có gì để thêm"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tạo đơn vị tính thất bại"
                };
            }
        }

        public async Task<Response<List<ViewProductUnitPriceModel>>> GetProductUnitByProductId(int productId)
        {
            try
            {
                var query = from pu in _context.ProductUnitPrices
                            where pu.ProductId == productId
                            select pu;
                var data = await query.Where(x => x.IsDoseBasedOnBodyWeightUnit == false).Select(x => new ViewProductUnitPriceModel()
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Unit = x.Unit,
                    Price = x.Price,
                    ConversionValue = x.ConversionValue,
                    IsBaseUnit = x.IsBaseUnit,
                    IsDoseBasedOnBodyWeightUnit = x.IsDoseBasedOnBodyWeightUnit,
                    IsPackingSpecification = x.IsPackingSpecification,
                    IsActive = x.IsActive,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy
                }).ToListAsync();
                if (data != null)
                {
                    if (data.Count > 0)
                    {
                        double basePrice = 0;
                        foreach (var u in data)
                        {
                            if (u.IsBaseUnit == true)
                            {
                                basePrice = (double)u.Price;
                            }
                        }
                        if (data != null)
                        {
                            foreach (var x in data)
                            {
                                if (x.Price == null)
                                {
                                    x.Price = (decimal)(basePrice * x.ConversionValue);
                                }
                            }
                        }
                        return new Response<List<ViewProductUnitPriceModel>>(data)
                        {
                            Message = "Đơn vị tính của sản phẩm"
                        };
                    }
                    else
                    {
                        return new Response<List<ViewProductUnitPriceModel>>(null)
                        {
                            Message = "Không có đơn vị tính"
                        };
                    }
                }
                else
                {
                    return new Response<List<ViewProductUnitPriceModel>>(null)
                    {
                        StatusCode = 400,
                        Message = "Không tìm thấy đơn vị tính theo sản phẩm này"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewProductUnitPriceModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }
        public async Task<Response<ViewProductUnitPriceModel>> GetProductUnitById(int productUnitId)
        {
            try
            {
                var pu = await _context.ProductUnitPrices.FirstOrDefaultAsync(x => x.Id == productUnitId);
                if (pu != null)
                {
                    var data = new ViewProductUnitPriceModel()
                    {
                        Id = pu.Id,
                        ProductId = pu.ProductId,
                        Unit = pu.Unit,
                        ConversionValue = pu.ConversionValue,
                        Price = pu.Price,
                        IsBaseUnit = pu.IsBaseUnit,
                        IsDoseBasedOnBodyWeightUnit = pu.IsDoseBasedOnBodyWeightUnit,
                        IsPackingSpecification = pu.IsPackingSpecification,
                        IsActive = pu.IsActive,
                        CreatedBy = pu.CreatedBy,
                        CreatedAt = pu.CreatedAt,
                        UpdatedAt = pu.UpdatedAt,
                        UpdatedBy = pu.UpdatedBy
                    };
                    return new Response<ViewProductUnitPriceModel>(data)
                    {
                        Message = "Thông tin của đơn vị tính"
                    };
                }
                else
                {
                    return new Response<ViewProductUnitPriceModel>(null)
                    {
                        StatusCode = 400,
                        Message = "Không tìm thấy đơn vị tính này"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<ViewProductUnitPriceModel>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }


        }

        public async Task<Response<bool>> UpdateProductUnit(int productUnitId, int userId, List<UpdateProductUnitPriceModel> model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                if (model.Count > 0)
                {
                    foreach (var u in model)
                    {
                        var productUnit = await _context.ProductUnitPrices.FirstOrDefaultAsync(x => x.Id == u.ProductUnitId);
                        var query = from pu in _context.ProductUnitPrices
                                    where pu.ProductId == u.ProductId
                                    select pu;
                        var puId = await query.Where(x => x.IsBaseUnit == true).Select(x => x.Id).FirstOrDefaultAsync();
                        if (productUnit != null)
                        {
                            if (puId > 0)
                            {
                                if (puId == u.ProductUnitId)
                                {
                                    productUnit.Unit = u.Unit;
                                    productUnit.Price = u.Price;
                                    productUnit.UpdatedAt = today;
                                    productUnit.UpdatedBy = userId;
                                }
                                else
                                {
                                    productUnit.Unit = u.Unit;
                                    productUnit.Price = u.Price;
                                    productUnit.ConversionValue = u.ConversionValue;
                                    productUnit.UpdatedAt = today;
                                    productUnit.UpdatedBy = userId;
                                }
                                await _context.SaveChangesAsync();
                                await transaction.CommitAsync();
                                return new Response<bool>(true)
                                {
                                    Message = "Cập nhật đơn vị thành công"
                                };
                            }
                            else
                            {
                                return new Response<bool>(false)
                                {
                                    StatusCode = 400,
                                    Message = "Không tìm thấy đơn vị cơ bản"
                                };
                            }
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                            return new Response<bool>(false)
                            {
                                StatusCode = 400,
                                Message = "Không tìm thấy đơn vị để cập nhật"
                            };
                        }
                    }
                    return new Response<bool>(true)
                    {
                        Message = "Không có gì xảy ra"
                    };
                }
                else
                {
                    return new Response<bool>(true)
                    {
                        Message = "Không có gì thay đổi"
                    };
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Cập nhật thông tin đơn vị thành công"
                };
            }
        }

        public async Task<Response<ViewBaseUnitModel>> GetBaseUnit(int productId)
        {
            var query = from pu in _context.ProductUnitPrices
                        where pu.ProductId == productId
                        select pu;
            var data = await query.Where(x => x.IsBaseUnit == true).Select(x => new ViewBaseUnitModel()
            {
                Id = x.Id,
                BaseUnit = x.Unit,
                BasePrice = x.Price
            }).FirstOrDefaultAsync();
            if (data != null)
            {
                return new Response<ViewBaseUnitModel>(data)
                {
                    Message = "Thành công"
                };
            }
            else return new Response<ViewBaseUnitModel>(null)
            {
                StatusCode = 400,
                Message = "Không tìm thấy"
            };
        }

        public async Task<Response<bool>> RemoveProductUnit(int productUnitId)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var query = from pu in _context.ProductUnitPrices
                            where pu.Id == productUnitId
                            select pu;
                var data = await query.FirstOrDefaultAsync();
                if (data != null)
                {
                    if (data.IsBaseUnit == true)
                    {
                        return new Response<bool>(false)
                        {
                            StatusCode = 400,
                            Message = "Không thể xoá đơn vị cơ bản"
                        };
                    }
                    else
                    {
                        _context.ProductUnitPrices.Remove(data);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Xoá thành công"
                        };
                    }
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new Response<bool>(false)
                    {
                        StatusCode = 400,
                        Message = "Không tìm thấy đơn vị này"
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
    }
}
