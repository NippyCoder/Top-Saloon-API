﻿using System;
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
        public async Task<Service> GetServiceByNameEN(string NameEN)
        {
            return await Task.Run(() =>
            {
                var service = context.Services.Where(a => a.NameEN == NameEN).FirstOrDefault();
                return service;
            });
        }
        public async Task<Service> GetServiceByNameAR(string NameAR)
        {
            return await Task.Run(() =>
            {
                var service = context.Services.Where(a => a.NameAR == NameAR).FirstOrDefault();
                return service;
            });
        }
        public async Task<int> GetServiceByNameCount(string NameEN)
        {
            return await Task.Run(() =>
            {
                int service = context.Services.Where(a => a.NameEN == NameEN).Count();
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
