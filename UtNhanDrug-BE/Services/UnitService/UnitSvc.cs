using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.UnitModel;
using System.Linq;

namespace UtNhanDrug_BE.Services.UnitService
{
    public class UnitSvc : IUnitSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        public UnitSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckUnit(int id)
        {
            var result = await _context.Units.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null) return true;
            return false;
        }

        public async Task<bool> CreateUnit(CreateUnitModel model)
        {
            Unit unit = new Unit()
            {
                Name = model.Name
            };
            _context.Units.Add(unit);
            var result = await _context.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }

        public async Task<List<ViewUnitModel>> GetAllUnit()
        {
            var query = from u in _context.Units
                        select new { u };
            var list = await query.Select(x => new ViewUnitModel()
            {
                Id = x.u.Id,
                Name = x.u.Name
            }).ToListAsync();
            return list;
        }

        public async Task<ViewUnitModel> GetUnitById(int id)
        {
            var result = await _context.Units.FirstOrDefaultAsync(x => x.Id == id);
            if(result != null)
            {
                return new ViewUnitModel()
                {
                    Id = result.Id,
                    Name = result.Name
                };
            }
            return null;
        }

        public async Task<bool> UpdateUnit(int id, UpdateUnitModel model)
        {
            var result = await _context.Units.FirstOrDefaultAsync(x => x.Id == id);
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
