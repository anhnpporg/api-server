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
                DoseBasedOnBodyWeight = model.DoseBasedOnBodyWeight,
                FrequencyPerDay = model.FrequencyPerDay,
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
                    MinimumQuantity = b.Product.MinimumQuantity,
                    StockStrength = b.Product.StockStrength,
                    StockStrengthUnit = new ViewModel()
                    {
                        Id = b.Product.StockStrengthUnit.Id,
                        Name = b.Product.StockStrengthUnit.Name
                    },
                    RouteOfAdministration = new ViewModel()
                    {
                        Id = b.Product.RouteOfAdministration.Id,
                        Name = b.Product.RouteOfAdministration.Name
                    },
                    IsMedicine = b.Product.IsMedicine,
                    IsConsignment = b.Product.IsConsignment,
                    CreatedAt = b.Product.CreatedAt,
                    CreatedBy = new ViewModel()
                    {
                        Id = b.Product.CreatedByNavigation.Id,
                        Name = b.Product.CreatedByNavigation.UserAccount.FullName
                    },
                    IsActive = b.Product.IsActive,
                },
                Dose = b.Dose,
                DoseBasedOnBodyWeight = b.DoseBasedOnBodyWeight,
                FrequencyPerDay = b.FrequencyPerDay,
                Use = b.Use,
                CreatedAt = b.CreatedAt,
                CreatedBy = new ViewModel()
                {
                    Id = b.CreatedByNavigation.UserAccount.Id,
                    Name = b.CreatedByNavigation.UserAccount.FullName
                },
                //UpdatedAt = b.UpdatedAt,
                //UpdatedBy = new ViewModel()
                //{
                //    Id = b.UpdatedByNavigation.UserAccount.Id,
                //    Name = b.UpdatedByNavigation.UserAccount.FullName
                //},
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
                        MinimumQuantity = sp.Product.MinimumQuantity,
                        StockStrength = sp.Product.StockStrength,
                        StockStrengthUnit = new ViewModel()
                        {
                            Id = sp.Product.StockStrengthUnit.Id,
                            Name = sp.Product.StockStrengthUnit.Name
                        },
                        RouteOfAdministration = new ViewModel()
                        {
                            Id = sp.Product.RouteOfAdministration.Id,
                            Name = sp.Product.RouteOfAdministration.Name
                        },
                        IsMedicine = sp.Product.IsMedicine,
                        IsConsignment = sp.Product.IsConsignment,
                        CreatedAt = sp.Product.CreatedAt,
                        CreatedBy = new ViewModel()
                        {
                            Id = sp.Product.CreatedByNavigation.Id,
                            Name = sp.Product.CreatedByNavigation.UserAccount.FullName
                        },
                        IsActive = sp.Product.IsActive,
                    },
                    Dose = sp.Dose,
                    DoseBasedOnBodyWeight = sp.DoseBasedOnBodyWeight,
                    FrequencyPerDay = sp.FrequencyPerDay,
                    Use = sp.Use,
                    IsActive = sp.IsActive,
                    CreatedAt = sp.CreatedAt,
                    CreatedBy = new ViewModel()
                    {
                        Id = sp.CreatedByNavigation.UserAccount.Id,
                        Name = sp.CreatedByNavigation.UserAccount.FullName
                    },
                    //UpdatedAt = sp.UpdatedAt,
                    //UpdatedBy = new ViewModel()
                    //{
                    //    Id = sp.UpdatedByNavigation.UserAccount.Id,
                    //    Name = sp.UpdatedByNavigation.UserAccount.FullName
                    //}
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
                spd.DoseBasedOnBodyWeight = model.DoseBasedOnBodyWeight;
                spd.FrequencyPerDay = model.FrequencyPerDay;
                spd.Use = model.Use;
                spd.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
