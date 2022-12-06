using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class DataSettingSystemCustomerPoint
    {
        public int Id { get; set; }
        public double TyLeQuyDoiDiemThuong { get; set; }
        public int TyLeQuyDoiThanhTien { get; set; }
        public int SoLanTichDiemToiThieu { get; set; }
        public bool TichDiemChoHoaDonGiamGiaBangDiemThuong { get; set; }
    }
}
