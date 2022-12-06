using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.SamplePrescriptionDetailModel;
using System.Linq;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Hepper;
using System;
using UtNhanDrug_BE.Models.ResponseModel;
using static UtNhanDrug_BE.Models.SamplePrescriptionDetailModel.SamplePrescriptionDetailViewModel;
using UtNhanDrug_BE.Services.ProfileUserService;

namespace UtNhanDrug_BE.Services.SamplePrescriptionDetailService
{
    public class SamplePrescriptionDetailSvc : ISamplePrescriptionDetailSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly IProfileUserSvc _profileUserService;

        private readonly DateTime today = LocalDateTime.DateTimeNow();

        public SamplePrescriptionDetailSvc(ut_nhan_drug_store_databaseContext context, IProfileUserSvc profileUserService)
        {
            _context = context;
            _profileUserService = profileUserService;
        }

        public async Task<SamplePrescriptionDetailForManager> MapSamplePrescriptionDetail(SamplePrescriptionDetail samplePrescriptionDetailEntity)
        {
            string errorProduct = "";
            string errorBrand = "";
            string errorActiveSubstance = "";
            string errorSupplier = "";
            string errorProductUnitPrice = "";

            // Get product from db
            var productEntity = await _context.Products.FirstOrDefaultAsync(product =>
            product.Id.Equals(samplePrescriptionDetailEntity.ProductId));

            // Check product
            if (productEntity == null)
            {
                errorProduct = "Không tìm thấy sản phẩm với mã " + productEntity.Id.ToString();
            }
            else if ((bool)!productEntity.IsActive)
            {
                errorProduct = "Sản phẩm " + productEntity.Name + " đã dừng hoạt động";
            }

            // Get brand from db
            var brandEntity = await _context.Products.FirstOrDefaultAsync(brand =>
            brand.Id.Equals(productEntity.BrandId));

            // Check brand
            if (brandEntity == null)
            {
                errorBrand = "Nhà sản xuất của sản phẩm không tồn tại";
            }
            else if ((bool)!brandEntity.IsActive)
            {
                errorBrand = "Nhà sản xuất của sản phẩm đã dừng hoạt động";
            }

            // Get productActiveSubstances from db
            var productActiveSubstancesEntity = await _context.ProductActiveSubstances.Where(productActiveSubstance =>
            productActiveSubstance.ProductId.Equals(productEntity.Id))
                .ToListAsync();

            // Check activeSubstances
            foreach (ProductActiveSubstance productActiveSubstance in productActiveSubstancesEntity)
            {
                // Get activeSubstance from db
                var activeSubstanceEntity = await _context.ActiveSubstances.FirstOrDefaultAsync(activeSubstance =>
                activeSubstance.Id.Equals(productActiveSubstance.ActiveSubstanceId));

                // Check activeSubstance
                if ((bool)!activeSubstanceEntity.IsActive)
                {
                    errorActiveSubstance = "Sản phẩm có chứa hoạt chất đang dừng hoạt động";
                    break;
                }
            }

            // Get batches from db
            var batchesEntity = await _context.Batches.Where(batch =>
            batch.ProductId.Equals(productEntity.Id))
                .ToListAsync();

            // Check batches
            foreach (Batch batchEntity in batchesEntity)
            {
                // chỉ check những lô ĐANG hoạt động và còn TỒN HÀNG
                // Check batch
                if (batchEntity.ManufacturingDate <= today)
                {
                    // Get goodsRecieptNotes from db
                    var goodsReceiptNotesEntity = await _context.GoodsReceiptNotes.Where(goodsReceiptNote =>
                    goodsReceiptNote.BatchId.Equals(batchEntity.Id)).ToListAsync();

                    int totalReciept = 0;

                    // Check goodsRecieptNotes
                    foreach (GoodsReceiptNote goodsReceiptNoteEntity in goodsReceiptNotesEntity)
                    {
                        totalReciept += goodsReceiptNoteEntity.ConvertedQuantity;
                    }

                    // Get goodsIssueNotes from db
                    var goodsIssueNotesEntity = await _context.GoodsIssueNotes.Where(goodsIssueNote =>
                    goodsIssueNote.BatchId.Equals(batchEntity.Id)).ToListAsync();

                    double totalIssue = 0;

                    // Check goodsIssueNotes
                    foreach (GoodsIssueNote goodsIssueNoteEntity in goodsIssueNotesEntity)
                    {
                        totalIssue += goodsIssueNoteEntity.ConvertedQuantity;
                    }

                    double inventory = totalReciept - totalIssue;

                    // Còn TỒN HÀNG
                    if (inventory > 0)
                    {
                        // Check goodsRecieptNotes
                        foreach (GoodsReceiptNote goodsReceiptNoteEntity in goodsReceiptNotesEntity)
                        {
                            // Get supplier from db
                            var supplierEntity = await _context.Suppliers.FirstOrDefaultAsync(supplier =>
                            supplier.Id.Equals(goodsReceiptNoteEntity.SupplierId));

                            // Check supplier
                            if ((bool)!supplierEntity.IsActive)
                            {
                                errorSupplier = "Sản phẩm có lô hàng nhập từ nhà cung cấp đang dừng hoạt động";
                                break;
                            }
                        }
                        if (errorSupplier != "") break;
                    }
                }
            }

            // Get productUnitPrice from db
            var productUnitPriceEntity = await _context.ProductUnitPrices.FirstOrDefaultAsync(productUnitPrice =>
            productUnitPrice.Id.Equals(samplePrescriptionDetailEntity.ProductUnitPriceId));

            // BR: Đơn vị tính phải THUỘC sản phẩm đã chọn và ĐANG hoạt động
            // Check productUnitPrice
            if (productUnitPriceEntity == null)
            {
                errorProductUnitPrice = "Không tìm thấy đơn vị tính với mã " + samplePrescriptionDetailEntity.ProductUnitPriceId.ToString();
            }
            else if (productEntity.Id != productUnitPriceEntity.ProductId)
            {
                errorProductUnitPrice = "Mã sản phẩm và đơn vị tính không tương thích";
            }
            else if ((bool)!productUnitPriceEntity.IsActive)
            {
                errorProductUnitPrice = "Đơn vị tính " + productUnitPriceEntity.Unit + " của sản phẩm " + productEntity.Name + " đã dừng hoạt động";
            }

            return new SamplePrescriptionDetailForManager
            {
                Id = samplePrescriptionDetailEntity.SamplePrescriptionId,
                SamplePrescriptionId = samplePrescriptionDetailEntity.SamplePrescriptionId,
                ProductId = samplePrescriptionDetailEntity.ProductId,
                Dose = samplePrescriptionDetailEntity.Dose,
                ProductUnitPriceId = samplePrescriptionDetailEntity.ProductUnitPriceId,
                Frequency = samplePrescriptionDetailEntity.Frequency,
                Use = samplePrescriptionDetailEntity.Use,
                ErrorProduct = errorProduct,
                ErrorBrand = errorBrand,
                ErrorActiveSubstance = errorActiveSubstance,
                ErrorSupplier = errorSupplier,
                ErrorProductUnitPrice = errorProductUnitPrice,
                CreatedAt = samplePrescriptionDetailEntity.CreatedAt,
                CreatedByProfile = await _profileUserService.GetProfileUser(samplePrescriptionDetailEntity.CreatedBy),
                UpdatedAt = samplePrescriptionDetailEntity.UpdatedAt,
                UpdatedByProfile = await _profileUserService.GetProfileUser(samplePrescriptionDetailEntity.UpdatedBy)
            };
        }

        public async Task<Response<List<SamplePrescriptionDetailForManager>>> GetSamplePrescriptionDetailsForManager(int samplePrescriptionId)
        {
            // Get samplePrescriptions from db
            var samplePrescriptionEntity = await _context.SamplePrescriptions.FirstOrDefaultAsync(samplePrescription =>
            samplePrescription.Id.Equals(samplePrescriptionId)
            && samplePrescription.DeletedAt == null
            && samplePrescription.DeletedBy == null);

            // Check samplePrescription
            if (samplePrescriptionEntity == null)
            {
                return new Response<List<SamplePrescriptionDetailForManager>>(null)
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy đơn mẫu với mã " + samplePrescriptionId.ToString()
                };
            }

            // Get samplePrescriptionDetails from db
            var samplePrescriptionDetailsEntity = await _context.SamplePrescriptionDetails.Where(samplePrescriptionDetail =>
            samplePrescriptionDetail.SamplePrescriptionId.Equals(samplePrescriptionId)
            && samplePrescriptionDetail.DeletedAt == null
            && samplePrescriptionDetail.DeletedBy == null)
                .ToListAsync();

            List<SamplePrescriptionDetailForManager> samplePrescriptionDetails = new List<SamplePrescriptionDetailForManager>();

            // Map view model
            foreach (SamplePrescriptionDetail samplePrescriptionDetailEntity in samplePrescriptionDetailsEntity)
            {
                samplePrescriptionDetails.Add(await MapSamplePrescriptionDetail(samplePrescriptionDetailEntity));
            }

            // Return view model
            return new Response<List<SamplePrescriptionDetailForManager>>(samplePrescriptionDetails);
        }

        public async Task<Response<List<SamplePrescriptionDetailForStaff>>> GetSamplePrescriptionDetailsForStaff(int samplePrescriptionId, SamplePrescriptionDetailFilter filter)
        {
            // Get samplePrescriptions from db
            var samplePrescriptionEntity = await _context.SamplePrescriptions.FirstOrDefaultAsync(samplePrescription =>
            samplePrescription.Id.Equals(samplePrescriptionId)
            && samplePrescription.DeletedAt == null
            && samplePrescription.DeletedBy == null);

            // Check samplePrescription
            if (samplePrescriptionEntity == null)
            {
                return new Response<List<SamplePrescriptionDetailForStaff>>(null)
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy đơn mẫu với mã " + samplePrescriptionId.ToString()
                };
            }

            // Check bodyWeight
            if (filter.BodyWeight != null)
            {
                if (filter.BodyWeight <= 0)
                {
                    return new Response<List<SamplePrescriptionDetailForStaff>>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Cân nặng khách hàng không được nhỏ hơn hoặc bằng 0"
                    };
                }
            }

            // Check dateUse
            if (filter.DateUse != null)
            {
                if (filter.DateUse <= 0)
                {
                    return new Response<List<SamplePrescriptionDetailForStaff>>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Số ngày sử dụng không được nhỏ hơn hoặc bằng 0"
                    };
                }
            }

            // Get samplePrescriptionDetails from db
            var samplePrescriptionDetailsEntity = await _context.SamplePrescriptionDetails.Where(samplePrescriptionDetail =>
            samplePrescriptionDetail.SamplePrescriptionId.Equals(samplePrescriptionId)
            && samplePrescriptionDetail.DeletedAt == null
            && samplePrescriptionDetail.DeletedBy == null)
                .ToListAsync();

            List<SamplePrescriptionDetailForStaff> samplePrescriptionDetails = new List<SamplePrescriptionDetailForStaff>();

            // Map view model
            foreach (SamplePrescriptionDetail samplePrescriptionDetailEntity in samplePrescriptionDetailsEntity)
            {
                // Check samplePrescriptionDetail
                var samplePrescriptionDetailForManager = await MapSamplePrescriptionDetail(samplePrescriptionDetailEntity);

                if (samplePrescriptionDetailForManager.ErrorProduct != ""
                        || samplePrescriptionDetailForManager.ErrorBrand != ""
                        || samplePrescriptionDetailForManager.ErrorActiveSubstance != ""
                        || samplePrescriptionDetailForManager.ErrorSupplier != ""
                        || samplePrescriptionDetailForManager.ErrorProductUnitPrice != "")
                {
                    return new Response<List<SamplePrescriptionDetailForStaff>>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Đơm mẫu " + samplePrescriptionEntity.Name + " chứa sản phẩm lỗi. Vui lòng chọn đơn mẫu khác"
                    };
                }

                // Map to view model
                SamplePrescriptionDetailForStaff samplePrescriptionDetail = new SamplePrescriptionDetailForStaff
                {
                    Id = samplePrescriptionDetailEntity.SamplePrescriptionId,
                    SamplePrescriptionId = samplePrescriptionDetailEntity.SamplePrescriptionId,
                    ProductId = samplePrescriptionDetailEntity.ProductId,
                    Use = samplePrescriptionDetailEntity.Use,
                };

                // Get product from db
                var productEntity = await _context.Products.FirstOrDefaultAsync(product =>
                product.Id.Equals(samplePrescriptionDetailEntity.ProductId));

                // Get productUnitPrice from db
                var productUnitPriceEntity = await _context.ProductUnitPrices.FirstOrDefaultAsync(productUnitPrice =>
                productUnitPrice.Id.Equals(samplePrescriptionDetailEntity.ProductUnitPriceId));

                // Check product
                // Check isUseDose
                if (!productEntity.IsUseDose) // Sản phẩm KHÔNG bán theo liều
                {
                    // Check productUnitPrice
                    // Check isDoseBasedOnBodyWeightUnit
                    if (productUnitPriceEntity.IsDoseBasedOnBodyWeightUnit) // Đơn vị tính theo cân nặng
                    {
                        return new Response<List<SamplePrescriptionDetailForStaff>>(null)
                        {
                            Succeeded = false,
                            StatusCode = 500,
                            Message = "Sản phẩm không bán theo liều, đơn vị tính theo cân nặng không hợp lệ (lỗi thiết lập từ quản lí)"
                        };
                    }

                    // Check frequency
                    if (samplePrescriptionDetailEntity.Frequency != null) // Có tần suất sử dùng
                    {
                        return new Response<List<SamplePrescriptionDetailForStaff>>(null)
                        {
                            Succeeded = false,
                            StatusCode = 500,
                            Message = "Sản phẩm không bán theo liều, tần suất sử dụng không hợp lệ (lỗi thiết lập từ quản lí)"
                        };
                    }

                    // Set quantity
                    samplePrescriptionDetail.Quantity = (int)samplePrescriptionDetailEntity.Dose;

                    // Set unit quantity
                    samplePrescriptionDetail.UnitQuantityId = samplePrescriptionDetailEntity.ProductUnitPriceId;
                }
                else // Sản phẩm bán theo liều
                {
                    // Check isDoseBasedOnBodyWeightUnit
                    if (!productUnitPriceEntity.IsDoseBasedOnBodyWeightUnit) // Đơn vị tính thông thường
                    {
                        // Check frequency
                        if (samplePrescriptionDetailEntity.Frequency == null) // Không có tần suất sử dụng
                        {
                            // Set quantity
                            samplePrescriptionDetail.Quantity = (int)samplePrescriptionDetailEntity.Dose;

                            // Set unit quantity
                            samplePrescriptionDetail.UnitQuantityId = samplePrescriptionDetailEntity.ProductUnitPriceId;
                        }
                        else // Có tần suất sử dùng
                        {
                            // Set dose per time
                            samplePrescriptionDetail.DosePerTime = samplePrescriptionDetailEntity.Dose;

                            // Set dose per day
                            samplePrescriptionDetail.DosePerDay = samplePrescriptionDetail.DosePerTime * samplePrescriptionDetailEntity.Frequency;

                            // Set unit dose
                            samplePrescriptionDetail.UnitDoseId = samplePrescriptionDetailEntity.ProductUnitPriceId;

                            // Set quantity
                            samplePrescriptionDetail.Quantity = (int)samplePrescriptionDetail.DosePerDay;

                            // Set unit quantity
                            samplePrescriptionDetail.UnitQuantityId = samplePrescriptionDetailEntity.ProductUnitPriceId;

                            // Check date use
                            if (filter.DateUse != null) // Có ngày dùng
                            {
                                // Set total dose
                                samplePrescriptionDetail.TotalDose = samplePrescriptionDetail.DosePerDay * filter.DateUse;

                                // Set quantity
                                samplePrescriptionDetail.Quantity = (int)samplePrescriptionDetail.TotalDose;
                            }
                        }
                    }
                    else // Đơn vị tính theo cân nặng
                    {
                        // Check body weight
                        if (filter.BodyWeight == null) // Không có cân nặng khách hàng
                        {
                            return new Response<List<SamplePrescriptionDetailForStaff>>(null)
                            {
                                Succeeded = false,
                                StatusCode = 400,
                                Message = "Đơn mẫu yêu cầu cân nặng của khách hàng"
                            };
                        }

                        // Check converion value
                        if (productUnitPriceEntity.ConversionValue == null) // Không có giá trị quy đổi
                        {
                            // Set dose per time
                            samplePrescriptionDetail.DosePerTime = samplePrescriptionDetailEntity.Dose * filter.BodyWeight;

                            // Set unit dose
                            samplePrescriptionDetail.UnitDoseId = samplePrescriptionDetailEntity.ProductUnitPriceId;

                            // Check frequency
                            if (samplePrescriptionDetailEntity.Frequency != null) // Có tần suất sử dụng
                            {
                                // Set dose per day
                                samplePrescriptionDetail.DosePerDay = samplePrescriptionDetail.DosePerTime * samplePrescriptionDetailEntity.Frequency;

                                // Check date use
                                if (filter.DateUse != null) // Có ngày dùng
                                {
                                    // Set total dose
                                    samplePrescriptionDetail.TotalDose = samplePrescriptionDetail.DosePerDay * filter.DateUse;
                                }
                            }
                        }
                        else // Có giá trị quy đổi
                        {
                            // Set dose per time
                            samplePrescriptionDetail.DosePerTime = samplePrescriptionDetailEntity.Dose * filter.BodyWeight;

                            // Set unit dose
                            samplePrescriptionDetail.UnitDoseId = samplePrescriptionDetailEntity.ProductUnitPriceId;

                            // Calculate quantity
                            samplePrescriptionDetail.Quantity = (int)Math.Ceiling((decimal)(samplePrescriptionDetail.DosePerTime / productUnitPriceEntity.ConversionValue));

                            // Get productUnitPrice is baseUnit from db
                            var productUnitPriceIsBaseUnitEntity = await _context.ProductUnitPrices.FirstOrDefaultAsync(productUnitPrice =>
                            productUnitPrice.ProductId.Equals(samplePrescriptionDetailEntity.ProductId)
                            && productUnitPrice.IsBaseUnit == true);

                            // Set unit quantity
                            samplePrescriptionDetail.UnitQuantityId = productUnitPriceIsBaseUnitEntity.Id;

                            // Check frequency
                            if (samplePrescriptionDetailEntity.Frequency != null) // Có tần suất sử dụng
                            {
                                // Set dose per day
                                samplePrescriptionDetail.DosePerDay = samplePrescriptionDetail.DosePerTime * samplePrescriptionDetailEntity.Frequency;

                                // Calculate quantity
                                samplePrescriptionDetail.Quantity = (int)Math.Ceiling((decimal)(samplePrescriptionDetail.DosePerDay / productUnitPriceEntity.ConversionValue));

                                // Check date use
                                if (filter.DateUse != null) // Có ngày dùng
                                {
                                    // Set total dose
                                    samplePrescriptionDetail.TotalDose = samplePrescriptionDetail.DosePerDay * filter.DateUse;

                                    // Calculate quantity
                                    samplePrescriptionDetail.Quantity = (int)Math.Ceiling((decimal)(samplePrescriptionDetail.TotalDose / productUnitPriceEntity.ConversionValue));
                                }
                            }
                        }
                    }
                }

                samplePrescriptionDetails.Add(samplePrescriptionDetail);
            }

            // Return view model
            return new Response<List<SamplePrescriptionDetailForStaff>>(samplePrescriptionDetails);
        }

        public async Task<Response<SamplePrescriptionDetailForManager>> MapProduct(int productId)
        {
            // Get product from db
            var productEntity = await _context.Products.FirstOrDefaultAsync(product =>
            product.Id.Equals(productId));

            // Check product
            if (productEntity == null)
            {
                return new Response<SamplePrescriptionDetailForManager>(null)
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy sản phẩm với mã " + productEntity.Id.ToString()
                };
            }
            else if ((bool)!productEntity.IsActive)
            {
                return new Response<SamplePrescriptionDetailForManager>(null)
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Message = "Sản phẩm đã dừng hoạt động"
                };
            }

            // Get brand from db
            var brandEntity = await _context.Products.FirstOrDefaultAsync(brand =>
            brand.Id.Equals(productEntity.BrandId));

            // Check brand
            if (brandEntity == null)
            {
                return new Response<SamplePrescriptionDetailForManager>(null)
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Message = "Nhà sản xuất của sản phẩm không tồn tại"
                };
            }
            else if ((bool)!brandEntity.IsActive)
            {
                return new Response<SamplePrescriptionDetailForManager>(null)
                {
                    Succeeded = false,
                    StatusCode = 400,
                    Message = "Nhà sản xuất của sản phẩm đã dừng hoạt động"
                };
            }

            // Get productActiveSubstances from db
            var productActiveSubstancesEntity = await _context.ProductActiveSubstances.Where(productActiveSubstance =>
            productActiveSubstance.ProductId.Equals(productEntity.Id))
                .ToListAsync();

            // Check activeSubstances
            foreach (ProductActiveSubstance productActiveSubstance in productActiveSubstancesEntity)
            {
                // Get activeSubstance from db
                var activeSubstanceEntity = await _context.ActiveSubstances.FirstOrDefaultAsync(activeSubstance =>
                activeSubstance.Id.Equals(productActiveSubstance.ActiveSubstanceId));

                // Check activeSubstance
                if ((bool)!activeSubstanceEntity.IsActive)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Sản phẩm có chứa hoạt chất đang dừng hoạt động"
                    };
                }
            }

            // Get batches from db
            var batchesEntity = await _context.Batches.Where(batch =>
            batch.ProductId.Equals(productEntity.Id))
                .ToListAsync();

            // Check batches
            foreach (Batch batchEntity in batchesEntity)
            {
                // chỉ check những lô đang hoạt động
                // Check batch
                if (batchEntity.ManufacturingDate <= today)
                {
                    // Get goodsRecieptNotes from db
                    var goodsReceiptNotesEntity = await _context.GoodsReceiptNotes.Where(goodsReceiptNote =>
                    goodsReceiptNote.BatchId.Equals(batchEntity.Id)).ToListAsync();

                    // Check goodsRecieptNotes
                    foreach (GoodsReceiptNote goodsReceiptNoteEntity in goodsReceiptNotesEntity)
                    {

                    }
                }
            }

            return new Response<SamplePrescriptionDetailForManager>(null);
        }

        public async Task<bool> CheckSamplePrescriptionDetailExist(int samplePrescriptionId, int productId)
        {
            // Get samplePrescriptionDetail from db
            var samplePrescriptionDetailEntity = await _context.SamplePrescriptionDetails.FirstOrDefaultAsync(samplePrescriptionDetail =>
            samplePrescriptionDetail.SamplePrescriptionId.Equals(samplePrescriptionId)
            && samplePrescriptionDetail.ProductId.Equals(productId)
            && samplePrescriptionDetail.DeletedAt == null
            && samplePrescriptionDetail.DeletedBy == null);

            return samplePrescriptionDetailEntity != null;
        }

        public async Task<Response<SamplePrescriptionDetailForManager>> CreateSamplePrescriptionDetail(SamplePrescriptionDetailForCreation newSamplePrescriptionDetail, int userAccountId)
        {
            try
            {
                // Get samplePrescriptions from db
                var samplePrescriptionEntity = await _context.SamplePrescriptions.FirstOrDefaultAsync(samplePrescription =>
                samplePrescription.Id.Equals(newSamplePrescriptionDetail.SamplePrescriptionId)
                && samplePrescription.DeletedAt == null
                && samplePrescription.DeletedBy == null);

                // Check samplePrescription
                if (samplePrescriptionEntity == null)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy đơn mẫu với mã " + newSamplePrescriptionDetail.SamplePrescriptionId.ToString()
                    };
                }

                // Get product from db
                var productEntity = await _context.Products.FirstOrDefaultAsync(product =>
                product.Id.Equals(newSamplePrescriptionDetail.ProductId));

                // BR: Thêm được mọi sản phẩm ĐANG hoạt động
                // Check product
                if ((bool)!productEntity.IsActive)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Sản phẩm " + productEntity.Name + " đã dừng hoạt động"
                    };
                }

                // Get productUnitPrices from db
                var productUnitPriceEntity = await _context.ProductUnitPrices.FirstOrDefaultAsync(productUnitPrice =>
                productUnitPrice.Id.Equals(newSamplePrescriptionDetail.ProductUnitPriceId));

                // BR: Đơn vị tính phải THUỘC sản phẩm đã chọn và ĐANG hoạt động
                // Check productUnitPrices
                if (productUnitPriceEntity == null)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy đơn vị tính với mã " + newSamplePrescriptionDetail.ProductUnitPriceId.ToString()
                    };
                }
                else if (productEntity.Id != productUnitPriceEntity.ProductId)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Mã sản phẩm và đơn vị tính không tương thích"
                    };
                }
                else if ((bool)!productUnitPriceEntity.IsActive)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Đơn vị tính " + productUnitPriceEntity.Unit + " của sản phẩm " + productEntity.Name + " đã dừng hoạt động"
                    };
                }
                // BR: CHỈ sản phẩm được bán theo liều cân nặng mới chọn được đơn vị theo cân nặng (Check lúc tạo thì k lỗi)
                else if (productUnitPriceEntity.IsDoseBasedOnBodyWeightUnit)
                {
                    if (!productEntity.IsUseDose)
                    {
                        return new Response<SamplePrescriptionDetailForManager>(null)
                        {
                            Succeeded = false,
                            StatusCode = 400,
                            Message = "Sản phẩm " + productEntity.Name + " không được bán theo liều cân nặng. Đơn vị tính " + productUnitPriceEntity.Unit + " theo cân nặng"
                        };
                    }
                }

                // BR: CHỈ TỒN TẠI một sản phẩm đã chọn trong đơn mẫu
                // Check samplePrescriptionDetail
                if (await CheckSamplePrescriptionDetailExist(newSamplePrescriptionDetail.SamplePrescriptionId, newSamplePrescriptionDetail.ProductId))
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Sản phẩm " + productEntity.Name + " đã tồn tại ở đơn mẫu " + samplePrescriptionEntity.Name
                    };
                }

                // Check dose
                if (newSamplePrescriptionDetail.Dose < 0)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Liều dùng nhỏ hơn 0. Liều dùng không được nhỏ hơn hoặc bằng 0"
                    };
                }
                else if (newSamplePrescriptionDetail.Dose == 0)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Liều dùng bằng 0. Liều dùng không được nhỏ hơn hoặc bằng 0"
                    };
                }

                // Check Frequency
                if (newSamplePrescriptionDetail.Frequency != null)
                {
                    if (newSamplePrescriptionDetail.Frequency < 0)
                    {
                        return new Response<SamplePrescriptionDetailForManager>(null)
                        {
                            Succeeded = false,
                            StatusCode = 400,
                            Message = "Tần suất nhỏ hơn 0. Tần suất sử dụng không được nhỏ hơn hoặc bằng 0"
                        };
                    }
                    else if (newSamplePrescriptionDetail.Frequency == 0)
                    {
                        return new Response<SamplePrescriptionDetailForManager>(null)
                        {
                            Succeeded = false,
                            StatusCode = 400,
                            Message = "Tần suất bằng 0. Tần suất sử dụng không được nhỏ hơn hoặc bằng 0"
                        };
                    }
                }

                // Map new data
                SamplePrescriptionDetail samplePrescriptionDetailEntity = new SamplePrescriptionDetail()
                {
                    SamplePrescriptionId = newSamplePrescriptionDetail.SamplePrescriptionId,
                    ProductId = newSamplePrescriptionDetail.ProductId,
                    Dose = newSamplePrescriptionDetail.Dose,
                    ProductUnitPriceId = newSamplePrescriptionDetail.ProductUnitPriceId,
                    Frequency = newSamplePrescriptionDetail.Frequency,
                    Use = newSamplePrescriptionDetail.Use,
                    CreatedAt = today,
                    CreatedBy = userAccountId,
                };

                // Add new data
                _context.SamplePrescriptionDetails.Add(samplePrescriptionDetailEntity);

                // Save Change
                await _context.SaveChangesAsync();

                // Map view model
                SamplePrescriptionDetailForManager createdSamplePrescriptionDetail = await MapSamplePrescriptionDetail(samplePrescriptionDetailEntity);

                // Return view model
                return new Response<SamplePrescriptionDetailForManager>(createdSamplePrescriptionDetail)
                {
                    StatusCode = 201,
                    Message = "Thêm sản phẩm vào đơn mẫu thành công"
                };
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.InnerException.Message;

                int statusCode = 500;
                string message = "Thêm sản phẩm vào đơn mẫu thất bại. Lỗi hệ thống";
                List<string> errors = new List<string> { exceptionMessage };

                //if (exceptionMessage.Contains("Cannot insert duplicate key"))
                //{
                //    statusCode = 400;
                //    message = "Tên bệnh đã tồn tại";
                //}

                return new Response<SamplePrescriptionDetailForManager>(null)
                {
                    Succeeded = false,
                    StatusCode = statusCode,
                    Message = message,
                    Errors = errors.ToArray()
                };
            }
        }

        public async Task<Response<SamplePrescriptionDetailForManager>> UpdateSamplePrescriptionDetail(int samplePrescriptionDetailId, SamplePrescriptionDetailForUpdate newSamplePrescriptionDetail, int userAccountId)
        {
            try
            {
                // Get samplePrescriptionDetail from db
                var samplePrescriptionDetailEntity = await _context.SamplePrescriptionDetails.FirstOrDefaultAsync(samplePrescriptionDetail =>
                samplePrescriptionDetail.Id.Equals(samplePrescriptionDetailId)
                && samplePrescriptionDetail.DeletedAt == null
                && samplePrescriptionDetail.DeletedBy == null);

                // Check samplePrescriptionDetail
                if (samplePrescriptionDetailEntity == null)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy chi tiết đơn mẫu với mã " + samplePrescriptionDetailId.ToString()
                    };
                }

                // Get product from db
                var productEntity = await _context.Products.FirstOrDefaultAsync(product =>
                product.Id.Equals(newSamplePrescriptionDetail.ProductId));

                // BR: Thêm được mọi sản phẩm ĐANG hoạt động
                // Check product
                if ((bool)!productEntity.IsActive)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Sản phẩm " + productEntity.Name + " đã dừng hoạt động"
                    };
                }

                // Get productUnitPrices from db
                var productUnitPriceEntity = await _context.ProductUnitPrices.FirstOrDefaultAsync(productUnitPrice =>
                productUnitPrice.Id.Equals(newSamplePrescriptionDetail.ProductUnitPriceId));

                // BR: Đơn vị tính phải THUỘC sản phẩm đã chọn và ĐANG hoạt động
                // Check productUnitPrices
                if (productUnitPriceEntity == null)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy đơn vị tính với mã " + newSamplePrescriptionDetail.ProductUnitPriceId.ToString()
                    };
                }
                else if (productEntity.Id != productUnitPriceEntity.ProductId)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Mã sản phẩm và đơn vị tính không tương thích"
                    };
                }
                else if ((bool)!productUnitPriceEntity.IsActive)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Đơn vị tính " + productUnitPriceEntity.Unit + " của sản phẩm " + productEntity.Name + " đã dừng hoạt động"
                    };
                }
                // BR: CHỈ sản phẩm được bán theo liều cân nặng mới chọn được đơn vị theo cân nặng (Check lúc tạo thì k lỗi)
                else if (productUnitPriceEntity.IsDoseBasedOnBodyWeightUnit)
                {
                    if (!productEntity.IsUseDose)
                    {
                        return new Response<SamplePrescriptionDetailForManager>(null)
                        {
                            Succeeded = false,
                            StatusCode = 400,
                            Message = "Sản phẩm " + productEntity.Name + " không được bán theo liều cân nặng. Đơn vị tính " + productUnitPriceEntity.Unit + " theo cân nặng"
                        };
                    }
                }

                // BR: CHỈ TỒN TẠI một sản phẩm đã chọn trong đơn mẫu
                // Check samplePrescriptionDetail
                if (await CheckSamplePrescriptionDetailExist(samplePrescriptionDetailEntity.SamplePrescriptionId, newSamplePrescriptionDetail.ProductId))
                {
                    // Get samplePrescriptions from db
                    var samplePrescriptionEntity = await _context.SamplePrescriptions.FirstOrDefaultAsync(samplePrescription =>
                    samplePrescription.Id.Equals(samplePrescriptionDetailEntity.SamplePrescriptionId)
                    && samplePrescription.DeletedAt == null
                    && samplePrescription.DeletedBy == null);

                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Sản phẩm " + productEntity.Name + " đã tồn tại ở đơn mẫu " + samplePrescriptionEntity.Name
                    };
                }

                // Check dose
                if (newSamplePrescriptionDetail.Dose < 0)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Liều dùng nhỏ hơn 0. Liều dùng không được nhỏ hơn hoặc bằng 0"
                    };
                }
                else if (newSamplePrescriptionDetail.Dose == 0)
                {
                    return new Response<SamplePrescriptionDetailForManager>(null)
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Message = "Liều dùng bằng 0. Liều dùng không được nhỏ hơn hoặc bằng 0"
                    };
                }

                // Check Frequency
                if (newSamplePrescriptionDetail.Frequency != null)
                {
                    if (newSamplePrescriptionDetail.Frequency < 0)
                    {
                        return new Response<SamplePrescriptionDetailForManager>(null)
                        {
                            Succeeded = false,
                            StatusCode = 400,
                            Message = "Tần suất nhỏ hơn 0. Tần suất sử dụng không được nhỏ hơn hoặc bằng 0"
                        };
                    }
                    else if (newSamplePrescriptionDetail.Frequency == 0)
                    {
                        return new Response<SamplePrescriptionDetailForManager>(null)
                        {
                            Succeeded = false,
                            StatusCode = 400,
                            Message = "Tần suất bằng 0. Tần suất sử dụng không được nhỏ hơn hoặc bằng 0"
                        };
                    }
                }

                // Map new data
                samplePrescriptionDetailEntity.ProductId = newSamplePrescriptionDetail.ProductId;
                samplePrescriptionDetailEntity.Dose = newSamplePrescriptionDetail.Dose;
                samplePrescriptionDetailEntity.ProductUnitPriceId = newSamplePrescriptionDetail.ProductUnitPriceId;
                samplePrescriptionDetailEntity.Frequency = newSamplePrescriptionDetail.Frequency;
                samplePrescriptionDetailEntity.Use = newSamplePrescriptionDetail.Use;
                samplePrescriptionDetailEntity.UpdatedAt = today;
                samplePrescriptionDetailEntity.UpdatedBy = userAccountId;

                // Update new data
                _context.SamplePrescriptionDetails.Update(samplePrescriptionDetailEntity);

                // Save Change
                await _context.SaveChangesAsync();

                // Map view model
                SamplePrescriptionDetailForManager createdSamplePrescriptionDetail = await MapSamplePrescriptionDetail(samplePrescriptionDetailEntity);

                // Return view model
                return new Response<SamplePrescriptionDetailForManager>(createdSamplePrescriptionDetail)
                {
                    StatusCode = 200,
                    Message = "Cập nhật sản phẩm trong đơn mẫu thành công"
                };
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.InnerException.Message;

                int statusCode = 500;
                string message = "Cập nhật sản phẩm trong đơn mẫu thất bại. Lỗi hệ thống";
                List<string> errors = new List<string> { exceptionMessage };

                //if (exceptionMessage.Contains("Cannot insert duplicate key"))
                //{
                //    statusCode = 400;
                //    message = "Tên bệnh đã tồn tại";
                //}

                return new Response<SamplePrescriptionDetailForManager>(null)
                {
                    Succeeded = false,
                    StatusCode = statusCode,
                    Message = message,
                    Errors = errors.ToArray()
                };
            }
        }

        async Task<Response<bool>> ISamplePrescriptionDetailSvc.DeleteSamplePrescriptionDetail(int samplePrescriptionDetailId, int userAccountId)
        {
            try
            {
                // Get samplePrescriptionDetail from db
                var samplePrescriptionDetailEntity = await _context.SamplePrescriptionDetails.FirstOrDefaultAsync(samplePrescriptionDetail =>
                samplePrescriptionDetail.Id.Equals(samplePrescriptionDetailId)
                && samplePrescriptionDetail.DeletedAt == null
                && samplePrescriptionDetail.DeletedBy == null);

                // Check samplePrescriptionDetail
                if (samplePrescriptionDetailEntity == null)
                {
                    return new Response<bool>(false)
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy chi tiết đơn mẫu với mã " + samplePrescriptionDetailId.ToString()
                    };
                }

                // Map new data
                samplePrescriptionDetailEntity.DeletedAt = today;
                samplePrescriptionDetailEntity.DeletedBy = userAccountId;

                // Delete data
                _context.SamplePrescriptionDetails.Update(samplePrescriptionDetailEntity);

                // Save Change
                await _context.SaveChangesAsync();

                // Return view model
                return new Response<bool>(true)
                {
                    StatusCode = 200,
                    Message = "Xóa sản phẩm trong đơn mẫu thành công"
                };
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.InnerException.Message;

                int statusCode = 500;
                string message = "Cập nhật sản phẩm trong đơn mẫu thất bại. Lỗi hệ thống";
                List<string> errors = new List<string> { exceptionMessage };

                //if (exceptionMessage.Contains("Cannot insert duplicate key"))
                //{
                //    statusCode = 400;
                //    message = "Tên bệnh đã tồn tại";
                //}

                return new Response<bool>(false)
                {
                    Succeeded = false,
                    StatusCode = statusCode,
                    Message = message,
                    Errors = errors.ToArray()
                };
            }

        }
    }
}
