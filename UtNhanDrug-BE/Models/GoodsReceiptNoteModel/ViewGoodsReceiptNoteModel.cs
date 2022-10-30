using System;
using System.Collections.Generic;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Models.GoodsReceiptNoteModel
{
    public class ViewGoodsReceiptNoteModel
    {
        public int Id { get; set; }
        public ViewModel GoodsReceiptNoteType { get; set; }
        public ViewBatch Batch { get; set; }
        public int? Invoice { get; set; }
        public ViewModel? Supplier { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal TotalPrice { get; set; }
        public int ConvertedQuantity { get; set; }
        public decimal BaseUnitPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public ViewModel CreatedBy { get; set; }
        public List<NoteLog> Note { get; set; }
    }
    public class NoteLog
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public DateTime? UpdateAt { get; set; }
        public ViewModel UpdateBy { get; set; }
    }
    public class ViewBatch
    {
        public int Id { get; set; }
        public string Barcode { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
