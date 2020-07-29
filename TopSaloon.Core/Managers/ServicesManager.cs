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

        public async Task<List<Service>> getallservices()
        {
            return await Task.Run(() =>
            {
                List<Service> Service = context.Services.ToList();
                for (int i = 0; i < Service.Count; i++)
                {
                    var serviceFeedBackQuestionList = context.ServiceFeedBackQuestions.Where(b => b.ServiceId == Service[i].Id).ToList();

                    for (int k = 0; k < serviceFeedBackQuestionList.Count; k++)
                    {
                        Service[i].FeedBackQuestions[k] = serviceFeedBackQuestionList[k];
                    }
                }
                return Service;
            });
        }

    }
}
