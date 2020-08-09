using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopSaloon.DAL;
using TopSaloon.Entities.Models;
using TopSaloon.Repository;

namespace TopSaloon.Core.Managers
{
    public class SMSManager : Repository<SMS, ApplicationDbContext>
    {
        public SMSManager(ApplicationDbContext _context) : base(_context)
        {

        }
         public SMS GEtSmsById(int id)
        {
            var sms = context.SMS.Where(b => b.Id == id ).FirstOrDefault();

            return sms;
        }


    }
}
