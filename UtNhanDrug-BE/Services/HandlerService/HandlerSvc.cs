using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Services.FcmNotificationService;
using System.Linq;
using UtNhanDrug_BE.Hepper;
using System;
using Microsoft.EntityFrameworkCore;
using UtNhanDrug_BE.Models.FcmNoti;
using UtNhanDrug_BE.Models.HandlerModel;
using UtNhanDrug_BE.Services.InventorySystemReportsService;

namespace UtNhanDrug_BE.Services.HandlerService
{   
    public class HandlerSvc : IHandlerSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly INotificationService _noti;
        private readonly IInventoryReport _iSvc;
        private readonly DateTime today = LocalDateTime.DateTimeNow();
        public HandlerSvc(ut_nhan_drug_store_databaseContext context, INotificationService noti, IInventoryReport iSvc)
        {
            _context = context;
            _noti = noti;
            _iSvc = iSvc;
        }
        public async Task CheckExpiryBatch()
        {
            var query = from b in _context.Batches
                        select b;
            var data = await query.Where(b => b.ExpiryDate < today & b.IsActive == true).ToListAsync();
            foreach (var i in data)
            {
                var checkExit = await CheckExitNotiBatch(i.Id);
                if(checkExit == true)
                {
                    //lưu các lô hết hạn vào bảng thông báo để không thông báo lại lần sau
                    await _iSvc.SaveNoti(new SaveNotiRequest() { BatchId = i.Id, Title = "Lô đã hết hạn, vui lòng kiểm tra", Content = i.Barcode + "" });
                    i.IsActive = false;
                    await _context.SaveChangesAsync();
                    //Gửi thông báo cho manager
                    await _noti.SendNotification(new NotificationModel { Title = "Lô sản phẩm hết hạn sử dụng", Body = "Có " + data.Count() + "lô đã hết hạn, vui lòng kiểm tra" });
                }
            }
        }

        public async Task CheckExpiryBatch1()
        {
            var query = from b in _context.Batches
                        select b;
            var data = await query.Where(b => (b.ExpiryDate.Value.Month - today.Month == 3 && b.ExpiryDate.Value.Year - today.Year == 0) & b.IsActive == true).ToListAsync();
            foreach (var i in data)
            {
                var checkExit = await CheckExitNotiBatch1(i.Id);
                if (checkExit == true)
                {
                    //lưu các lô hết hạn vào bảng thông báo để không thông báo lại lần sau
                    await _iSvc.SaveNoti(new SaveNotiRequest() { BatchId = i.Id, Title = "Lô sắp hết hạn, vui lòng kiểm tra", Content = i.Barcode + "" });

                    //Gửi thông báo cho manager
                    await _noti.SendNotification(new NotificationModel { Title = "Lô sản phẩm hết hạn sử dụng", Body = "Có " + data.Count() + "lô sắp hết hạn, vui lòng kiểm tra" });
                }
            }
        }

        private async Task<bool> CheckExitNoti(int productId)
        {
            var queryNoti = from i in _context.InventorySystemReports
                            where i.ProductId == productId
                            select i;
            var data = await queryNoti.Where(x => x.CreatedAt.Date == today.Date).FirstOrDefaultAsync();
            if (data != null)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> CheckExitNotiBatch1(int batchId)
        {
            var queryNoti = from i in _context.InventorySystemReports
                            where i.BatchId == batchId & i.Batch.IsActive == true
                            select i;
            var data = await queryNoti.FirstOrDefaultAsync();
            if (data != null)
            {
                return false;
            }
            return true;
        }

        private async Task<bool> CheckExitNotiBatch(int batchId)
        {
            var queryNoti = from i in _context.InventorySystemReports
                            where i.BatchId == batchId
                            select i;
            var data = await queryNoti.Where(x => x.CreatedAt.Date == today.Date & x.Batch.IsActive == true).FirstOrDefaultAsync();
            if (data != null)
            {
                return false;
            }
            return true;
        }

        public async Task CheckQuantityOfProduct(int productId)
        {
            var queryProduct = from p in _context.Products
                               where p.Id == productId
                               select p;
            var products = await queryProduct.ToListAsync();
            foreach (var p in products)
            {
                int currentQuantity = await GetCurrentQuantityOfProduct(p.Id);
                if (currentQuantity < p.MininumInventory)
                {
                    // lưu vào bảng thông báo -> lọc trường hợp thông báo rồi
                    var checkExit = await CheckExitNoti(productId);
                    if (checkExit == true)
                    {
                        await _iSvc.SaveNoti(new SaveNotiRequest() { ProductId = p.Id, Title = "Sản phẩm dưới hạn mức tồn kho", Content = p.Name + "" });
                        NotificationModel notification = new NotificationModel { Title = "Sản phẩm dưới hạn mức tồn kho", Body = "Sản phẩm " + p.Name + " dưới hạn mức tồn kho" };
                        await _noti.SendNotification(notification);
                    }

                    // Thông báo số lượng hiện tạo dưới hạn mức tồn kho

                }
            }
        }

        public async Task CheckQuantityOfProduct()
        {
            var queryProduct = from p in _context.Products
                               select p;
            var products = await queryProduct.ToListAsync();
            foreach (var p in products)
            {
                int currentQuantity = await GetCurrentQuantityOfProduct(p.Id);
                if (currentQuantity < p.MininumInventory)
                {
                    // lưu vào bảng thông báo -> lọc trường hợp thông báo rồi

                    // Thông báo số lượng hiện tạo dưới hạn mức tồn kho
                    NotificationModel notification = new NotificationModel { Title = "Sản phẩm dưới hạn mức tồn kho", Body = "Sản phẩm " + p.Name + " dưới hạn mức tồn kho" };
                    await _noti.SendNotification(notification);
                }
            }
        }

        private async Task<int> GetCurrentQuantityOfProduct(int productId)
        {

            var query = from g in _context.GoodsReceiptNotes
                        where g.Batch.ProductId == productId
                        select g;
            var totalQuantity = await query.Where(x => x.Batch.ExpiryDate > today || x.Batch.ExpiryDate == null).Select(x => x.ConvertedQuantity).SumAsync();

            //var productId = await query.Select(x => x.Batch.ProductId).FirstOrDefaultAsync();
            var query2 = from g in _context.GoodsIssueNotes
                         where g.Batch.ProductId == productId
                         select g;
            var saledQuantity = await query2.Select(x => x.ConvertedQuantity).SumAsync();
            var currentQuantity = totalQuantity - saledQuantity;
            return currentQuantity;

            //var query3 = from u in _context.ProductUnitPrices
            //             where u.ProductId == productId
            //             select u;
            //var data = await query3.Where(x => x.IsDoseBasedOnBodyWeightUnit == false).Select(x => new ViewQuantityModel()
            //{
            //    Id = x.Id,
            //    Unit = x.Unit,
            //    UnitPrice = x.Price,
            //    CurrentQuantity = (int)(currentQuantity / x.ConversionValue)
            //}).ToListAsync();
            //return data;

        }
    }
}
