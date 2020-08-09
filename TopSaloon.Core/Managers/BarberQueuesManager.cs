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
    public class BarbersQueuesManager : Repository<BarberQueue, ApplicationDbContext>
    {
        public BarbersQueuesManager(ApplicationDbContext _context) : base(_context)
        {

        }

        public async Task<BarberQueue> GetBarberQueueByBarberId(int barberId)
        {
            return await Task.Run(() =>
            {
                var res = context.BarberQueues.Where(a => a.BarberId == barberId).FirstOrDefault();
                return res;
            });
        }

    }
}
