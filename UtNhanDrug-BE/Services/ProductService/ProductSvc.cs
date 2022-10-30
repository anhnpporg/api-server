using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ProductModel;
using System.Linq;
using System;
using UtNhanDrug_BE.Hepper.GenaralBarcode;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.PagingModel;

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
                MininumInventory = model.MininumInventory,
                RouteOfAdministrationId = model.RouteOfAdministrationId,
                IsUseDose = model.IsUseDose,
                IsManagedInBatches = model.IsManagedInBatches,
                CreatedBy = userId,
            };
            _context.Products.Add(product);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                product.Barcode = GenaralBarcode.CreateEan13(product.Id+"");
                await _context.SaveChangesAsync();
                ProductUnitPrice pu = new ProductUnitPrice()
                {
                    ProductId = product.Id,
                    Unit = model.Unit,
                    ConversionValue = 1,
                    Price = model.Price,
                    IsBaseUnit = true,
                    IsPackingSpecification = model.IsPackingSpecification,
                    IsDoseBasedOnBodyWeightUnit = model.IsDoseBasedOnBodyWeightUnit
                };
                _context.ProductUnitPrices.Add(pu);

                foreach (var productUnit in model.ProductUnits)
                {
                    ProductUnitPrice x = new ProductUnitPrice()
                    {
                        ProductId = product.Id,
                        Unit = productUnit.Unit,
                        ConversionValue = productUnit.ConversionValue,
                        Price = productUnit.Price,
                        IsBaseUnit = false,
                        IsDoseBasedOnBodyWeightUnit = productUnit.IsDoseBasedOnBodyWeightUnit,
                        IsPackingSpecification = productUnit.IsPackingSpecification,   
                    };
                    _context.ProductUnitPrices.Add(x);
                }

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
                MininumInventory = p.MininumInventory,
                RouteOfAdministration = new ViewModel()
                {
                    Id = p.RouteOfAdministration.Id,
                    Name = p.RouteOfAdministration.Name
                },
                IsUseDose = p.IsUseDose,
                IsManagedInBatches = p.IsManagedInBatches,
                CreatedAt = p.CreatedAt,
                CreatedBy = p.CreatedBy,
                UpdatedAt = p.UpdatedAt,
                UpdatedBy = p.UpdatedBy,
                IsActive = p.IsActive,
            }).ToListAsync();

            return result;
        }

        public async Task<PageResult<ViewProductModel>> GetProductPaging(ProductPagingRequest request)
        {
            var query = from p in _context.Products
                        join pas in _context.ProductActiveSubstances on p.Id equals pas.ProductId
                        where p.Name.Contains(request.SearchValue) || p.Barcode.Contains(request.SearchValue) || pas.ActiveSubstance.Name.Equals(request.SearchValue)
                        select  p;
            var data = query.Distinct();
            var result = await data.Select( p => new ViewProductModel()
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
                CreatedBy = p.CreatedBy,
                UpdatedAt = p.UpdatedAt,
                UpdatedBy = p.UpdatedBy,
                IsActive = p.IsActive,
            }).ToListAsync();
            //paging
            int totalRow = await data.CountAsync();

            var pagedResult = new PageResult<ViewProductModel>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                Items = result
            };

            return pagedResult;
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

        public async Task<List<ViewModel>> GetListRouteOfAdmin()
        {
            var query = from x in _context.RouteOfAdministrations
                        select x;
            var data = await query.Select(x => new ViewModel()
            {
                Id = x.Id,
                Name = x.Name
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
                    MininumInventory = product.MininumInventory,
                    RouteOfAdministration = new ViewModel()
                    {
                        Id = product.RouteOfAdministration.Id,
                        Name = product.RouteOfAdministration.Name
                    },
                    IsUseDose = product.IsUseDose,
                    IsManagedInBatches = product.IsManagedInBatches,
                    CreatedAt = product.CreatedAt,
                    CreatedBy = product.CreatedBy,
                    UpdatedAt = product.UpdatedAt,
                    UpdatedBy = product.UpdatedBy,
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
                product.MininumInventory = model.MininumInventory;
                product.RouteOfAdministrationId = model.RouteOfAdministrationId;
                product.IsManagedInBatches = model.IsManagedInBatches;
                product.IsUseDose = model.IsUseDose;
                product.UpdatedAt = DateTime.Now;
                product.UpdatedBy = userId;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
