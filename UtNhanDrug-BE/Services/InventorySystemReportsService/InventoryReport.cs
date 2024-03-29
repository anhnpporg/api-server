﻿using Microsoft.EntityFrameworkCore.Storage;
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

        public async Task<Response<List<ShowNotiModel>>> ViewAllNoti()
        {
            var queryNoti = _context.InventorySystemReports.AsEnumerable()
                .Where(x => x.BatchId != null & x.ProductId == null)
              .GroupBy(x => x.CreatedAt.Date)
              .OrderByDescending(x => x.Key);

            var data = queryNoti.Select(x => new ShowNotiModel()
            {
                NotiDate = x.Key,
                TitleBatch = "Có " + x.Count() + " thông báo về tình trạng lô"
            }).ToList();

            var key = data.Select(x => x.NotiDate).ToList();
            var queryNoti1 = _context.InventorySystemReports.AsEnumerable()
                .Where(x => x.ProductId != null & x.BatchId == null)
              .GroupBy(x => x.CreatedAt.Date)
              .OrderByDescending(x => x.Key);

            var data1 = queryNoti1.Select(x => new ShowNotiModel()
            {
                NotiDate = x.Key,
                TitleQuantity = "Có " + x.Count() + " thông báo về tình trạng số lượng sản phẩm"
            }).ToList();
            var key1 = data1.Select(x => x.NotiDate).ToList();
            var unionLKey = key.Union(key1).OrderByDescending(x => x.Date).ToList();

            var result = new List<ShowNotiModel>();
            foreach (var x in unionLKey)
            {
                result.Add(new ShowNotiModel() { NotiDate = x });
            }
            foreach (var x in result)
            {
                foreach (var y in data)
                {
                    if (x.NotiDate == y.NotiDate)
                    {
                        x.TitleBatch = y.TitleBatch;
                    }
                }
                foreach (var z in data1)
                {
                    if (x.NotiDate == z.NotiDate)
                    {
                        x.TitleQuantity = z.TitleQuantity;
                    }
                }
            }

            return new Response<List<ShowNotiModel>>(result);
        }


        public async Task<Response<ViewNotiModel>> ViewDetailNoti(DateTime key)
        {
            var queryNoti = _context.InventorySystemReports.AsEnumerable()
                .Where(x => x.BatchId != null & x.ProductId == null)
              .GroupBy(x => x.CreatedAt.Date)
              .OrderByDescending(x => x.Key).Where(x => x.Key == key);


            var data = queryNoti.Select(x => new ViewNotiModel()
            {
                NotiDate = x.Key,
                ListNotiBatch = new ListNoti()
                {
                    Title = "Có " + x.Count() + " thông báo về tình trạng lô",
                    IsNotReadCount = x.Where(x => x.IsRead == false).Count(),
                    ListNotification = x.Select(x => new Noti()
                    {
                        Id = x.Id,
                        BatchId = x.BatchId,
                        Title = x.Title,
                        ProductId = x.ProductId,
                        Content = x.Content,
                        IsRead = x.IsRead,
                        CreatedAt = x.CreatedAt
                    }).ToList(),
                }
            }).FirstOrDefault();
            if(data == null)
            {
                data = new ViewNotiModel()
                {
                    NotiDate = key,
                    ListNotiBatch = new ListNoti()
                    {
                        Title = "Có 0 thông báo về tình trạng lô",
                        ListNotification = null
                    }
                };
            }


            var queryNoti1 = _context.InventorySystemReports.AsEnumerable()
                .Where(x => x.ProductId != null & x.BatchId == null)
              .GroupBy(x => x.CreatedAt.Date)
              .OrderByDescending(x => x.Key).Where(x => x.Key == key);

            var data1 = queryNoti1.Select(x => new ViewNotiModel()
            {
                NotiDate = x.Key,
                ListNotiQuantity = new ListNoti()
                {
                    Title = "Có " + x.Count() + " thông báo về tình trạng số lượng sản phẩm",
                    IsNotReadCount = x.Where(x => x.IsRead == false).Count(),
                    ListNotification = x.Select(x => new Noti()
                    {
                        Id = x.Id,
                        BatchId = x.BatchId,
                        Title = x.Title,
                        ProductId = x.ProductId,
                        Content = x.Content,
                        IsRead = x.IsRead,
                        CreatedAt = x.CreatedAt
                    }).ToList(),
                }
            }).FirstOrDefault();
            if (data1 == null)
            {
                data1 = new ViewNotiModel()
                {
                    NotiDate = key,
                    ListNotiQuantity = new ListNoti()
                    {
                        Title = "Có 0 thông báo về tình trạng số lượng sản phẩm",
                        ListNotification = null
                    }
                };
            }
            //List<ViewNotiModel> result = new List<ViewNotiModel>();

            ViewNotiModel view = new ViewNotiModel()
            {
                NotiDate = key,
                ListNotiBatch = data.ListNotiBatch,
                ListNotiQuantity = data1.ListNotiQuantity
            };

            //foreach (var x in data)
            //{
            //    view.ListNotiBatch = x.ListNotiBatch;
            //}
            //foreach (var y in data1)
            //{
            //    view.ListNotiQuantity = y.ListNotiQuantity;
            //}

            //result.Add(view);

            return new Response<ViewNotiModel>(view);
        }

        public async Task<Response<List<ShowNotiModel>>> ShowFilterNoti()
        {
            var queryNoti = _context.InventorySystemReports.AsEnumerable()
                .Where(x => x.BatchId != null & x.ProductId == null)
              .GroupBy(x => x.CreatedAt.Date)
              .OrderByDescending(x => x.Key);

            var data = queryNoti.Take(5).Select(x => new ShowNotiModel()
            {
                NotiDate = x.Key,
                TitleBatch = "Có " + x.Count() + " thông báo về tình trạng lô "
            }).ToList();
            var key = data.Select(x => x.NotiDate).ToList();
            var queryNoti1 = _context.InventorySystemReports.AsEnumerable()
                .Where(x => x.ProductId != null & x.BatchId == null)
              .GroupBy(x => x.CreatedAt.Date)
              .OrderByDescending(x => x.Key);

            var data1 = queryNoti1.Take(5).Select(x => new ShowNotiModel()
            {
                NotiDate = x.Key,
                TitleQuantity = "Có " + x.Count() + " thông báo về tình trạng số lượng sản phẩm"
            }).ToList();

            var key1 = data1.Select(x => x.NotiDate).ToList();
            var unionLKey = key.Union(key1).OrderByDescending(x => x.Date).ToList().Distinct();

            var result = new List<ShowNotiModel>();
            foreach (var x in unionLKey)
            {
                result.Add(new ShowNotiModel() { NotiDate = x });
            }
            foreach (var x in result)
            {
                foreach (var y in data)
                {
                    if (x.NotiDate == y.NotiDate)
                    {
                        x.TitleBatch = y.TitleBatch;
                    }
                }
                foreach (var z in data1)
                {
                    if (x.NotiDate == z.NotiDate)
                    {
                        x.TitleQuantity = z.TitleQuantity;
                    }
                }
            }
            result.OrderBy(x => x.NotiDate);

            return new Response<List<ShowNotiModel>>(result);
        }
    }
}
