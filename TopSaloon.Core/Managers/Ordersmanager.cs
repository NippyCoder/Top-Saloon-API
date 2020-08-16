using System;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using TopSaloon.DAL;
using TopSaloon.Entities;
using TopSaloon.Entities.Models;
using TopSaloon.Repository;
using TopSaloon.DTOs.Enums;
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
        public async Task<float> GetUserDailyEarning(DateTime Start, DateTime End)
        {
            return await Task.Run(() =>
            {
                var  Result = context.CompleteOrders.Where(A => A.OrderDateTime >= Start && A.OrderDateTime <= End).ToList();
                float total = 0.0f;
                for (int i = 0; i < Result.Count(); i++)
                {
                    total += (float)Result[i].OrderTotalAmount; 
                 }

               
                return total;
            });
        }
        public async Task<int> TotalVisitperDay(DateTime date)
        {

            return await Task.Run(() =>
            {
                var today = context.CompleteOrders.Where(a => a.OrderDateTime == date  && a.Status=="done");
                int Result = today.Count();

                return Result;

            });
        }
        public async Task<float> TotalAmountOfServiceCostPerDay(DateTime date)
        {

            return await Task.Run(() =>
            {
                var today = context.CompleteOrders.Where(a => a.OrderDateTime == date );

                float Result =0f ;
                List<CompleteOrder> tmp = today.ToList(); 
                for (int i = 0; i < today.Count(); i++)
                {
                    Result += (float)tmp[i].OrderTotalAmount; 
                }

                return Result;

            });
        }
        public async Task<float> AverageWaitingTimePerDay(DateTime date)
        {

            return await Task.Run(() =>
            {
                var today = context.CompleteOrders.Where(a => a.OrderDateTime == date && a.Status=="done");

                float Result = 0f;
                List<CompleteOrder> tmp = today.ToList();
                int listcount = today.Count(); 
                if (listcount >= 1)
                {
                    for (int i = 0; i < listcount; i++)
                    {
                        Result += (float)tmp[i].CustomerWaitingTimeInMinutes;
                    }
                    Result = Result / listcount;

                }
               
                return Result;

            });
        }

    }
}
