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
using UtNhanDrug_BE.Models.BatchModel;
using Microsoft.EntityFrameworkCore.Storage;

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
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                Product product = new Product()
                {
                    DrugRegistrationNumber = model.DrugRegistrationNumber,
                    Name = model.Name,
                    Barcode = model.DrugRegistrationNumber,
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
                    product.Barcode = GenaralBarcode.CreateEan13Product(product.Id + "");
                    await _context.SaveChangesAsync();
                    ProductUnitPrice pu = new ProductUnitPrice()
                    {
                        ProductId = product.Id,
                        Unit = model.Unit,
                        ConversionValue = 1,
                        Price = model.Price,
                        IsBaseUnit = true,
                        IsPackingSpecification = model.IsPackingSpecification,
                        IsDoseBasedOnBodyWeightUnit = model.IsDoseBasedOnBodyWeightUnit,
                        CreatedBy = userId
                    };
                    _context.ProductUnitPrices.Add(pu);

                    if (model.ProductUnits != null)
                    {
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
                                CreatedBy = userId
                            };
                            _context.ProductUnitPrices.Add(x);
                        }
                    }
                    if (model.ActiveSubstances != null)
                    {
                        foreach (var p in model.ActiveSubstances)
                        {
                            ProductActiveSubstance pas = new ProductActiveSubstance()
                            {
                                ProductId = product.Id,
                                ActiveSubstanceId = p
                            };
                            product.ProductActiveSubstances.Add(pas);
                        }
                    }
                    var r = await _context.SaveChangesAsync();
                    if (r > 0)
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
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


            var result = await query.Select(p => new ViewProductModel()
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

            return result;
        }

        public async Task<PageResult<ViewProductModel>> GetProductFilter(ProductFilterRequest request)
        {
            var query = from p in _context.Products
                        join pas in _context.ProductActiveSubstances on p.Id equals pas.ProductId
                        join b in _context.Batches on p.Id equals b.ProductId
                        where p.Name.Contains(request.SearchValue) || p.Barcode.Contains(request.SearchValue) || pas.ActiveSubstance.Name.Equals(request.SearchValue) || b.BatchBarcode.Contains(request.SearchValue)
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
                CreatedBy = new ViewModel()
                {
                    Id = p.CreatedByNavigation.Id,
                    Name = p.CreatedByNavigation.FullName
                },
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
            var query = from p in _context.Products
                        where p.Id == id
                        select  p ;
            var data = await query.Select(x => new ViewProductModel()
            {
                Id = x.Id,
                DrugRegistrationNumber = x.DrugRegistrationNumber,
                Barcode = x.Barcode,
                Name = x.Name,
                Brand = new ViewModel()
                {
                    Id = x.Brand.Id,
                    Name = x.Brand.Name
                },
                Shelf = new ViewModel()
                {
                    Id = x.Shelf.Id,
                    Name = x.Shelf.Name
                },
                MininumInventory = x.MininumInventory,
                RouteOfAdministration = new ViewModel()
                {
                    Id = x.RouteOfAdministration.Id,
                    Name = x.RouteOfAdministration.Name
                },
                IsUseDose = x.IsUseDose,
                IsManagedInBatches = x.IsManagedInBatches,
                CreatedAt = x.CreatedAt,
                CreatedBy = new ViewModel()
                {
                    Id = x.CreatedByNavigation.Id,
                    Name = x.CreatedByNavigation.FullName
                },
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsActive = x.IsActive,
            }).FirstOrDefaultAsync();
            return data;
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

        public async Task<List<ViewBatchModel>> GetBatchByProductId(int id)
        {
            var query = from x in _context.Batches
                        where x.ProductId == id
                        select x;
            var data = await query.Select(b => new ViewBatchModel()
            {
                Id = b.Id,
                BatchBarcode = b.BatchBarcode,
                Product = new ViewModel()
                {
                    Id = b.Product.Id,
                    Name = b.Product.Name
                },
                ManufacturingDate = b.ManufacturingDate,
                ExpiryDate = b.ExpiryDate,
                IsActive = b.IsActive,
                CreatedAt = b.CreatedAt,
                CreatedBy = new ViewModel()
                {
                    Id = b.CreatedByNavigation.Id,
                    Name = b.CreatedByNavigation.FullName
                },
            }).ToListAsync();
            return data;
        }

        //public async Task<PageResult<PageResult<ViewProductModel>>> GetProductPaging(ProductPagingRequest request)
        //{
        //    var query = from p in _context.Products
                        
        //    //filter
        //    if (!string.IsNullOrEmpty(request.keyword))
        //        query = query.Where(x => x.c.CampaignName.Contains(request.keyword));

        //    if (request.DonationCaseId != null && request.DonationCaseId != 0)
        //    {
        //        query = query.Where(p => p.dc.DonationCaseId == request.DonationCaseId);
        //    }
        //    //paging
        //    int totalRow = await query.CountAsync();

        //    var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
        //        .Take(request.PageSize)
        //        .Select(x => new CampaignViewModel()
        //        {
        //            CampaignId = x.c.CampaignId,
        //            CampaignName = x.c.CampaignName,
        //            CardNumber = x.c.CardNumber,
        //            DateCreate = (DateTime)x.c.DateCreate,
        //            Description = x.c.Description,
        //            DonationCaseId = (int)x.c.DonationCaseId,
        //            Image = _storageService.GetFileUrl(x.c.Image),
        //            OrganizationId = (int)x.c.OrganizationId,
        //            Title = x.c.Title,
        //            Goal = (double)x.c.Goal
        //        }).ToListAsync();

        //    // select and projection
        //    var pagedResult = new PageResult<ViewProductModel>()
        //    {
        //        TotalRecord = totalRow,
        //        PageSize = request.PageSize,
        //        PageIndex = request.PageIndex,
        //        Items = data
        //    };
        //    return pagedResult;
        //}
    }
}
