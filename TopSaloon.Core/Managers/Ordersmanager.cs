using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSaloon.DAL;
using TopSaloon.Entities.Models;
using TopSaloon.Repository;

namespace TopSalon.Core.Managers
{
    public class OrdersManager : Repository<Order, ApplicationDbContext>
    {
        public OrdersManager(ApplicationDbContext _context) : base(_context)
        {

        }
        public async Task<float> GetUserDailyEarning(DateTime Start, DateTime End)
        {
            return await Task.Run(() =>
            {
                var  Result = context.Orders.Where(A => A.OrderDate >= Start && A.OrderDate <= End).ToList();
                float total = 0.0f;
                for (int i = 0; i < Result.Count(); i++)
                {
                    total += (float)Result[i].OrderTotal; 
                 }

               
                return total;
            });
        }
    }
}
