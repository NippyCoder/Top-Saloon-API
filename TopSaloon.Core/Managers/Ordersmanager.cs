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


        public async Task<QueueTimeHandlerModel> GetOrdersViaBarberQueues(int barberQueue)
        {

            return await Task.Run(() =>
            {
                List<Order> res = context.Orders.Where(table => table.BarberQueueId == barberQueue).ToList();
                List<OrderTimeDTO> Orders = new List<OrderTimeDTO>();
                QueueTimeHandlerModel queueHandler = new QueueTimeHandlerModel();

                for(int i=0; i<res.Count; i++) 
                {
                    Orders[i].Id = res[i].Id;
                    Orders[i].OrderDate = res[i].OrderDate;
                    Orders[i].Status = res[i].Status;
                    //for(int k=0; k<res[i].OrderServices.Count; k++)
                    //{
                    //    Orders[i].OrderServices[k].Id = res[i].OrderServices[k].Id;
                    //    Orders[i].OrderServices[k].IsConfirmed = res[i].OrderServices[k].IsConfirmed;
                    //    Orders[i].OrderServices[k].Name = res[i].OrderServices[k].Name;
                    //    Orders[i].OrderServices[k].OrderId = res[i].OrderServices[k].OrderId;
                    //    Orders[i].OrderServices[k].Price = res[i].OrderServices[k].Price;
                    //    Orders[i].OrderServices[k].Time = res[i].OrderServices[k].Time;
                    //}
                }
                queueHandler.Orders = Orders;
                return queueHandler;
            });
            
        }
    }
}
