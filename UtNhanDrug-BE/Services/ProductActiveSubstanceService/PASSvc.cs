using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ProductActiveSubstance;
using System.Linq;

namespace UtNhanDrug_BE.Services.ProductActiveSubstanceService
{
    public class PASSvc : IPASSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        public PASSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckPAS(int id)
        {
            var result = await _context.ProductActiveSubstances.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null) return true;
            return false;
        }

        public async Task<bool> CreatePAS(CreatePASModel model)
        {
            ProductActiveSubstance pas = new ProductActiveSubstance()
            {
                ProductId = model.ProductId,
                ActiveSubstanceId = model.ActiveSubstanceId
            };
            _context.ProductActiveSubstances.Add(pas);
            var result = await _context.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }

        public async Task<List<ViewPASModel>> GetAllPAS()
        {
            var query = from pas in _context.ProductActiveSubstances
                        select new { pas };
            var list = await query.Select(x => new ViewPASModel()
            {
               Id = x.pas.Id,
               ProductId = x.pas.ProductId,
               ActiveSubstanceId = x.pas.ActiveSubstanceId
            }).ToListAsync();
            return list;
        }

        public async Task<ViewPASModel> GetPASById(int id)
        {
            var result = await _context.ProductActiveSubstances.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null)
            {
                return new ViewPASModel()
                {
                    Id = result.Id,
                    ProductId = result.ProductId,
                    ActiveSubstanceId = result.ActiveSubstanceId
                };
            }
            return null;
        }

        public async Task<bool> UpdatePAS(int id, UpdatePASModel model)
        {
            var result = await _context.ProductActiveSubstances.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null)
            {
                result.ProductId = model.ProductId;
                result.ActiveSubstanceId = model.ActiveSubstanceId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
