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
    public class OrderFeedBackQuestionsManager : Repository<OrderFeedbackQuestion, ApplicationDbContext>
    {
        public OrderFeedBackQuestionsManager(ApplicationDbContext _context) : base(_context)
        {

        }
         public  async Task <List<OrderFeedbackQuestion>> GetOrderFeedbackQuestionsByOrderFeedBackId(int id)
        {
            return await Task.Run(() =>
            {
                List<OrderFeedbackQuestion> Result = context.OrderFeedbackQuestions.Where(A=>A.OrderFeedbackId==id).ToList();

                return Result;

            });

        }



    }
}
