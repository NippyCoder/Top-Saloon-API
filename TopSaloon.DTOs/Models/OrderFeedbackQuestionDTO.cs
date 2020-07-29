using System;
using System.Collections.Generic;

namespace TopSalon.DTOs.Models
{
    public partial class OrderFeedbackQuestionDTO
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int? Rating { get; set; }

    }
}
