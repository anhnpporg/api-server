using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.DosageUnitModel;
using System.Linq;

namespace UtNhanDrug_BE.Services.DosageUnitService
{
    public class DosageUnitSvc : IDosageUnitSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        public DosageUnitSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckDosageUnit(int id)
        {
            var result = await _context.DosageUnits.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null) return true;
            return false;
        }

        public async Task<bool> CreateDosageUnit(CreateDosageUnitModel model)
        {
            DosageUnit dosageUnit = new DosageUnit()
            {
                Name = model.Name
            };
            _context.DosageUnits.Add(dosageUnit);
            var result = await _context.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }

        public async Task<List<ViewDosageUnitModel>> GetAllDosageUnit()
        {
            var query = from du in _context.DosageUnits
                        select new { du };
            var list = await query.Select(x => new ViewDosageUnitModel()
            {
                Id = x.du.Id,
                Name = x.du.Name
            }).ToListAsync();
            return list;
        }

        public async Task<ViewDosageUnitModel> GetDosageUnitById(int id)
        {
            var result = await _context.DosageUnits.FirstOrDefaultAsync(x => x.Id == id);
            if(result != null)
            {
                return new ViewDosageUnitModel()
                {
                    Id = result.Id,
                    Name = result.Name
                };
            }
            return null;
        }

        public async Task<bool> UpdateDosageUnit(int id, UpdateDosageUnitModel model)
        {
            var result = await _context.DosageUnits.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null)
            {
                result.Name = model.Name;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
