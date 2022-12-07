using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.DashBoardModel;
using UtNhanDrug_BE.Models.ResponseModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Hepper;
using UtNhanDrug_BE.Services.HandlerService;

namespace UtNhanDrug_BE.Services.DashBoardService
{
    public class DashBoardSvc : IDashBoardSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly DateTime today = LocalDateTime.DateTimeNow();
        private readonly IHandlerSvc _handlerSvc;
        public DashBoardSvc(ut_nhan_drug_store_databaseContext context, IHandlerSvc handlerSvc)
        {
            _context = context;
            _handlerSvc = handlerSvc;
        }

        public async Task<Response<ChartModel>> GetChart(FilterChartModel request)
        {
            try
            {
                if (request.ByWeek == true)
                {
                    List<Double> data = new List<double>();
                    var query = from i in _context.Invoices
                                select i;
                    var query1 = from i in _context.GoodsReceiptNotes
                                 select i;

                    List<DateChart> dateChart = new List<DateChart>();
                    Line lineTurnover = new Line() { Name = "Doanh thu", Type = "line", Data = new List<decimal>() };
                    Line lineCost = new Line() { Name = "Chi phí", Type = "line", Data = new List<decimal>() };
                    Line lineProfit = new Line() { Name = "Lợi nhuận", Type = "line", Data = new List<decimal>() };

                    var index = CheckMonday.MondayOfWeek(today);
                    for (var i = index; i >= 0; i--)
                    {
                        decimal turnover = await query.Where(x => x.CreatedAt.Date == today.AddDays(-i).Date).Select(x => x.TotalPrice).SumAsync();

                        decimal cost = await query1.Where(x => x.CreatedAt.Date == today.AddDays(-i).Date).Select(x => x.TotalPrice).SumAsync();

                        decimal profit = turnover - cost;

                        dateChart.Add(new DateChart() { Date = today.AddDays(-i).Date });

                        lineTurnover.Data.Add(turnover);
                        lineCost.Data.Add(cost);
                        lineProfit.Data.Add( profit );
                    }
                    List<Line> listChart = new List<Line>();
                    listChart.Add(lineTurnover);
                    listChart.Add(lineCost);
                    listChart.Add(lineProfit);
                    return new Response<ChartModel>(new ChartModel { ListLine = listChart, ListDate = dateChart })
                    {
                        Message = "Thông tin"
                    };
                }
                if (request.ByMonth == true)
                {
                    List<Double> data = new List<double>();
                    var query = from i in _context.Invoices
                                select i;
                    var query1 = from i in _context.GoodsReceiptNotes
                                 select i;

                    List<DateChart> dateChart = new List<DateChart>();
                    Line lineTurnover = new Line() { Name = "Doanh thu", Type = "line", Data = new List<decimal>() };
                    Line lineCost = new Line() { Name = "Chi phí", Type = "line", Data = new List<decimal>() };
                    Line lineProfit = new Line() { Name = "Lợi nhuận", Type = "line", Data = new List<decimal>() };
                    var index = today.Day - 1;
                    for (var i = index; i >= 0; i--)
                    {
                        decimal turnover = await query.Where(x => x.CreatedAt.Date == today.AddDays(-i).Date).Select(x => x.TotalPrice).SumAsync();

                        decimal cost = await query1.Where(x => x.CreatedAt.Date == today.AddDays(-i).Date).Select(x => x.TotalPrice).SumAsync();

                        decimal profit = turnover - cost;

                        dateChart.Add(new DateChart() { Date = today.AddDays(-i).Date });

                        lineTurnover.Data.Add(turnover);
                        lineCost.Data.Add(cost);
                        lineProfit.Data.Add(profit);
                    }
                    List<Line> listChart = new List<Line>();
                    listChart.Add(lineTurnover);
                    listChart.Add(lineCost);
                    listChart.Add(lineProfit);
                    return new Response<ChartModel>(new ChartModel { ListLine = listChart, ListDate = dateChart })
                    {
                        Message = "Thông tin"
                    };
                }
                if (request.ByYear == true)
                {
                    List<Double> data = new List<double>();
                    var query = from i in _context.Invoices
                                select i;
                    var query1 = from i in _context.GoodsReceiptNotes
                                 select i;

                    List<DateChart> dateChart = new List<DateChart>();
                    Line lineTurnover = new Line() { Name = "Doanh thu", Type = "line", Data = new List<decimal>() };
                    Line lineCost = new Line() { Name = "Chi phí", Type = "line", Data = new List<decimal>() };
                    Line lineProfit = new Line() { Name = "Lợi nhuận", Type = "line", Data = new List<decimal>() };
                    var index = today.Month - 1;
                    for (var i = index; i >= 0; i--)
                    {
                        decimal turnover = await query.Where(x => x.CreatedAt.Month == today.AddMonths(-i).Month).Select(x => x.TotalPrice).SumAsync();

                        decimal cost = await query1.Where(x => x.CreatedAt.Month == today.AddMonths(-i).Month).Select(x => x.TotalPrice).SumAsync();

                        decimal profit = turnover - cost;

                        dateChart.Add(new DateChart() { Date = today.AddMonths(-i).Date });

                        lineTurnover.Data.Add(turnover);
                        lineCost.Data.Add(cost);
                        lineProfit.Data.Add(profit);
                    }
                    List<Line> listChart = new List<Line>();
                    listChart.Add(lineTurnover);
                    listChart.Add(lineCost);
                    listChart.Add(lineProfit);
                    return new Response<ChartModel>(new ChartModel { ListLine = listChart, ListDate = dateChart })
                    {
                        Message = "Thông tin"
                    };
                }

                return new Response<ChartModel>(null)
                {
                    StatusCode = 400,
                    Message = "Thông tin"
                };
            }
            catch (Exception)
            {
                return new Response<ChartModel>(null)
                {
                    StatusCode = 500,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<List<RecentSalesModel>>> GetRecentSales(SearchModel request)
        {
            try
            {
                await _handlerSvc.CheckExpiryBatch();
                if (request != null)
                {
                    if (request.ByDay == true & request.ByMonth == false & request.ByYear == false)
                    {
                        var query = from i in _context.Invoices
                                    where i.CreatedAt == DateTime.Now
                                    select i;
                        var data = await query.OrderByDescending(x => x.CreatedAt).Take(request.Size).Select(x => new RecentSalesModel()
                        {
                            Id = x.Id,
                            Customer = new ViewModel()
                            {
                                Id = x.CreatedByNavigation.Id,
                                Name = x.CreatedByNavigation.FullName
                            },
                            CreateAt = x.CreatedAt,
                            TotalPrice = x.TotalPrice
                        }).ToListAsync();
                        return new Response<List<RecentSalesModel>>(data);
                    }else if (request.ByDay == false & request.ByMonth == true & request.ByYear == false)
                    {
                        var month = DateTime.Now.Month;
                        var year = DateTime.Now.Year;
                        var query = from i in _context.Invoices
                                    where i.CreatedAt.Month == month & i.CreatedAt.Year == year
                                    select i;
                        var data = await query.OrderByDescending(x => x.CreatedAt).Take(request.Size).Select(x => new RecentSalesModel()
                        {
                            Id = x.Id,
                            Customer = new ViewModel()
                            {
                                Id = x.CreatedByNavigation.Id,
                                Name = x.CreatedByNavigation.FullName
                            },
                            CreateAt = x.CreatedAt,
                            TotalPrice = x.TotalPrice
                        }).ToListAsync();
                        return new Response<List<RecentSalesModel>>(data);
                    }
                    else if (request.ByDay == false & request.ByMonth == false & request.ByYear == true)
                    {
                        var year = DateTime.Now.Year;
                        var query = from i in _context.Invoices
                                    where i.CreatedAt.Year == year
                                    select i;
                        var data = await query.OrderByDescending(x => x.CreatedAt).Take(request.Size).Select(x => new RecentSalesModel()
                        {
                            Id = x.Id,
                            Customer = new ViewModel()
                            {
                                Id = x.CreatedByNavigation.Id,
                                Name = x.CreatedByNavigation.FullName
                            },
                            CreateAt = x.CreatedAt,
                            TotalPrice = x.TotalPrice
                        }).ToListAsync();
                        return new Response<List<RecentSalesModel>>(data);
                    }
                }
                return new Response<List<RecentSalesModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Dữ liệu tìm kiếm không hợp lệ"
                };
            }
            catch (Exception)
            {
                return new Response<List<RecentSalesModel>>(null)
                {
                    StatusCode = 500,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<SaleModel>> GetSale()
        {
            try
            {
                //DateTime todaySystem = DateTime.Today;
                //var todayVN = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(todaySystem , "SE Asia Standard Time").ToShortDateString();
                DateTime todayConvert = DateTime.ParseExact(today.ToShortDateString(), "d", null);
                var query = from i in _context.Invoices
                            select i;
                var query1 = from i in _context.GoodsReceiptNotes
                             select i;
                var quantityOrderNow = await query.Where(x => x.CreatedAt >= todayConvert).CountAsync();

                var turnoverNow = await query.Where(x => x.CreatedAt >= todayConvert).Select(x => x.TotalPrice).SumAsync();


                var costNow = await query1.Where(x => x.CreatedAt >= todayConvert).Select(x => x.TotalPrice).SumAsync();


                decimal profitNow = turnoverNow - costNow;


                SaleModel saleModel = new SaleModel()
                {
                    Cost = costNow,
                    Profit = profitNow,
                    QuantityOrder = quantityOrderNow,
                    Turnover = turnoverNow
                };
                return new Response<SaleModel>(saleModel)
                {
                    Message = "Thông tin"
                };
            }
            catch (Exception)
            {
                return new Response<SaleModel>(null)
                {
                    StatusCode = 500,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public async Task<Response<List<TopSellingModel>>> GetTopSelling(SearchModel request)
        {
            try
            {
                if (request != null)
                {
                    if (request.ByDay == true & request.ByMonth == false & request.ByYear == false)
                    {
                        var query = from g in _context.GoodsIssueNotes
                                    where g.OrderDetail.Invoice.CreatedAt.Date == today.Date
                                    select g;
                        var result = query.Select(x => x.Batch.Product).Distinct();
                        var data = await result.Take(request.Size).Select(y => new TopSellingModel()
                        {
                            ProductId = y.Id,
                            ProductName = y.Name,
                            Revenue = query.Where(x => x.Batch.ProductId == y.Id).Select(x => x.OrderDetail.TotalPrice).Sum(),
                            Sold = query.Where(x => x.Batch.ProductId == y.Id).Select(x => x.ConvertedQuantity).Sum()
                        }).OrderByDescending(x => x.Sold).ToListAsync();
                        foreach(var t in data)
                        {
                            var baseUnit = await GetBaseUnit(t.ProductId);
                            t.Unit = baseUnit.BaseUnit;
                            t.Price = baseUnit.BasePrice;
                        }
                        return new Response<List<TopSellingModel>>(data);
                    }
                    else if (request.ByDay == false & request.ByMonth == true & request.ByYear == false)
                    {
                        var month = today.Month;
                        var year = today.Year;
                        var query = from g in _context.GoodsIssueNotes
                                    where g.OrderDetail.Invoice.CreatedAt.Month == month & g.OrderDetail.Invoice.CreatedAt.Year == year
                                    select g;
                        var result = query.Select(x => x.Batch.Product).Distinct();
                        var data = await result.Take(request.Size).Select(y => new TopSellingModel()
                        {
                            ProductId = y.Id,
                            ProductName = y.Name,
                            Revenue = query.Where(x => x.Batch.ProductId == y.Id).Select(x => x.OrderDetail.TotalPrice).Sum(),
                            Sold = query.Where(x => x.Batch.ProductId == y.Id).Select(x => x.ConvertedQuantity).Sum()
                        }).OrderByDescending(x => x.Sold).ToListAsync();
                        foreach (var t in data)
                        {
                            var baseUnit = await GetBaseUnit(t.ProductId);
                            t.Unit = baseUnit.BaseUnit;
                            t.Price = baseUnit.BasePrice;
                        }
                        return new Response<List<TopSellingModel>>(data);
                    }
                    else if (request.ByDay == false & request.ByMonth == false & request.ByYear == true)
                    {
                        var year = today.Year;
                        var query = from g in _context.GoodsIssueNotes
                                    where g.OrderDetail.Invoice.CreatedAt.Year == year
                                    select g;
                        var result = query.Select(x => x.Batch.Product).Distinct();
                        var data = await result.Take(request.Size).Select(y => new TopSellingModel()
                        {
                            ProductId = y.Id,
                            ProductName = y.Name,
                            Revenue = query.Where(x => x.Batch.ProductId == y.Id).Select(x => x.OrderDetail.TotalPrice).Sum(),
                            Sold = query.Where(x => x.Batch.ProductId == y.Id).Select(x => x.ConvertedQuantity).Sum()
                        }).OrderByDescending(x => x.Sold).ToListAsync();
                        foreach (var t in data)
                        {
                            var baseUnit = await GetBaseUnit(t.ProductId);
                            t.Unit = baseUnit.BaseUnit;
                            t.Price = baseUnit.BasePrice;
                        }
                        return new Response<List<TopSellingModel>>(data);
                    }
                }
                return new Response<List<TopSellingModel>>(null)
                {
                    StatusCode = 400,
                    Message = "Dữ liệu tìm kiếm không hợp lệ"
                };
            }
            catch (Exception)
            {
                return new Response<List<TopSellingModel>>(null)
                {
                    StatusCode = 500,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }
        private async Task<ViewUnit> GetBaseUnit(int productId)
        {
            var query = from p in _context.ProductUnitPrices
                        where p.ProductId == productId
                        select p;
            var data = await query.Where(x => x.IsBaseUnit == true).Select(x => new ViewUnit()
            {
                BasePrice = (decimal)x.Price,
                BaseUnit = x.Unit
            }).FirstOrDefaultAsync();
            return data;
        }

    }
}
