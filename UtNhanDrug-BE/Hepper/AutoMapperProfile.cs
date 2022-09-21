using AutoMapper;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.CustomerModel;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Models.StaffModel;

namespace UtNhanDrug_BE.Hepper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // using CreateMap<usingmodel, defaul model >
            CreateMap<CustomerViewModel, Customer>();
            CreateMap<CustomerExitsModel, Customer>();
            CreateMap<ManagerViewModel, Manager>();
            CreateMap<ViewStaffModel, Staff>();
            
        }
    }
}
