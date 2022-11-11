using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.BatchModel;
using System.Linq;
using UtNhanDrug_BE.Hepper.GenaralBarcode;
using System;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.ResponseModel;
using UtNhanDrug_BE.Models.GoodsReceiptNoteModel;
using Microsoft.EntityFrameworkCore.Storage;

namespace UtNhanDrug_BE.Services.BatchService
{
    public class BatchSvc : IBatchSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;

        public BatchSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckBatch(int id)
        {
            var result = await _context.Batches.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null) return true;
            return false;
        }

        public async Task<Response<bool>> CreateBatch(int userId, CreateBatchModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                Batch consignment = new Batch()
                {
                    BatchBarcode = "#####",
                    ProductId = model.ProductId,
                    ManufacturingDate = model.ManufacturingDate,
                    ExpiryDate = model.ExpiryDate,
                    CreatedBy = userId,
                };
                _context.Batches.Add(consignment);
                await _context.SaveChangesAsync();
                consignment.BatchBarcode = GenaralBarcode.CreateEan13Batch(consignment.Id + "");
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new Response<bool>(true)
                {
                    Message = "Tạo lô hàng thành công"
                };
            }catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tạo lô hàng thất bại"
                };
            }
        }

        public async Task<Response<bool>> DeleteBatch(int id, int userId)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var result = await _context.Batches.FirstOrDefaultAsync(x => x.Id == id);
                if (result != null)
                {
                    if(result.IsActive == false)
                    {
                        result.IsActive = true;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Lô hàng đang hoạt động"
                        };
                    }
                    else
                    {
                        result.IsActive = false;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Lô hàng ngưng hoạt động"
                        };
                    }
                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy lô hàng này"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    Message = "Xoá lô hàng thất bại"
                };
            }
        }

        public async Task<Response<List<ViewBatchModel>>> GetAllBatch()
        {
            try
            {
                var query = from c in _context.Batches
                            select c;
                var data = await query.OrderBy(x => x.ExpiryDate).Select(x => new ViewBatchModel()
                {
                    Id = x.Id,
                    BatchBarcode = x.BatchBarcode,
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
                foreach (var x in data)
                {
                    var currentQuantity = await GetCurrentQuantity(x.Id);
                    x.CurrentQuantity = currentQuantity;
                }
                if(data.Count > 0)
                {
                    return new Response<List<ViewBatchModel>>(data)
                    {
                        Message = "Thông tin tất cả lô hàng"
                    };
                }
                else
                {
                    return new Response<List<ViewBatchModel>>(null)
                    {
                        Message = "Không tồn tại lô hàng nào"
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

        public async Task<Response<List<ViewBatchModel>>> GetBatchesByProductId(int id)
        {
            try
            {
                var query = from b in _context.Batches
                            where b.ProductId == id
                            select b;
                var data = await query.OrderBy(x => x.ExpiryDate).Select(x => new ViewBatchModel()
                {
                    Id = x.Id,
                    BatchBarcode = x.BatchBarcode,
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
                foreach (var x in data)
                {
                    var currentQuantity = await GetCurrentQuantity(x.Id);
                    x.CurrentQuantity = currentQuantity;
                }
                if(data.Count > 0)
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

        public async Task<Response<ViewBatchModel>> GetBatchById(int id)
        {
            try
            {
                var query = from b in _context.Batches
                            where b.Id == id
                            select b;
                var data = await query.Select(x => new ViewBatchModel()
                {
                    Id = x.Id,
                    BatchBarcode = x.BatchBarcode,
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
                }).FirstOrDefaultAsync();
                if(data != null)
                {
                    var currentQuantity = await GetCurrentQuantity(data.Id);
                    data.CurrentQuantity = currentQuantity;
                    return new Response<ViewBatchModel>(data)
                    {
                        Message = "Thông tin lô hàng"
                    };
                }
                else
                {
                    return new Response<ViewBatchModel>(null)
                    {
                        StatusCode = 400,
                        Message = "Lô hàng không tồn tại"
                    };
                }
            }
            catch (Exception)
            {
                return new Response<ViewBatchModel>(null)
                {
                    Message = "Đã có lỗi xảy ra",
                    StatusCode = 400
                };
            }
            
        }

        public async Task<Response<bool>> UpdateBatch(int id, int userId, UpdateBatchModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var c = await _context.Batches.FirstOrDefaultAsync(x => x.Id == id);
                if (c != null)
                {
                    c.ProductId = model.ProductId;
                    c.ManufacturingDate = model.ManufacturingDate;
                    c.ExpiryDate = model.ExpiryDate;
                    c.UpdatedAt = DateTime.Now;
                    c.UpdatedBy = userId;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Cập nhật lô hàng thành công"
                    };
                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy lô hàng này"
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

        //quantity
        private async Task<List<ViewQuantityModel>> GetCurrentQuantity(int batchId)
        {
            var query = from g in _context.GoodsReceiptNotes
                        where g.BatchId == batchId
                        select g;
            var totalQuantity = await query.Select(x => x.ConvertedQuantity).SumAsync();

            var productId = await query.Select(x => x.Batch.ProductId).SumAsync();
            var query2 = from g in _context.GoodsIssueNotes
                         where g.BatchId == batchId
                         select g;
            var saledQuantity = await query2.Select(x => x.ConvertedQuantity).SumAsync();
            var currentQuantity = totalQuantity - saledQuantity;

            var query3 = from u in _context.ProductUnitPrices
                         where u.ProductId == productId
                         select u;
            var data = await query3.Select(x => new ViewQuantityModel()
            {
                Id = x.Id,
                Unit = x.Unit,
                CurrentQuantity = (int)(currentQuantity /x.ConversionValue)
            }).ToListAsync();
            return data;
        }

        public async Task<Response<ViewBatchModel>> GetBatchesByBarcode(string barcode)
        {
            var query = from b in _context.Batches
                        where b.BatchBarcode.Equals(barcode)
                        select b;
            var data = await query.Select(x => new ViewBatchModel()
            {
                Id = x.Id,
                BatchBarcode = x.BatchBarcode,
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
            }).FirstOrDefaultAsync();
            if(data != null)
            {
                var currentQuantity = await GetCurrentQuantity(data.Id);
                data.CurrentQuantity = currentQuantity;
                return new Response<ViewBatchModel>(data)
                {
                    Message = "Successfully"
                };
            }
            
            return new Response<ViewBatchModel>(data)
            {
                StatusCode = 400,
                Message = "Not found this batch"
            };
        }

        public async Task<Response<List<ViewGoodsReceiptNoteModel>>> GetGRNByBatchId(int id)
        {
            var query = from grn in _context.GoodsReceiptNotes
                        where grn.BatchId == id
                        select grn;
            var data = await query.Select(x => new ViewGoodsReceiptNoteModel()
            {
                Id = x.Id,
                GoodsReceiptNoteType = new ViewModel()
                {
                    Id = x.GoodsReceiptNoteType.Id,
                    Name = x.GoodsReceiptNoteType.Name
                },
                Batch = new ViewBatch()
                {
                    Id = x.Batch.Id,
                    Barcode = x.Batch.BatchBarcode,
                    ManufacturingDate = x.Batch.ManufacturingDate,
                    ExpiryDate = x.Batch.ExpiryDate
                },
                Supplier = new ViewModel()
                {
                    Id = x.Supplier.Id,
                    Name = x.Supplier.Name
                },
                Quantity = x.Quantity,
                Unit = x.Unit,
                InvoiceId = x.InvoiceId,
                ConvertedQuantity = x.ConvertedQuantity,
                TotalPrice = x.TotalPrice,
                BaseUnitPrice = x.BaseUnitPrice,
                CreatedBy = new ViewModel()
                {
                    Id = x.CreatedByNavigation.Id,
                    Name = x.CreatedByNavigation.FullName
                },
                CreatedAt = x.CreatedAt,
            }).ToListAsync();

            if(data.Count == 0){
                return new Response<List<ViewGoodsReceiptNoteModel>>(data)
                {
                    StatusCode = 400,
                    Message = "Không có phiếu nhập hàng cho lô sản phẩm này"
                };
            };
            return new Response<List<ViewGoodsReceiptNoteModel>>(data)
            {
                Message = "Thành công"
            };
        }

        public async Task<Response<List<ViewQuantityInventoryModel>>> GetInventoryByUnitId(int unitId)
        {
            var query = from pup in _context.ProductUnitPrices
                        where pup.Id == unitId
                        select pup;
            var data = await query.Select(x => new ViewModel()
            {
                Id = x.ProductId,
                Name = x.Product.Name
            }).FirstOrDefaultAsync();

            if (data == null)
            {
                return new Response<List<ViewQuantityInventoryModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Không tồn tại đơn vị này"
                };
            }

            var query1 = from b in _context.Batches
                         where b.ProductId == data.Id
                         select b;
            var data1 = await query1.Select(xx => new ViewQuantityInventoryModel()
            {
                BatchId = xx.Id,
                BatchBarcode = xx.BatchBarcode,
                ExpiryDate = xx.ExpiryDate,
                ManufacturingDate = xx.ManufacturingDate
            }).ToListAsync();

            if (data1.Count == 0)
            {
                return new Response<List<ViewQuantityInventoryModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Không tồn tại đơn vị này"
                };
            }

            foreach (var x in data1)
            {
                var currentQuantity = await GetCurrentQuantity(x.BatchId);
                foreach(var xx in currentQuantity)
                {
                    if(unitId == xx.Id)
                    {
                        x.Unit = xx.Unit;
                        x.CurrentQuantity = xx.CurrentQuantity;
                    }
                }
                
            }
            return new Response<List<ViewQuantityInventoryModel>>(data1);
        }
    }
}
