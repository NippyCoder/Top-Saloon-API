using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSaloon.DAL;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
using TopSaloon.Repository;

namespace TopSaloon.Core.Managers
{
    public class BarberLoginsManager : Repository<BarberLogin, ApplicationDbContext>
    {
        public BarberLoginsManager(ApplicationDbContext _context) : base(_context)
        {

        }
    }
}
