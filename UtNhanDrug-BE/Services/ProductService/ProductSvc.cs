using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ProductModel;
using System.Linq;
using System;
using UtNhanDrug_BE.Hepper.GenaralBarcode;
using UtNhanDrug_BE.Models.ProductActiveSubstance;
using UtNhanDrug_BE.Models.ActiveSubstanceModel;
using UtNhanDrug_BE.Services.ProductActiveSubstanceService;
using UtNhanDrug_BE.Models.BrandModel;

namespace UtNhanDrug_BE.Services.ProductService
{
    public class ProductSvc : IProductSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;

        public ProductSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckProduct(int id)
        {
            var result = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null) return true;
            return false;
        }

        public async Task<bool> CreateProduct(int userId, CreateProductModel model)
        {
            Product product = new Product()
            {
                DrugRegistrationNumber = model.DrugRegistrationNumber,
                Name = model.Name,
                Barcode = "####",
                BrandId = model.BrandId,
                CategoryId = model.CategoryId,
                MinimumQuantity = model.MinimumQuantity,
                Dosage = model.Dosage,
                DosageUnitId = model.DosageUnitId,
                UnitId = model.UnitId,
                Price = model.Price,
                
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };
            _context.Products.Add(product);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                product.Barcode = GenaralBarcode.CreateEan13(product.Id+"");
                foreach (var p in model.ActiveSubstances)
                {
                    ProductActiveSubstance pas = new ProductActiveSubstance()
                    {
                        ProductId = product.Id,
                        ActiveSubstanceId = p
                    };
                    product.ProductActiveSubstances.Add(pas);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            
            return false;
        }

        public async Task<bool> DeleteProduct(int id, int userId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product != null)
            {
                product.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ViewProductModel>> GetAllProduct()
        {
            var query = from p in _context.Products
                        select p;
            

            var result = await query.Select( p => new ViewProductModel()
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
                Category = new ViewModel()
                    {
                    Id = p.Category.Id,
                    Name = p.Category.Name
                },
                MinimumQuantity = p.MinimumQuantity,
                Dosage = p.Dosage,
                DosageUnit = new ViewModel()
                {
                    Id = p.DosageUnit.Id,
                    Name = p.DosageUnit.Name
                },
                Unit = new ViewModel()
                {
                    Id = p.Unit.Id,
                    Name = p.Unit.Name
                },
                Price = p.Price,
                CreatedAt = p.CreatedAt,
                CreatedBy = p.CreatedBy,
                UpdatedAt = p.UpdatedAt,
                UpdatedBy = p.UpdatedBy,
                IsActive = p.IsActive,
            }).ToListAsync();

            return result;
        }

        public async Task<List<ViewModel>> GetListActiveSubstances(int productId)
        {
            var query = from pas in _context.ProductActiveSubstances
                        where pas.ProductId == productId
                        select pas;
            var data = await query.Select(x => new ViewModel()
            {
                Id = x.ActiveSubstance.Id,
                Name = x.ActiveSubstance.Name
            }).ToListAsync();
            return data;
        }

        public async Task<ViewProductModel> GetProductById(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product != null)
            {
                ViewProductModel result = new ViewProductModel()
                {
                    Id = product.Id,
                    DrugRegistrationNumber = product.DrugRegistrationNumber,
                    Barcode = product.Barcode,
                    Name = product.Name,
                    Brand = new ViewModel() 
                    {
                        Id = product.Brand.Id,
                        Name = product.Brand.Name
                    },
                    Category = new ViewModel()
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name
                    },
                    MinimumQuantity = product.MinimumQuantity,
                    Dosage = product.Dosage,
                    DosageUnit = new ViewModel()
                    {
                        Id = product.DosageUnit.Id,
                        Name = product.DosageUnit.Name
                    },
                    Unit = new ViewModel()
                    {
                        Id = product.Unit.Id,
                        Name = product.Unit.Name
                    },
                    Price = product.Price,
                    CreatedAt = product.CreatedAt,
                    CreatedBy = product.CreatedBy,
                    UpdatedAt = product.UpdatedAt,
                    UpdatedBy = product.UpdatedBy,
                    IsActive = product.IsActive
                };
                return result;
            }
            return null;
        }

        public async Task<bool> UpdateProduct(int id, int userId, UpdateProductModel model)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product != null)
            {  
               product.DrugRegistrationNumber = model.DrugRegistrationNumber;
               product.Name = model.Name;
               product.BrandId = model.BrandId;
               product.CategoryId = model.CategoryId;
               product.MinimumQuantity = model.MinimumQuantity;
               product.Dosage = model.Dosage;
               product.DosageUnitId = model.DosageUnitId;
               product.UnitId = model.UnitId;
               product.Price = model.Price;
               product.UpdatedAt = DateTime.Now;
               product.UpdatedBy = userId;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
