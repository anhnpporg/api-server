﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.HandlerModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.InventorySystemReportsService
{
    public interface IInventoryReport
    {
        Task SaveNoti(SaveNotiRequest request);
        Task<Response<List<ShowNotiModel>>> ViewAllNoti();
        Task<Response<ViewNotiModel>> ViewDetailNoti(DateTime key);
        Task<Response<List<ShowNotiModel>>> ShowFilterNoti();
        Task CheckViewNoti(int id);
    }
}
