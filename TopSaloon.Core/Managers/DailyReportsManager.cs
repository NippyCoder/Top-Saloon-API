using System;
using System.Collections.Generic;
using System.Text;
using TopSaloon.DAL;
using TopSaloon.Entities.Models;
using TopSaloon.Repository;
using System.Threading.Tasks;
using System.Linq;


namespace TopSaloon.Core.Managers
{
    public class DailyReportsManager : Repository<DailyReport, ApplicationDbContext>
    {
        public DailyReportsManager(ApplicationDbContext _context) : base(_context)
        {

        }
        public async Task<int> GetSigndInbarbers(DateTime Today)
        {

            return await Task.Run(() =>
            {
                int Result = context.CompleteOrders.Where(a => a.OrderDateTime == Today).Distinct().Count(); 

                return Result;

            });
        }


    }
}
