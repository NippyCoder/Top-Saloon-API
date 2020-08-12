using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class OrderFeedback
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsSubmitted { get; set; }
        public int CompleteOrderId { get; set; }
        public virtual CompleteOrder CompleteOrder { get; set; }
        public virtual List<OrderFeedbackQuestion> OrderFeedbackQuestions { get; set; }
    }
}
