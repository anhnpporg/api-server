using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.SamplePrescriptionDetailModel;
using System.Linq;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.ProductModel;

namespace UtNhanDrug_BE.Services.SamplePrescriptionDetailService
{
    public class SamplePrescriptionDetailSvc : ISamplePrescriptionDetailSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;

        public SamplePrescriptionDetailSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckSamplePrescriptionDetail(int id)
        {
            var spd = await _context.SamplePrescriptionDetails.FirstOrDefaultAsync(x => x.Id == id);
            if (spd != null) return true;
            return false;
        }

        public async Task<bool> CreateSamplePrescriptionDetail(int userId, CreateSPDModel model)
        {
            SamplePrescriptionDetail spd = new SamplePrescriptionDetail()
            {
                SamplePrescriptionId = model.SamplePrescriptionId,
                ProductId = model.ProductId,
                Dose = model.Dose,
                ProductUnitPriceId = model.ProductUnitPriceId,
                Frequency = model.Frequency,
                Use = model.Use,
                CreatedBy = userId,
            };
            _context.SamplePrescriptionDetails.Add(spd);
            var result = await _context.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }

        public async Task<bool> DeleteSamplePrescriptionDetail(int id, int userId)
        {
            var spd = await _context.SamplePrescriptionDetails.FirstOrDefaultAsync(x => x.Id == id);
            if (spd != null)
            {
                spd.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ViewSPDModel>> GetAllSamplePrescriptionDetail()
        {
            var query = from b in _context.SamplePrescriptionDetails
                        select b;

            var result = await query.Select(b => new ViewSPDModel()
            {
                Id = b.Id,
                SamplePrescription = new SPDModel()
                {
                    Id = b.SamplePrescription.Id,
                    SamplePrescriptionId = b.SamplePrescriptionId,
                    Name = b.SamplePrescription.Name,
                    IsActive = b.SamplePrescription.IsActive
                },
                Product = new ViewProductModel()
                {
                    Id = b.Product.Id,
                    DrugRegistrationNumber = b.Product.DrugRegistrationNumber,
                    Barcode = b.Product.Barcode,
                    Name = b.Product.Name,
                    Brand = new ViewModel()
                    {
                        Id = b.Product.Brand.Id,
                        Name = b.Product.Brand.Name
                    },
                    Shelf = new ViewModel()
                    {
                        Id = b.Product.Shelf.Id,
                        Name = b.Product.Shelf.Name
                    },
                    MininumInventory = b.Product.MininumInventory,
                    RouteOfAdministration = new ViewModel()
                    {
                        Id = b.Product.RouteOfAdministration.Id,
                        Name = b.Product.RouteOfAdministration.Name
                    },
                    IsUseDose = b.Product.IsUseDose,
                    IsManagedInBatches = b.Product.IsManagedInBatches,
                    CreatedAt = b.Product.CreatedAt,
                    CreatedBy = b.Product.CreatedBy,
                    UpdatedAt = b.Product.UpdatedAt,
                    UpdatedBy = b.Product.UpdatedBy,
                    IsActive = b.Product.IsActive,
                },
                Dose = b.Dose,
                Use = b.Use,
                Frequency = b.Frequency,
                ProductUnitPriceId = b.ProductUnitPriceId,
                CreatedAt = b.CreatedAt,
                CreatedBy = b.CreatedBy,
                UpdatedAt = b.UpdatedAt,
                UpdatedBy = b.UpdatedBy,
                IsActive = b.IsActive,
            }).ToListAsync();

            return result;
        }

        public async Task<ViewSPDModel> GetSamplePrescriptionDetailById(int id)
        {
            var sp = await _context.SamplePrescriptionDetails.FirstOrDefaultAsync(x => x.Id == id);
            if (sp != null)
            {
                ViewSPDModel result = new ViewSPDModel()
                {
                    Id = sp.Id,
                    SamplePrescription = new SPDModel()
                    {
                        Id = sp.SamplePrescription.Id,
                        SamplePrescriptionId = sp.SamplePrescriptionId,
                        Name = sp.SamplePrescription.Name,
                        IsActive = sp.SamplePrescription.IsActive
                    },
                    Product = new ViewProductModel()
                    {
                        Id = sp.Product.Id,
                        DrugRegistrationNumber = sp.Product.DrugRegistrationNumber,
                        Barcode = sp.Product.Barcode,
                        Name = sp.Product.Name,
                        Brand = new ViewModel()
                        {
                            Id = sp.Product.Brand.Id,
                            Name = sp.Product.Brand.Name
                        },
                        Shelf = new ViewModel()
                        {
                            Id = sp.Product.Shelf.Id,
                            Name = sp.Product.Shelf.Name
                        },
                        MininumInventory = sp.Product.MininumInventory,
                        RouteOfAdministration = new ViewModel()
                        {
                            Id = sp.Product.RouteOfAdministration.Id,
                            Name = sp.Product.RouteOfAdministration.Name
                        },
                        IsUseDose = sp.Product.IsUseDose,
                        IsManagedInBatches = sp.Product.IsManagedInBatches,
                        CreatedAt = sp.Product.CreatedAt,
                        CreatedBy = sp.Product.CreatedBy,
                        UpdatedAt = sp.Product.UpdatedAt,
                        UpdatedBy = sp.Product.UpdatedBy,
                        IsActive = sp.Product.IsActive,
                    },
                    Dose = sp.Dose,
                    Frequency = sp.Frequency,
                    Use = sp.Use,
                    ProductUnitPriceId = sp.ProductUnitPriceId,
                    IsActive = sp.IsActive,
                    CreatedAt = sp.CreatedAt,
                    CreatedBy = sp.CreatedBy,
                    UpdatedAt = sp.UpdatedAt,
                    UpdatedBy = sp.UpdatedBy
                };
                return result;
            }
            return null;
        }

        public async Task<bool> UpdateSamplePrescriptionDetail(int id, int userId, UpdateSPDModel model)
        {
            var spd = await _context.SamplePrescriptionDetails.FirstOrDefaultAsync(x => x.Id == id);
            if (spd != null)
            {
                spd.ProductId = model.ProductId;
                spd.Dose = model.Dose;
                spd.Use = model.Use;
                spd.Frequency = model.Frequency;
                spd.UpdatedBy = userId;
                spd.ProductUnitPriceId = model.ProductUnitPriceId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
