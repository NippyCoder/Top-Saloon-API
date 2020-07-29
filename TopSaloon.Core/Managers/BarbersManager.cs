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
    public class BarbersManager : Repository<Barber, ApplicationDbContext>
    {
        public BarbersManager(ApplicationDbContext _context) : base(_context)
        {

        }

        public async Task<List<Barber>> getallBarbers()
        {
            return await Task.Run(() =>
            {
                List<Barber> serviceFeedBackQuestions = context.Barbers.ToList();
                return serviceFeedBackQuestions;
            });
        }

    }
}
