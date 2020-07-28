using System;
using System.Collections.Generic;
using System.Text;
using TopSaloon.DAL;
using TopSaloon.Entities.Models;
using TopSaloon.Repository;

namespace TopSaloon.Core.Managers
{
    public class DailyReportsManager : Repository<DailyReport, ApplicationDbContext>
    {
        public DailyReportsManager(ApplicationDbContext _context) : base(_context)
        {

        }



    }
}
