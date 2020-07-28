using System;
using System.Collections.Generic;
using System.Text;
using TopSaloon.DAL;
using TopSaloon.Entities.Models;
using TopSaloon.Repository;

namespace TopSaloon.Core.Managers
{
    public class ShopsManager : Repository<Shop, ApplicationDbContext>
    {
        public ShopsManager(ApplicationDbContext _context) : base(_context)
        {

        }



    }
}
