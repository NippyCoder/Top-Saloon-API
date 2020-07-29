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
using System.ComponentModel;
using TopSaloon.DTOs.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace TopSalon.Core.Managers
{
    public class OrdersManager : Repository<Order, ApplicationDbContext>
    {
        public OrdersManager(ApplicationDbContext _context) : base(_context)
        {

        }


        public async Task<int> GetOrderByBarberQueue(int barberQueue)
        {
            int totalWaitingTime = 0;

            return await Task.Run(() =>
            {
                List<Order> res = context.Orders.Where(table => table.BarberQueueId == barberQueue).ToList();
                for (int i = 0; i < res.Count; i++)
                {
                    totalWaitingTime += res[i].WaitingTimeInMinutes.Value;
                }
                return totalWaitingTime;
            });
        }


        public async Task<List<Order>> GetOrdersViaBarberQueues(int barberQueue)
        {

            return await Task.Run(() =>
            {
                List<Order> res = context.Orders.Where(table => table.BarberQueueId == barberQueue).ToList();
                return res;
              

            });
            
        }
    }
}
