using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ProductUnitModel;
using System.Linq;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Services.ProductUnitService
{
    public class ProductUnitPriceSvc : IProductUnitPriceSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        public ProductUnitPriceSvc(ut_nhan_drug_store_databaseContext context)
        {

            _context = context;
        }
        public async Task<bool> CheckProductUnit(int productUnitId)
        {
            var pu = await _context.ProductUnitPrices.FirstOrDefaultAsync(x => x.Id == productUnitId);
            if (pu == null) return false;
            return true;
        }

        public async Task<bool> CreateProductUnit(int userId, CreateProductUnitPriceModel model)
        {
            ProductUnitPrice pu = new ProductUnitPrice()
            {
                ProductId = model.ProductId,
                Unit = model.Unit,
                ConversionValue = model.ConversionValue,
                Price = model.Price,
                IsBaseUnit = model.IsBaseUnit,
                IsPackingSpecification = model.IsPackingSpecification,
                IsDoseBasedOnBodyWeightUnit = model.IsDoseBasedOnBodyWeightUnit,
                CreatedBy = userId
            };
            _context.ProductUnitPrices.Add(pu);
            var result = await _context.SaveChangesAsync();
            if (result >= 0) return true;
            return false;
        }

        public async Task<List<ViewProductUnitPriceModel>> GetProductUnitByProductId(int productId)
        {
            var query = from pu in _context.ProductUnitPrices
                        where pu.ProductId == productId
                        select pu;
            var data = await query.Select(x => new ViewProductUnitPriceModel()
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
            double basePrice = 0;
            foreach (var u in data)
            {
                if(u.IsBaseUnit == true)
                {
                    basePrice = (double)u.Price;
                }
            }
            if (data != null){ 
                foreach(var x in data)
                {
                    if(x.Price == null)
                    {
                        x.Price = (decimal)(basePrice*x.ConversionValue);
                    }
                }
            }
            return data;

        }

        public async Task<ViewProductUnitPriceModel> GetProductUnitById(int productUnitId)
        {
            var pu = await _context.ProductUnitPrices.FirstOrDefaultAsync(x => x.Id == productUnitId);
            return pu == null ? null : new ViewProductUnitPriceModel()
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

        }

        public async Task<bool> UpdateProductUnit(int productUnitId, int userId, UpdateProductUnitPriceModel model)
        {
            var pu = await _context.ProductUnitPrices.FirstOrDefaultAsync(x => x.Id == productUnitId);
            if(pu != null)
            {
                pu.ConversionValue = model.ConversionValue;
                pu.Price = model.Price;
                pu.Unit = model.Unit;
                pu.IsBaseUnit = model.IsBaseUnit;
                pu.UpdatedBy = userId;
                pu.IsPackingSpecification= model.IsPackingSpecification;
                pu.IsDoseBasedOnBodyWeightUnit = model.IsDoseBasedOnBodyWeightUnit;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
