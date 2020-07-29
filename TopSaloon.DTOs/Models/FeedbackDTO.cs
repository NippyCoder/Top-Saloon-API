using System;
using System.Collections.Generic;
using System.Text;
using TopSaloon.DTOs.Models;

namespace TopSaloon.DTOs.Models
{
    public class FeedbackDTO
    {
        public int id { get; set; }
        public string comment { get; set; }
        public DateTime date { get; set; }

        //Order feedback questions
        public virtual List<OrderFeedbackQuestionDTO> OrderFeedbackQuestions { get; set; }
    }
}
