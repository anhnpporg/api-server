using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.GenderModel;
using System.Linq;

namespace UtNhanDrug_BE.Services.GenderService
{
    public class GenderSvc : IGenderSvc
    {
        private readonly utNhanDrugStoreManagementContext _context;

        public GenderSvc(utNhanDrugStoreManagementContext context)
        {
            _context = context;
        }
        public async Task<List<GenderViewModel>> GetGender()
        {
            var query = from g in _context.Genders
                        select new { g };

            List<GenderViewModel> result = await query.Select(x => new GenderViewModel()
            {
                Id = x.g.Id,
                Gender = x.g.Gender1.Trim()
            }).ToListAsync();
            return result;
        }

        public async Task<GenderViewModel> GetGender(int id)
        {
            var x = await _context.Genders.FirstOrDefaultAsync(x => x.Id == id);
            GenderViewModel result = new GenderViewModel()
            {
                Id = x.Id,
                Gender = x.Gender1.Trim()
            };
            return result;
        }
    }
}
