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
    public class BarbersQueuesManager : Repository<BarberQueue, ApplicationDbContext>
    {
        public BarbersQueuesManager(ApplicationDbContext _context) : base(_context)
        {

        }
        public async Task <int> GetAvilableBarber( )
        {

            return await Task.Run(() =>
            {
                int Result = context.BarberQueues.Where(a => a.QueueStatus == "avaliable").Count();

                return Result;

            });
        }



    }
}
