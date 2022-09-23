using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Hepper.Paging;
using UtNhanDrug_BE.Models.CustomerModel;
using UtNhanDrug_BE.Models.ManagerModel;

namespace UtNhanDrug_BE.Services.CustomerService
{
    public interface ICustomerSvc
    {
        Task<PageResult<CustomerViewModel>> GetCustomers(PagingModel paging);
        Task<Customer> FindByPhoneNumber(string phoneNumber);
        Task<bool> CreateCustomer(string phoneNumber, string fullName);
    }
}
