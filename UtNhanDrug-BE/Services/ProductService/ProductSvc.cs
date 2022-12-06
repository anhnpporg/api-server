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
using UtNhanDrug_BE.Models.ResponseModel;
using UtNhanDrug_BE.Services.ProductUnitService;
using UtNhanDrug_BE.Services.BatchService;
using UtNhanDrug_BE.Models.ActiveSubstanceModel;
using UtNhanDrug_BE.Hepper;

namespace UtNhanDrug_BE.Services.ProductService
{
    public class ProductSvc : IProductSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly IProductUnitPriceSvc _productUnitSvc;
        private readonly IBatchSvc _batchSvc;
        private readonly DateTime today = LocalDateTime.DateTimeNow();

        public ProductSvc(ut_nhan_drug_store_databaseContext context, IProductUnitPriceSvc productUnitSvc, IBatchSvc batchSvc)
        {
            _context = context;
            _batchSvc = batchSvc;
            _productUnitSvc = productUnitSvc;
        }

        private async Task<bool> CheckDrugRegistrationNumber(string drugRegistrationNumber)
        {
            var result = await _context.Products.FirstOrDefaultAsync(x => x.DrugRegistrationNumber.ToLower() == drugRegistrationNumber.ToLower())  ;
            if (result != null) return true;
            return false;
        }

        public async Task<Response<bool>> CreateProduct(int userId, CreateProductModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var checkExit = await CheckDrugRegistrationNumber(model.DrugRegistrationNumber);
                if (checkExit == true) return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Mã đăng kí của sản phẩm bị trùng"
                };
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
                    CreatedAt = today
                };
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                //Tạo lô mặc định nếu không quản lí theo lô
                if (model.IsManagedInBatches == false)
                {
                    Batch consignment = new Batch()
                    {
                        BatchBarcode = "#####",
                        ProductId = product.Id,
                        CreatedBy = userId,
                        CreatedAt = today
                    };
                    _context.Batches.Add(consignment);
                    await _context.SaveChangesAsync();
                    consignment.BatchBarcode = GenaralBarcode.CreateEan13Batch(consignment.Id + "");
                    await _context.SaveChangesAsync();
                }

                product.Barcode = GenaralBarcode.CreateEan13Product(product.Id + "");
                await _context.SaveChangesAsync();

                ProductUnitPrice pu = new ProductUnitPrice()
                {
                    ProductId = product.Id,
                    Unit = model.Unit,
                    ConversionValue = 1,
                    Price = model.Price,
                    IsBaseUnit = true,
                    IsPackingSpecification = true,
                    IsDoseBasedOnBodyWeightUnit = false,
                    CreatedBy = userId,
                    CreatedAt = today
                };
                _context.ProductUnitPrices.Add(pu);
                await _context.SaveChangesAsync();

                if (model.IsUseDose == true)
                {
                    ProductUnitPrice du = new ProductUnitPrice()
                    {
                        ProductId = product.Id,
                        Unit = model.DoseUnitPrice.DoseUnit,
                        ConversionValue = 1,
                        IsBaseUnit = false,
                        IsPackingSpecification = true,
                        IsDoseBasedOnBodyWeightUnit = true,
                        CreatedBy = userId,
                        CreatedAt = today
                    };
                    _context.ProductUnitPrices.Add(du);
                    await _context.SaveChangesAsync();
                }
                //else
                //{
                //    await transaction.RollbackAsync();
                //    return new Response<bool>(false)
                //    {
                //        StatusCode = 400,
                //        Message = "Đơn vị tính không hợp lệ"
                //    };
                //}
                //_context.ProductUnitPrices.Add(du);
                var unit = await _context.ProductUnitPrices.Where(x => x.ProductId == product.Id).ToListAsync();
                if (model.ProductUnits != null)
                {
                    foreach (var productUnit in model.ProductUnits)
                    {
                        foreach (var unitPrice in unit)
                        {
                            if (unitPrice.Unit == productUnit.Unit) return new Response<bool>(false) { StatusCode = 400, Message = "Đơn vị này đã tồn tại" };
                        }

                        ProductUnitPrice x = new ProductUnitPrice()
                        {
                            ProductId = product.Id,
                            Unit = productUnit.Unit,
                            ConversionValue = productUnit.ConversionValue,
                            Price = productUnit.Price,
                            IsBaseUnit = false,
                            IsDoseBasedOnBodyWeightUnit = false,
                            IsPackingSpecification = true,
                            CreatedBy = userId,
                            CreatedAt = today
                        };
                        _context.ProductUnitPrices.Add(x);
                    }
                    await _context.SaveChangesAsync();
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
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new Response<bool>(true)
                {
                    Message = "Tạo sản phẩm thành công"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tạo sản phẩm thất bại"
                };
            }
        }

        public async Task<Response<bool>> DeleteProduct(int id, int userId)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (product != null)
                {
                    if (product.IsActive == true)
                    {
                        product.IsActive = false;
                        product.UpdatedAt = today;
                        product.UpdatedBy = userId;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Đã ngưng hoạt động sản phẩm"
                        };
                    }
                    else
                    {
                        product.IsActive = true;
                        product.UpdatedAt = today;
                        product.UpdatedBy = userId;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Sản phẩm đang hoạt động"
                        };
                    }

                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy sản phẩm này"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Xoá sản phẩm thất bại"
                };
            }

        }

        public async Task<Response<List<ViewProductModel>>> GetAllProduct()
        {
            try
            {
                var query = from p in _context.Products
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
                if (data != null)
                {
                    foreach (var product in data)
                    {
                        var activeSubstance = await GetListActiveSubstances(product.Id);
                        product.ActiveSubstances = activeSubstance.Data;
                        var productUnits = await _productUnitSvc.GetProductUnitByProductId(product.Id);
                        product.ProductUnits = productUnits.Data;
                        var batches = await GetBatchesByProductId(new SearchBatchRequest { ProductId = product.Id});
                        product.Batches = batches.Data;
                    }
                    return new Response<List<ViewProductModel>>(data)
                    {
                        Message = "Thông tin tất cả sản phẩm"
                    };
                }
                else
                {
                    return new Response<List<ViewProductModel>>(null)
                    {
                        Message = "Hiện không có sản phẩm nào"
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

        public async Task<Response<List<ViewBatchModel>>> GetBatchesByProductId(SearchBatchRequest request)
        {
            try
            {
                var query = from b in _context.Batches
                            where b.ProductId == request.ProductId
                            select b;
                var data = await query.OrderBy(x => x.ExpiryDate).Select(x => new ViewBatchModel()
                {
                    Id = x.Id,
                    BatchBarcode = x.Barcode,
                    Product = new ViewModel()
                    {
                        Id = x.Product.Id,
                        Name = x.Product.Name
                    },
                    ManufacturingDate = x.ManufacturingDate,
                    ExpiryDate = x.ExpiryDate,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = new ViewModel()
                    {
                        Id = x.CreatedByNavigation.Id,
                        Name = x.CreatedByNavigation.FullName
                    },
                }).ToListAsync();
                if(request.IsSale == true)
                {
                    data = data.Where(x => x.ExpiryDate > today || x.ExpiryDate == null).ToList();
                }
                foreach (var x in data)
                {
                    var currentQuantity = await GetCurrentQuantity(x.Id);
                    x.CurrentQuantity = currentQuantity;
                }
                if (data.Count > 0)
                {
                    return new Response<List<ViewBatchModel>>(data)
                    {
                        Message = "Thông tin lô hàng"
                    };
                }
                else
                {
                    return new Response<List<ViewBatchModel>>(null)
                    {
                        Message = "Không có lô hàng này"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewBatchModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<PageResult<ViewProductModel>> GetProductFilter(ProductFilterRequest request)
        {
            var query = from p in _context.Products
                        join pas in _context.ProductActiveSubstances on p.Id equals pas.ProductId
                        join b in _context.Batches on p.Id equals b.ProductId
                        where p.Name.Contains(request.SearchValue) || p.Barcode.Contains(request.SearchValue) || pas.ActiveSubstance.Name.Contains(request.SearchValue) || b.BatchBarcode.Contains(request.SearchValue)
                        select p;
            var query1 = from pa in _context.ProductActiveSubstances
                         select pa;
            var query2 = from g in _context.GoodsReceiptNotes
                         select g;
            var data1 = await query1.Where(x => x.ActiveSubstance.IsActive == false).Select(x => new ViewModel()
            {
                Id = x.Product.Id,
                Name = x.Product.Barcode
            }).Distinct().ToListAsync();

            var data2 = await query2.Where(x => x.Supplier.IsActive == false).Select(x => new ViewModel()
            {
                Id = x.Batch.Product.Id,
                Name = x.Batch.Product.Barcode
            }).Distinct().ToListAsync();

            var data = query.Distinct();
            var result = await data.Where(x => x.Brand.IsActive == true & x.IsActive == true).Select(p => new ViewProductModel()
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
            if (result.Count > 0)
            {
                //delete element has activesubstan deactive

                foreach (var p1 in data1)
                {
                    foreach (var p in result)
                    {
                        if (p.Id == p1.Id)
                        {
                            result.Remove(p);
                            break;
                        }
                    }
                }
                //delete element has supplier deactive

                foreach (var p2 in data2)
                {
                    foreach (var p in result)
                    {
                        if (p.Id == p2.Id)
                        {
                            result.Remove(p);
                            break;
                        }
                    }
                }

                //
                foreach (var product in result)
                {
                    var activeSubstance = await GetListActiveSubstances(product.Id);
                    product.ActiveSubstances = activeSubstance.Data;
                    var productUnits = await _productUnitSvc.GetProductUnitByProductId(product.Id);
                    product.ProductUnits = productUnits.Data;
                    List<ViewBatchModel> batches;
                    if (request.SearchValue.Contains("BAT"))
                    {
                        batches = new List<ViewBatchModel>();
                        var batch = await _batchSvc.GetBatchesByBarcode(request.SearchValue);
                        if(batch.Data.ExpiryDate <= today)
                        {
                            return new PageResult<ViewProductModel>()
                            {
                                Message = "Sản phẩm lô hàng này đã hết hạn sử dụng",
                                TotalRecords = 0,
                                PageSize = 0,
                                StatusCode = 400
                            };
                        }
                        if (batch.Data != null)
                        {
                            batches.Add(batch.Data);
                        }
                    }
                    else
                    {
                        var b = await GetBatchesByProductId(new SearchBatchRequest { ProductId = product.Id, IsSale = request.IsSale});
                        batches = b.Data;
                    }
                    product.Batches = batches;
                }
            }
            //paging
            int totalRow = result.Count();

            var pagedResult = new PageResult<ViewProductModel>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                Items = result,
                StatusCode = 200
            };

            return pagedResult;
        }

        private async Task<List<ViewQuantityModel>> GetCurrentQuantity(int batchId)
        {
            var ba = from b in _context.Batches
                        where b.Id == batchId
                        select b;
            var p = await ba.Select(x => x.Product).FirstOrDefaultAsync();
            var query = from g in _context.GoodsReceiptNotes
                        where g.BatchId == batchId
                        select g;
            var data2 = await query.ToListAsync();
            if(data2.Count == 0)
            {
                if (p.IsManagedInBatches == false)
                {
                    var query4 = from u in _context.ProductUnitPrices
                                 where u.ProductId == p.Id
                                 select u;
                    var data1 = await query4.Where(x => x.IsDoseBasedOnBodyWeightUnit == false).Select(x => new ViewQuantityModel()
                    {
                        Id = x.Id,
                        Unit = x.Unit,
                        UnitPrice = x.Price,
                        CurrentQuantity = 0
                    }).ToListAsync();
                    return data1;
                }
            }
            var totalQuantity = await query.Select(x => x.ConvertedQuantity).SumAsync();

            var productId = await query.Select(x => x.Batch.ProductId).FirstOrDefaultAsync();
            var query2 = from g in _context.GoodsIssueNotes
                         where g.BatchId == batchId
                         select g;
            var saledQuantity = await query2.Select(x => x.ConvertedQuantity).SumAsync();
            var currentQuantity = totalQuantity - saledQuantity;

            var query3 = from u in _context.ProductUnitPrices
                         where u.ProductId == productId
                         select u;
            var data = await query3.Where(x => x.IsDoseBasedOnBodyWeightUnit == false).Select(x => new ViewQuantityModel()
            {
                Id = x.Id,
                Unit = x.Unit,
                UnitPrice = x.Price,
                CurrentQuantity = (int)(currentQuantity / x.ConversionValue)
            }).ToListAsync();
            return data;
        }

        public async Task<Response<List<ViewATS>>> GetListActiveSubstances(int productId)
        {
            try
            {
                var query = from pas in _context.ProductActiveSubstances
                            where pas.ProductId == productId
                            select pas;
                var data = await query.Select(x => new ViewATS()
                {
                    Id = x.ActiveSubstance.Id,
                    Name = x.ActiveSubstance.Name,
                    IsActive = (bool)x.ActiveSubstance.IsActive
                }).ToListAsync();
                if (data != null)
                {
                    return new Response<List<ViewATS>>(data)
                    {
                        Message = "Thông tin hoạt chất của sản phẩm"
                    };
                }
                else
                {
                    return new Response<List<ViewATS>>(null)
                    {
                        Message = "Sản phẩm không có hoạt chất nào"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewATS>>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }

        }

        public async Task<Response<List<ViewModel>>> GetListRouteOfAdmin()
        {
            try
            {
                var query = from x in _context.RouteOfAdministrations
                            select x;
                var data = await query.Select(x => new ViewModel()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();
                if (data != null)
                {
                    return new Response<List<ViewModel>>(data)
                    {
                        Message = "Danh sách đường dùng"
                    };
                }
                else
                {
                    return new Response<List<ViewModel>>(data)
                    {
                        Message = "Không tìm thấy danh sách"
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

        public async Task<Response<ViewProductModel>> GetProductById(int id)
        {
            try
            {
                var query = from p in _context.Products
                            where p.Id == id
                            select p;
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
                if (data != null)
                {
                    var activeSubstance = await GetListActiveSubstances(data.Id);
                    data.ActiveSubstances = activeSubstance.Data;
                    var productUnits = await _productUnitSvc.GetProductUnitByProductId(data.Id);
                    data.ProductUnits = productUnits.Data;
                    var batches = await GetBatchesByProductId(new SearchBatchRequest { ProductId = data.Id});
                    data.Batches = batches.Data;
                    return new Response<ViewProductModel>(data)
                    {
                        Message = "Thông tin sản phẩm"
                    };
                }
                else
                {
                    return new Response<ViewProductModel>(null)
                    {
                        StatusCode = 400,
                        Message = "Không tìm thấy sản phẩm này"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<ViewProductModel>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<bool>> UpdateProduct(int id, int userId, UpdateProductModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
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
                    product.UpdatedAt = today;
                    product.UpdatedBy = userId;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Cập nhật sản phẩm thành công"
                    };
                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy sản phẩm này"
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

        public async Task<Response<List<ViewBatchModel>>> GetBatchByProductId(int id)
        {
            try
            {
                var query = from x in _context.Batches
                            where x.ProductId == id
                            select x;
                var data = await query.Select(b => new ViewBatchModel()
                {
                    Id = b.Id,
                    BatchBarcode = b.Barcode,
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
                if (data != null)
                {
                    if (data.Count > 0)
                    {
                        return new Response<List<ViewBatchModel>>(data)
                        {
                            Message = "Thông tin lô hàng theo sản phẩm"
                        };
                    }
                    else
                    {
                        return new Response<List<ViewBatchModel>>(null)
                        {
                            Message = "Sản phẩm này không có lô tương ứng"
                        };
                    }
                }
                else
                {
                    return new Response<List<ViewBatchModel>>(null)
                    {
                        StatusCode = 400,
                        Message = "Không tìm thấy lô hàng theo sản phẩm này"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewBatchModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<List<ViewProductModel>>> GetAllProduct(FilterProduct request)
        {
            try
            {
                var query = from p in _context.Products
                            select p;
                var query1 = from pa in _context.ProductActiveSubstances
                             select pa;
                var query2 = from g in _context.GoodsReceiptNotes
                             select g;
                //get product is active
                if (request.IsProductActive == true)
                {
                    var data1 = await query1.Where(x => x.ActiveSubstance.IsActive == false).Select(x => new ViewModel()
                    {
                        Id = x.Product.Id,
                        Name = x.Product.Barcode
                    }).Distinct().ToListAsync();

                    var data2 = await query2.Where(x => x.Supplier.IsActive == false).Select(x => new ViewModel()
                    {
                        Id = x.Batch.Product.Id,
                        Name = x.Batch.Product.Barcode
                    }).Distinct().ToListAsync();

                    var data = query.Distinct();
                    var result = await data.Where(x => x.Brand.IsActive == true & x.IsActive == true).Select(p => new ViewProductModel()
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
                    if (result.Count > 0)
                    {
                        //delete element has activesubstan deactive

                        foreach (var p1 in data1)
                        {
                            foreach (var p in result)
                            {
                                if (p.Id == p1.Id)
                                {
                                    result.Remove(p);
                                    break;
                                }
                            }
                        }
                        //delete element has supplier deactive

                        foreach (var p2 in data2)
                        {
                            foreach (var p in result)
                            {
                                if (p.Id == p2.Id)
                                {
                                    result.Remove(p);
                                    break;
                                }
                            }
                        }

                        //
                        foreach (var product in result)
                        {
                            var activeSubstance = await GetListActiveSubstances(product.Id);
                            product.ActiveSubstances = activeSubstance.Data;
                            var productUnits = await _productUnitSvc.GetProductUnitByProductId(product.Id);
                            product.ProductUnits = productUnits.Data;
                            var batches = await GetBatchesByProductId(new SearchBatchRequest { ProductId = product.Id});
                            product.Batches = batches.Data;
                        }
                        return new Response<List<ViewProductModel>>(result)
                        {
                            Message = "Thông tin tất cả sản phẩm đang hoạt động"
                        };
                    }
                    else
                    {
                        return new Response<List<ViewProductModel>>(null)
                        {
                            Message = "Hiện không có sản phẩm nào"
                        };
                    }
                }

                //get product is deactive by brand
                if (request.IsBrandActive == true)
                {
                    var data = await query.Where(x => x.Brand.IsActive == false).Select(p => new ViewProductModel()
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
                    if (data != null)
                    {
                        foreach (var product in data)
                        {
                            var activeSubstance = await GetListActiveSubstances(product.Id);
                            product.ActiveSubstances = activeSubstance.Data;
                            var productUnits = await _productUnitSvc.GetProductUnitByProductId(product.Id);
                            product.ProductUnits = productUnits.Data;
                            var batches = await GetBatchesByProductId(new SearchBatchRequest { ProductId = product.Id });
                            product.Batches = batches.Data;
                        }
                        return new Response<List<ViewProductModel>>(data)
                        {
                            Message = "Thông tin tất cả sản phẩm đang hoạt động"
                        };
                    }
                    else
                    {
                        return new Response<List<ViewProductModel>>(null)
                        {
                            Message = "Hiện không có sản phẩm nào"
                        };
                    }
                }

                //get product is deactive by active substance 
                if (request.IsActiveActiveSubstance == true)
                {
                    var data = await query1.Where(x => x.ActiveSubstance.IsActive == false).Select(p => new ViewProductModel()
                    {
                        Id = p.Product.Id,
                        DrugRegistrationNumber = p.Product.DrugRegistrationNumber,
                        Barcode = p.Product.Barcode,
                        Name = p.Product.Name,
                        Brand = new ViewModel()
                        {
                            Id = p.Product.Brand.Id,
                            Name = p.Product.Brand.Name
                        },
                        Shelf = new ViewModel()
                        {
                            Id = p.Product.Shelf.Id,
                            Name = p.Product.Shelf.Name
                        },
                        MininumInventory = p.Product.MininumInventory,
                        RouteOfAdministration = new ViewModel()
                        {
                            Id = p.Product.RouteOfAdministration.Id,
                            Name = p.Product.RouteOfAdministration.Name
                        },
                        IsUseDose = p.Product.IsUseDose,
                        IsManagedInBatches = p.Product.IsManagedInBatches,
                        CreatedAt = p.Product.CreatedAt,
                        CreatedBy = new ViewModel()
                        {
                            Id = p.Product.CreatedByNavigation.Id,
                            Name = p.Product.CreatedByNavigation.FullName
                        },
                        UpdatedAt = p.Product.UpdatedAt,
                        UpdatedBy = p.Product.UpdatedBy,
                        IsActive = p.Product.IsActive,
                    }).Distinct().ToListAsync();
                    if (data != null)
                    {
                        foreach (var product in data)
                        {
                            var activeSubstance = await GetListActiveSubstances(product.Id);
                            product.ActiveSubstances = activeSubstance.Data;
                            var productUnits = await _productUnitSvc.GetProductUnitByProductId(product.Id);
                            product.ProductUnits = productUnits.Data;
                            var batches = await GetBatchesByProductId(new SearchBatchRequest { ProductId = product.Id });
                            product.Batches = batches.Data;
                        }
                        return new Response<List<ViewProductModel>>(data)
                        {
                            Message = "Thông tin tất cả sản phẩm đang hoạt động"
                        };
                    }
                    else
                    {
                        return new Response<List<ViewProductModel>>(null)
                        {
                            Message = "Hiện không có sản phẩm nào"
                        };
                    }
                }

                //get product by supplier deactive
                if (request.IsSupplierActive == true)
                {
                    var data = await query2.Where(x => x.Supplier.IsActive == false).Select(p => new ViewProductModel()
                    {
                        Id = p.Batch.Product.Id,
                        DrugRegistrationNumber = p.Batch.Product.DrugRegistrationNumber,
                        Barcode = p.Batch.Product.Barcode,
                        Name = p.Batch.Product.Name,
                        Brand = new ViewModel()
                        {
                            Id = p.Batch.Product.Brand.Id,
                            Name = p.Batch.Product.Brand.Name
                        },
                        Shelf = new ViewModel()
                        {
                            Id = p.Batch.Product.Shelf.Id,
                            Name = p.Batch.Product.Shelf.Name
                        },
                        MininumInventory = p.Batch.Product.MininumInventory,
                        RouteOfAdministration = new ViewModel()
                        {
                            Id = p.Batch.Product.RouteOfAdministration.Id,
                            Name = p.Batch.Product.RouteOfAdministration.Name
                        },
                        IsUseDose = p.Batch.Product.IsUseDose,
                        IsManagedInBatches = p.Batch.Product.IsManagedInBatches,
                        CreatedAt = p.Batch.Product.CreatedAt,
                        CreatedBy = new ViewModel()
                        {
                            Id = p.Batch.Product.CreatedByNavigation.Id,
                            Name = p.Batch.Product.CreatedByNavigation.FullName
                        },
                        UpdatedAt = p.Batch.Product.UpdatedAt,
                        UpdatedBy = p.Batch.Product.UpdatedBy,
                        IsActive = p.Batch.Product.IsActive,
                    }).Distinct().ToListAsync();
                    if (data != null)
                    {
                        foreach (var product in data)
                        {
                            var activeSubstance = await GetListActiveSubstances(product.Id);
                            product.ActiveSubstances = activeSubstance.Data;
                            var productUnits = await _productUnitSvc.GetProductUnitByProductId(product.Id);
                            product.ProductUnits = productUnits.Data;
                            var batches = await GetBatchesByProductId(new SearchBatchRequest { ProductId = product.Id });
                            product.Batches = batches.Data;
                        }
                        return new Response<List<ViewProductModel>>(data)
                        {
                            Message = "Thông tin tất cả sản phẩm đang hoạt động"
                        };
                    }
                    else
                    {
                        return new Response<List<ViewProductModel>>(null)
                        {
                            Message = "Hiện không có sản phẩm nào"
                        };
                    }
                }
                return new Response<List<ViewProductModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Dữ liệu tìm kiếm không chính xác"
                };
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
