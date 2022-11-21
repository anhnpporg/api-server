using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Models.SupplierModel;
using System.Linq;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.ResponseModel;
using Microsoft.EntityFrameworkCore.Storage;
using UtNhanDrug_BE.Models.BatchModel;
using UtNhanDrug_BE.Hepper;

namespace UtNhanDrug_BE.Services.SupplierService
{
    public class SupplierSvc : ISupplierSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly DateTime today = LocalDateTime.DateTimeNow();
        public SupplierSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        //public async Task<bool> CheckSupplier(int supplierId)
        //{
        //    var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == supplierId);
        //    if (supplier != null) return true;
        //    return false;
        //}

        private async Task<bool> CheckName(string name)
        {
            var result = await _context.Suppliers.FirstOrDefaultAsync(x => x.Name == name);
            if (result == null)
            {
                return true;
            }
            return false;
        }
        public async Task<Response<bool>> CreateSupplier(int userId, CreateSupplierModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                if (await CheckName(model.Name) == false) return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tên nhà cung cấp đã tồn tại"
                };
                Supplier supplier = new Supplier()
                {
                    Name = model.Name,
                    CreatedBy = userId,
                    CreatedAt = today
                };
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new Response<bool>(true)
                {
                    Message = "Tạo nhà cung cấp thành công"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tạo nhà cung cấp thất bại"
                };
            }
            
        }

        public async Task<Response<bool>> IsDeleteSupplier(int supplierId, int userId)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == supplierId);
                if (supplier != null)
                {
                    if(supplier.IsActive == false)
                    {
                        supplier.IsActive = true;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Đã hoạt động"
                        };
                    }
                    else
                    {
                        supplier.IsActive = false;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Đã ngừng hoạt động"
                        };
                    }
                    
                }
                return new Response<bool>(false)
                {
                    Message = "Không tìm thấy nhà cung cấp này"
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

        public async Task<Response<List<ViewSupplierModel>>> GetAllSupplier()
        {
            try
            {
                var query = from s in _context.Suppliers
                            select s;

                var result = await query.OrderByDescending(x => x.CreatedAt).Select(s => new ViewSupplierModel()
                {
                    Id = s.Id,
                    Name = s.Name,
                    CreatedAt = s.CreatedAt,
                    CreatedBy = s.CreatedBy,
                    UpdatedAt = s.UpdatedAt,
                    UpdatedBy = s.UpdatedBy,
                    IsActive = s.IsActive,
                }).ToListAsync();
                if (result.Count > 0)
                {
                    return new Response<List<ViewSupplierModel>>(result);
                }
                else
                {
                    return new Response<List<ViewSupplierModel>>(null)
                    {
                        Message = "Không tìm thấy nhà cung cấp nào"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewSupplierModel>>(null)
                {
                    Message = "Đã có lỗi xảy ra"
                };
            }
            
        }

        public async Task<Response<List<ViewProductModel>>> GetListProduct(int supplierId)
        {
            try
            {
                var query = from g in _context.GoodsReceiptNotes
                            where g.SupplierId == supplierId
                            select g;
                var data = await query.Select(x => new ViewProductModel()
                {
                    Id = x.Batch.Product.Id,
                    Barcode = x.Batch.Product.Barcode,
                    Name = x.Batch.Product.Name,
                    DrugRegistrationNumber = x.Batch.Product.DrugRegistrationNumber
                }).ToListAsync();
                if (data.Count > 0)
                {
                    return new Response<List<ViewProductModel>>(data)
                    {
                        Message = "Tất cả các sản phẩm được nhập từ nhà cung cấp này"
                    };
                }
                else
                {
                    return new Response<List<ViewProductModel>>(null)
                    {
                        Message = "Không tìm thấy sản phẩm nào nhập từ nhà cung cấp này"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewProductModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Tìm kiếm sản phẩm thấy bại"
                };
            }
        }

        public async Task<Response<ViewSupplierModel>> GetSupplierById(int supplierId)
        {
            try
            {
                var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == supplierId);
                if (supplier != null)
                {
                    ViewSupplierModel result = new ViewSupplierModel()
                    {
                        Id = supplier.Id,
                        Name = supplier.Name,
                        IsActive = supplier.IsActive,
                        CreatedAt = supplier.CreatedAt,
                        CreatedBy = supplier.CreatedBy,
                        UpdatedAt = supplier.UpdatedAt,
                        UpdatedBy = supplier.UpdatedBy,
                    };
                    return new Response<ViewSupplierModel>(result)
                    {
                        Message = "Thông tin nhà cung cấp",
                    };
                }
                return new Response<ViewSupplierModel>(null)
                {
                    Message = "Không tìm thấy nhà cung cấp này",
                    StatusCode = 400
                };
            }
            catch (Exception)
            {
                return new Response<ViewSupplierModel>(null)
                {
                    Message = "Đã có lỗi xảy ra",
                    StatusCode = 400
                };
            }
            
        }

        public async Task<Response<bool>> UpdateSupplier(int supplierId, int userId, UpdateSupplierModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == supplierId);
                if (supplier != null)
                {
                    supplier.Name = model.Name;
                    supplier.UpdatedAt = DateTime.Now;
                    supplier.UpdatedBy = userId;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Cập nhật nhà cung cấp thành công",
                    };
                }
                return new Response<bool>(false)
                {
                    Message = "Không tìm thấy nhà cung cấp này",
                    StatusCode = 400
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    Message = "Đã có lỗi xảy ra",
                    StatusCode = 400
                };
            }
        }

        public async Task<Response<List<ViewBatchModel>>> GetListBatch(int supplierId)
        {
            try
            {
                var query = from g in _context.GoodsReceiptNotes
                            where g.SupplierId == supplierId
                            select g;
                var data = await query.OrderByDescending(x => x.CreatedAt).Select(x => new ViewBatchModel()
                {
                    Id = x.Batch.Id,
                    BatchBarcode = x.Batch.BatchBarcode,
                    ManufacturingDate = x.Batch.ManufacturingDate,
                    ExpiryDate = x.Batch.ExpiryDate,
                    CreatedBy = new ViewModel()
                    {
                        Id = x.Batch.CreatedBy,
                        Name = x.Batch.CreatedByNavigation.FullName
                    },
                    IsActive = x.Batch.IsActive,
                }).ToListAsync();
                if (data.Count > 0)
                {
                    return new Response<List<ViewBatchModel>>(data)
                    {
                        Message = "Tất cả các lô hàng được nhập từ nhà cung cấp này"
                    };
                }
                else
                {
                    return new Response<List<ViewBatchModel>>(null)
                    {
                        Message = "Không tìm thấy lô hàng nào nhập từ nhà cung cấp này"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewBatchModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Tìm kiếm lô hàng thấy bại"
                };
            }    
        }

        public async Task<Response<List<ViewModel>>> GetListSupplier()
        {
            try
            {
                var query = from s in _context.Suppliers
                            select s;

                var result = await query.Where(x => x.IsActive == true).OrderByDescending(x => x.CreatedAt).Select(s => new ViewModel()
                {
                    Id = s.Id,
                    Name = s.Name,
                }).ToListAsync();
                if (result.Count > 0)
                {
                    return new Response<List<ViewModel>>(result)
                    {
                        Message = "Danh sách nhà cung cấp đang hoạt động"
                    };
                }
                else
                {
                    return new Response<List<ViewModel>>(null)
                    {
                        Message = "Không tìm thấy nhà cung cấp nào"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<List<ViewModel>>(null)
                {
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }
    }
}
