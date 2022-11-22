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

        public async Task<Response<bool>> CreateProductUnit(int userId, CreateProductUnitPriceModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var unit = await _context.ProductUnitPrices.Where(x => x.ProductId == model.ProductId).ToListAsync();
                foreach(var unitPrice in unit)
                {
                    if (unitPrice.Unit == model.Unit) return new Response<bool>(false) { StatusCode = 400, Message = "Đơn vị này đã tồn tại" };
                }


                ProductUnitPrice pu = new ProductUnitPrice()
                {
                    ProductId = model.ProductId,
                    Unit = model.Unit,
                    ConversionValue = model.ConversionValue,
                    Price = model.Price,
                    IsBaseUnit = model.IsBaseUnit,
                    IsPackingSpecification = model.IsPackingSpecification,
                    IsDoseBasedOnBodyWeightUnit = model.IsDoseBasedOnBodyWeightUnit,
                    CreatedBy = userId,
                    CreatedAt = today
                };
                _context.ProductUnitPrices.Add(pu);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new Response<bool>(true)
                {
                    Message = "Tạo đơn vị tính thành công"
                };
            }
            catch(Exception)
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
                if(pu != null)
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

        public async Task<Response<bool>> UpdateProductUnit(int productUnitId, int userId, UpdateProductUnitPriceModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var pu = await _context.ProductUnitPrices.FirstOrDefaultAsync(x => x.Id == productUnitId);
                if (pu != null)
                {
                    pu.ConversionValue = model.ConversionValue;
                    pu.Price = model.Price;
                    pu.Unit = model.Unit;
                    pu.IsBaseUnit = model.IsBaseUnit;
                    pu.UpdatedBy = userId;
                    pu.UpdatedAt = today;
                    pu.IsPackingSpecification = model.IsPackingSpecification;
                    pu.IsDoseBasedOnBodyWeightUnit = model.IsDoseBasedOnBodyWeightUnit;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Cập nhật thông tin đơn vị tính thành công"
                    };
                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy đơn vị tính này"
                };
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
    }
}
