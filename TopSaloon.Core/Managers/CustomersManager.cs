using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSaloon.DAL;
using TopSaloon.Entities.Models;
using TopSaloon.Repository;

namespace TopSaloon.Core.Managers
{
    public class CustomersManager : Repository<Customer, ApplicationDbContext>
    {
        public CustomersManager(ApplicationDbContext _context) : base(_context)
        {

        }

        public async Task<Customer> GetCustomerByPhoneNumber(string phoneNumber)
        {
            return await Task.Run(() =>
            {
                var customer = context.Customers.Where(a => a.PhoneNumber == phoneNumber).FirstOrDefault();
                return customer;
            });
        }

    }
}
