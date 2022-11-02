using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.InvoiceModel;
using UtNhanDrug_BE.Models.ProductUnitModel;
using UtNhanDrug_BE.Services.ManagerService;
using UtNhanDrug_BE.Services.ProductUnitService;

namespace UtNhanDrug_BE.Services.InvoiceService
{
    public class InvoiceSvc : IInvoiceSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly IUserSvc _userSvc;
        private readonly IProductUnitPriceSvc _unitSvc;
        public InvoiceSvc(ut_nhan_drug_store_databaseContext context, IUserSvc userSvc, IProductUnitPriceSvc unitSvc)
        {
            _context = context;
            _userSvc = userSvc;
            _unitSvc = unitSvc;
        }
        public async Task<bool> CreateInvoice(int UserId, CreateInvoiceModel model)
        {
            if (model.CustomerId == null)
            {
                var customer =  await _userSvc.CreateCustomer(UserId, model.Customer);
                if(customer != null)
                {
                    model.CustomerId = customer.Id;
                }
            }
            Invoice i = new Invoice()
            {
                CustomerId = model.CustomerId,
                BodyWeight = model.BodyWeight,
                DayUse = model.DayUse,
                Discount = model.Discount,
                TotalPrice = 0,
                CreatedBy = UserId
            };
            _context.Invoices.Add(i);
            await _context.SaveChangesAsync();

            foreach(OrderDetailModel x in model.Product){
                
                decimal price = 0;
                var batch = await _context.Batches.FirstOrDefaultAsync(p => p.ProductId == x.ProductId);
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == x.ProductId);
                var unit = await _context.ProductUnitPrices.FirstOrDefaultAsync(p => p.Id == x.GoodsIssueNote.Unit);
                int ConvertedQuantity = (int)(x.GoodsIssueNote.Quantity * unit.ConversionValue);
                OrderDetail o = new OrderDetail()
                {
                    InvoiceId = i.Id,
                    ProductId = x.ProductId,
                    Dose = x.Dose,
                    UnitDose = x.UnitDose,
                    Frequency = x.Frequency,
                    DayUse = x.DayUse,
                    Use = x.Use,
                };
                _context.OrderDetails.Add(o);
                await _context.SaveChangesAsync();

                if (unit.Price == null)
                {
                    var units = await _unitSvc.GetProductUnitByProductId(x.ProductId);
                    foreach(var u in units)
                    {
                        if (u.IsBaseUnit)
                        {
                            price = (decimal)(u.Price * ConvertedQuantity);
                        }
                    }
                }
                else{
                    price = (decimal)unit.Price;
                }
                GoodsIssueNote g = new GoodsIssueNote()
                {
                    GoodsIssueNoteTypeId = x.GoodsIssueNote.GoodsIssueNoteTypeId,
                    OrderDetailId = o.Id,
                    BatchId = batch.Id,
                    Quantity = x.GoodsIssueNote.Quantity,
                    Unit = unit.Unit,
                    UnitPrice = price,
                    ConvertedQuantity = ConvertedQuantity
                };

                _context.GoodsIssueNotes.Add(g);
                o.TotalPrice = g.UnitPrice * g.Quantity;
                i.TotalPrice = i.TotalPrice + o.TotalPrice;
            }
            if (await _context.SaveChangesAsync() != 0) return true;
            return false;
        }
    }
}
