using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ProductUnitModel;
using System.Linq;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Services.ProductUnitService
{
    public class ProductUnitSvc : IProductUnitSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        public ProductUnitSvc(ut_nhan_drug_store_databaseContext context)
        {

            _context = context;
        }
        public async Task<bool> CheckProductUnit(int productUnitId)
        {
            var pu = await _context.ProductUnits.FirstOrDefaultAsync(x => x.Id == productUnitId);
            if (pu == null) return false;
            return true;
        }

        public async Task<bool> CreateProductUnit(CreateProductUnitModel model)
        {
            ProductUnit pu = new ProductUnit()
            {
                ProductId = model.ProductId,
                UnitId = model.UnitId,
                ConversionValue = model.ConversionValue,
                Price = model.Price,
                IsBaseUnit = model.IsBaseUnit
            };
            _context.ProductUnits.Add(pu);
            var result = await _context.SaveChangesAsync();
            if (result >= 0) return true;
            return false;
        }

        public async Task<List<ViewProductUnitModel>> GetProductUnitByProductId(int productId)
        {
            var query = from pu in _context.ProductUnits
                        where pu.ProductId == productId
                        select pu;
            var data = await query.Select(x => new ViewProductUnitModel()
            {
                Id = x.Id,
                ProductId = x.ProductId,
                Unit = new ViewModel()
                {
                    Id = x.Unit.Id,
                    Name = x.Unit.Name
                },
                ConversionValue = x.ConversionValue,
                Price = x.Price,
                IsBaseUnit = x.IsBaseUnit,
            }).ToListAsync();
            return data;

        }

        public async Task<ViewProductUnitModel> GetProductUnitById(int productUnitId)
        {
            var pu = await _context.ProductUnits.FirstOrDefaultAsync(x => x.Id == productUnitId);
            return pu == null ? null : new ViewProductUnitModel()
            {
                Id = pu.Id,
                ProductId = pu.ProductId,
                Unit = new ViewModel()
                {
                    Id = pu.Unit.Id,
                    Name = pu.Unit.Name
                },
                ConversionValue = pu.ConversionValue,
                Price = pu.Price,
                IsBaseUnit = pu.IsBaseUnit,
            };

        }

        public async Task<bool> UpdateProductUnit(int productUnitId, UpdateProductUnitModel model)
        {
            var pu = await _context.ProductUnits.FirstOrDefaultAsync(x => x.Id == productUnitId);
            if(pu != null)
            {
                pu.ConversionValue = model.ConversionValue;
                pu.Price = model.Price;
                pu.UnitId = model.UnitId;
                pu.ProductId = model.ProductId;
                pu.IsBaseUnit = model.IsBaseUnit;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
