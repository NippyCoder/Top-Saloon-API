using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class ServiceFeedbackQuestionDTO
    {
        public int Id { get; set; }
        public string QuestionAR { get; set; }
        public string QuestionEN { get; set; }
        public int ServiceId { get; set; }
        public virtual ServiceDTO Service { get; set; }
    }
}
