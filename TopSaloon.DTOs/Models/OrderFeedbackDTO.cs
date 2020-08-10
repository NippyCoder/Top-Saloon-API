using System;
using System.Collections.Generic;
using System.Text;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Models;

namespace TopSaloon.DTOs.Models
{
    public class OrderFeedbackDTO
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsSubmitted { get; set; }
        public int CompleteOrderId { get; set; }
        public virtual CompleteOrderDTO CompleteOrder { get; set; }
        public virtual List<OrderFeedbackQuestionDTO> OrderFeedbackQuestions { get; set; }
    }
}
