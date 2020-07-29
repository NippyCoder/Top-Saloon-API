using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
     public class ServiceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float? Price { get; set; }
        public int? Time { get; set; }
        public virtual List<ServiceFeedbackQuestionDTO> FeedBackQuestions { get; set; }
    }
}
