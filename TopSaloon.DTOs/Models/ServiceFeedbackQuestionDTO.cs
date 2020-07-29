using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class ServiceFeedbackQuestionDTO
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int ServiceId { get; set; }
        public virtual ServiceDTO Service { get; set; }
    }
}
