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
        public async Task<Service> GetServiceByName(string Name)
        {
            return await Task.Run(() =>
            {
                var service = context.Services.Where(a => a.Name == Name).FirstOrDefault();
                return service;
            });
        }
        public async Task<int> GetServiceByNameCount(string Name)
        {
            return await Task.Run(() =>
            {
                int service = context.Services.Where(a => a.Name == Name).Count();
                return service;
            });
        }


        public async Task<List<Service>> getallservices()
        {
            return await Task.Run(() =>
            {
                List<Service> Service = context.Services.ToList();
 
                return Service;
            });
        }

    }
}
