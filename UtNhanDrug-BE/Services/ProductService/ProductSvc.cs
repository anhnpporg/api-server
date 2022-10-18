using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ProductModel;
using System.Linq;
using System;
using UtNhanDrug_BE.Hepper.GenaralBarcode;
using UtNhanDrug_BE.Models.ModelHelper;

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
                ShelfId = model.ShelfId,
                MinimumQuantity = model.MinimumQuantity,
                StockStrength = model.StockStrength,
                StockStrengthUnitId = model.StockStrengthUnitId,
                RouteOfAdministrationId = model.RouteOfAdministrationId,
                IsMedicine = model.IsMedicine,
                IsConsignment = model.IsConsignment,
                CreatedBy = userId
            };
            _context.Products.Add(product);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                product.Barcode = GenaralBarcode.CreateEan13(product.Id+"");
                await _context.SaveChangesAsync();
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
                Shelf = new ViewModel()
                {
                    Id = p.Shelf.Id,
                    Name = p.Shelf.Name
                },
                MinimumQuantity = p.MinimumQuantity,
                StockStrength = p.StockStrength,
                StockStrengthUnit = new ViewModel()
                {
                    Id = p.StockStrengthUnit.Id,
                    Name = p.StockStrengthUnit.Name
                },
                RouteOfAdministration = new ViewModel()
                {
                    Id = p.RouteOfAdministration.Id,
                    Name = p.RouteOfAdministration.Name
                },
                IsMedicine = p.IsMedicine,
                IsConsignment = p.IsConsignment,
                CreatedAt = p.CreatedAt,
                CreatedBy = new ViewModel()
                {
                    Id = p.CreatedByNavigation.Id,
                    Name = p.CreatedByNavigation.UserAccount.FullName
                },
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
                    Shelf = new ViewModel()
                    {
                        Id = product.Shelf.Id,
                        Name = product.Shelf.Name
                    },
                    MinimumQuantity = product.MinimumQuantity,
                    StockStrength = product.StockStrength,
                    StockStrengthUnit = new ViewModel()
                    {
                        Id = product.StockStrengthUnit.Id,
                        Name = product.StockStrengthUnit.Name
                    },
                    RouteOfAdministration = new ViewModel()
                    {
                        Id = product.RouteOfAdministration.Id,
                        Name = product.RouteOfAdministration.Name
                    },
                    IsMedicine = product.IsMedicine,
                    IsConsignment = product.IsConsignment,
                    CreatedAt = product.CreatedAt,
                    CreatedBy = new ViewModel()
                    {
                        Id = product.CreatedByNavigation.UserAccount.Id,
                        Name = product.CreatedByNavigation.UserAccount.FullName
                    },
                    IsActive = product.IsActive,
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
                product.ShelfId = model.ShelfId;
                product.MinimumQuantity = model.MinimumQuantity;
                product.StockStrength = model.StockStrength;
                product.StockStrengthUnitId = model.StockStrengthUnitId;
                product.RouteOfAdministrationId = model.RouteOfAdministrationId;
                product.IsMedicine = model.IsMedicine;
                product.IsConsignment = model.IsConsignment;
                product.IsActive = model.IsActive;
                product.UpdatedAt = DateTime.Now;
                product.UpdatedBy = userId;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
