using AutoMapper;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.CustomerModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UtNhanDrug_BE.Hepper.Paging;
using UtNhanDrug_BE.Models.ManagerModel;
using System;

namespace UtNhanDrug_BE.Services.CustomerService
{
    public class CustomerSvc : ICustomerSvc
    {
        private readonly IMapper _mapper;
        private readonly utNhanDrugStoreManagementContext _context;
        private const string defaultAvatar = "https://firebasestorage.googleapis.com/v0/b/utnhandrug.appspot.com/o/image-profile.png?alt=media&token=928ea13d-d43f-4c0e-a8ba-ab1999059530";

        public CustomerSvc(IMapper mapper, utNhanDrugStoreManagementContext contex)
        {
            _mapper = mapper;
            _context = contex;
        }

        public async Task<bool> CreateCustomer(string phoneNumber, string fullName)
        {
            var customer = await FindByPhoneNumber(phoneNumber);

            if(customer == null)
            {
                var user = new User()
                {
                    IsBan = false,
                    CreateDate = DateTime.Now,
                    Avatar = defaultAvatar,
                    Fullname = fullName
                };
                _context.Users.Add(user);
                var isSavedUser = await _context.SaveChangesAsync();
                if (isSavedUser != 0)
                {
                    var cus = new Customer()
                    {
                        PhoneNumber = phoneNumber,
                        UserId = user.Id

                    };
                    _context.Customers.Add(cus);
                    var isSavedStaff = await _context.SaveChangesAsync();
                    if (isSavedStaff != 0) return true;
                }
            }
            return false;
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
