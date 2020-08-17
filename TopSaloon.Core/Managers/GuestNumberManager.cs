using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopSaloon.DAL;
using TopSaloon.Entities.Models;
using TopSaloon.Repository;

namespace TopSaloon.Core.Managers
{
    public class GuestNumberManager : Repository<GuestNumber, ApplicationDbContext>
    {
        public GuestNumberManager(ApplicationDbContext _context) : base(_context)
        {

        }
    }
}
