﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.BrandModel;
using System.Linq;
using System;
using UtNhanDrug_BE.Models.ProductModel;

namespace UtNhanDrug_BE.Services.BrandService
{
    public class BrandSvc : IBrandSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;

        public BrandSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateBrand(int userId, CreateBrandModel model)
        {
            Brand brand = new Brand()
            {
                Name = model.Name,
                CreatedBy = userId,
            };
            _context.Brands.Add(brand);
            var result = await _context.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }

        public async Task<bool> UpdateBrand(int brandId,int userId, UpdateBrandModel model)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == brandId);
            if(brand != null)
            {
                brand.Name = model.Name;
                brand.UpdatedAt = DateTime.Now;
                brand.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteBrand(int brandId, int userId)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == brandId);
            if(brand != null)
            {
                brand.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<ViewBrandModel> GetBrandById(int brandId)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == brandId);
            if (brand != null)
            {
                ViewBrandModel result = new ViewBrandModel()
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    IsActive = brand.IsActive,
                    CreatedAt = brand.CreatedAt,
                    CreatedBy = brand.CreatedBy,
                    UpdatedAt = brand.UpdatedAt,
                    UpdatedBy = brand.UpdatedBy
                };
                return result;
            }
            return null;
        }

        public async Task<List<ViewBrandModel>> GetAllBrand()
        {
            var query = from b in _context.Brands
                        select b;

            var result = await query.Select(b => new ViewBrandModel()
            {
                Id = b.Id,
                Name = b.Name,
                CreatedAt = b.CreatedAt,
                CreatedBy = b.CreatedBy,
                UpdatedAt = b.UpdatedAt,
                UpdatedBy = b.UpdatedBy,
                IsActive = b.IsActive,
            }).ToListAsync();

            return result;
        }

        public async Task<bool> CheckBrand(int brandId)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == brandId);
            if (brand != null) return true;
            return false;
        }

        public async Task<List<ViewProductModel>> GetListProduct(int brandId)
        {
            var query = from p in _context.Products
                        where p.BrandId == brandId
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

            return data;
        }
    }
}