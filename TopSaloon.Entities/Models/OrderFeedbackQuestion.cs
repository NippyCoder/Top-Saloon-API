using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class OrderFeedbackQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int? Rating { get; set; }
        public int OrderFeedbackId { get; set; }
        public virtual OrderFeedback OrderFeedback { get; set; }
    }
}
