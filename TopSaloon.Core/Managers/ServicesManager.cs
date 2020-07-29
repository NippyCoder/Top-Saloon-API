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
    public class ServicesManager : Repository<Service, ApplicationDbContext>
    {
        public ServicesManager(ApplicationDbContext _context) : base(_context)
        {

        }

        public async Task<List<Service>> GetAllServices()
        {
            return await Task.Run(() =>
            {
                List<Service> servicesList = context.Services.Select(x => x).ToList();
                return servicesList;
            });
        }


    }
}
