using TopSaloon.DAL;
using TopSaloon.Entities;
using TopSaloon.Entities.Models;
using TopSaloon.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.DTOs.Enums;
using System;
using System.Linq.Expressions;

namespace TopSaloon.Core.Managers
{
    public class OrderServicesManager : Repository<OrderService, ApplicationDbContext>
    {
        public OrderServicesManager(ApplicationDbContext _context) : base(_context)
        {

        }

        public async Task<List<OrderService>> GetOrderServices(int orderId)
        {
            return await Task.Run(() =>
            {
                List<OrderService> res = context.OrderServices.Where(table => table.OrderId == orderId && table.IsConfirmed == true).ToList();
                return res;
            });
        }

    }
}
