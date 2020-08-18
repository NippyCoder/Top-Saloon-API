using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class AddServiceFeedbackQuestionDTO
    {

      public int ServiceId { get; set; }
      public string QuestionAR { get; set; }
      public string QuestionEN { get; set; }

    }
}
