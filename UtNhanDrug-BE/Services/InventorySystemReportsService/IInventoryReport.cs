﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.HandlerModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.InventorySystemReportsService
{
    public interface IInventoryReport
    {
        Task SaveNoti(SaveNotiRequest request);
        Task<Response<List<ViewNotiModel>>> ViewNoti();
        Task CheckViewNoti(int id);
    }
}