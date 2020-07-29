using System;
using System.Collections.Generic;

namespace TopSalon.DTOs.Models
{
    public partial class OrderFeedbackQuestionDTO
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int? Rating { get; set; }
        public int OrderFeedbackId { get; set; }
        public virtual OrderFeedbackDTO OrderFeedback { get; set; }

    }
}
