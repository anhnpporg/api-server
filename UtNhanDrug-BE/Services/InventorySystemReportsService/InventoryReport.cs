using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Hepper;
using UtNhanDrug_BE.Models.HandlerModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UtNhanDrug_BE.Models.ResponseModel;
using System.Collections.Generic;

namespace UtNhanDrug_BE.Services.InventorySystemReportsService
{
    public class InventoryReport : IInventoryReport
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly DateTime today = LocalDateTime.DateTimeNow();
        public InventoryReport(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }

        public async Task CheckViewNoti(int id)
        {
            var noti = await _context.InventorySystemReports.FirstOrDefaultAsync(x => x.Id == id);
            if (noti != null)
            {
                noti.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveNoti(SaveNotiRequest request)
        {

            InventorySystemReport i = new InventorySystemReport()
            {
                BatchId = request.BatchId,
                ProductId = request.ProductId,
                Title = request.Title,
                Content = request.Content,
                IsRead = false,
                CreatedAt = today
            };

            _context.InventorySystemReports.Add(i);
            await _context.SaveChangesAsync();
        }

        public async Task<Response<List<ViewNotiModel>>> ViewNoti()
        {
            var queryNoti = from i in _context.InventorySystemReports
                            group i by i.CreatedAt.Date into g
                            select new
                            {
                                Price = g.Key,
                                ProductName = (from gg in g
                                               select gg).FirstOrDefault()
                            };




            return new Response<List<ViewNotiModel>>(null)
            {
                Message = "Thông báo"
            };
        }
    }
}
