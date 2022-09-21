using AutoMapper;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.CustomerModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UtNhanDrug_BE.Hepper.Paging;
using UtNhanDrug_BE.Models.ManagerModel;

namespace UtNhanDrug_BE.Services.CustomerService
{
    public class CustomerSvc : ICustomerSvc
    {
        private readonly IMapper _mapper;
        private readonly utNhanDrugStoreManagementContext _context;

        public CustomerSvc(IMapper mapper, utNhanDrugStoreManagementContext contex)
        {
            _mapper = mapper;
            _context = contex;
        }
        public async Task<Customer> FindByPhoneNumber(string phoneNumber)
        {
            CustomerExitsModel model = new CustomerExitsModel();
            Customer customer = _mapper.Map<Customer>(model);
            customer = await _context.Customers.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            return customer;
        }

        public async Task<PageResult<CustomerViewModel>> GetCustomers(PagingModel paging)
        {
            var query = from c in _context.Customers
                        join u in _context.Users on c.UserId equals u.Id
                        select new { u, c };

            //filter

            if (!string.IsNullOrEmpty(paging.Keyword))
                query = query.Where(x => x.u.Fullname.Contains(paging.Keyword));

            //paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((paging.PageIndex - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .Select(x => new CustomerViewModel()
                {
                    PhoneNumber = x.c.PhoneNumber,
                    User = x.u
                }).ToListAsync();

            // select and projection
            var pagedResult = new PageResult<CustomerViewModel>()
            {
                TotalRecord = totalRow,
                PageSize = paging.PageSize,
                PageIndex = paging.PageIndex,
                Items = data
            };
            return pagedResult;
        }
    }
}
