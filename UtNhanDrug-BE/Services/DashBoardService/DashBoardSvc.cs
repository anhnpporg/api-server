﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.DashBoardModel;
using UtNhanDrug_BE.Models.ResponseModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Services.DashBoardService
{
    public class DashBoardSvc : IDashBoardSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;

        public DashBoardSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        public async Task<Response<List<RecentSalesModel>>> GetRecentSales(SearchModel request)
        {
            try
            {
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
                DateTime today = DateTime.Today;
                var query = from i in _context.Invoices
                            select i;
                var query1 = from i in _context.GoodsReceiptNotes
                             select i;
                var quantityOrderNow = await query.Where(x => x.CreatedAt >= today).CountAsync();
                double percentQuantityOrder = 0;
                var quantityOrderYesterday = await query.Where(x => x.CreatedAt >= today.AddDays(-1) & x.CreatedAt < today).CountAsync();
                if(quantityOrderYesterday > 0)
                {
                    percentQuantityOrder = ((quantityOrderNow - quantityOrderYesterday) / quantityOrderYesterday) * 100;
                }

                var turnoverNow = await query.Where(x => x.CreatedAt >= today).Select(x => x.TotalPrice).SumAsync();
                double percentTurnover = 0;
                var turnoverYesterday = await query.Where(x => x.CreatedAt >= today.AddDays(-1) & x.CreatedAt < today).Select(x => x.TotalPrice).SumAsync();
                if(turnoverYesterday > 0)
                {
                    percentTurnover = (double)(((turnoverNow - turnoverYesterday) / turnoverYesterday) * 100);
                }
                var costNow = await query1.Where(x => x.CreatedAt >= today).Select(x => x.TotalPrice).SumAsync();
                double percentCost = 0;
                var costYesterday = await query1.Where(x => x.CreatedAt >= today.AddDays(-1) & x.CreatedAt < today).Select(x => x.TotalPrice).SumAsync();
                if(costYesterday > 0)
                {
                    percentCost = (double)(((costNow - costYesterday) / costYesterday) * 100);
                }
                decimal profitNow = turnoverNow - costNow;
                double PercentProfit = 0;
                decimal profitYesterday = turnoverYesterday - costYesterday;
                if(profitYesterday > 0)
                {
                    PercentProfit = (double)(((profitNow - profitYesterday) / profitYesterday) * 100);
                }
                SaleModel saleModel = new SaleModel()
                {
                    Cost = costNow,
                    PercentCost = percentCost,
                    PercentProfit = PercentProfit,
                    PercentQuantityOrder = percentQuantityOrder,
                    PercentTurnover = percentTurnover,
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
                                    where g.OrderDetail.Invoice.CreatedAt == DateTime.Now
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
                        var month = DateTime.Now.Month;
                        var year = DateTime.Now.Year;
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
                        var year = DateTime.Now.Year;
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