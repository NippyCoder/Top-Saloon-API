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
        public int OrderId { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsSubmitted { get; set; }
        public virtual OrderDTO Order { get; set; }
        public virtual List<OrderFeedbackDTO> OrderFeedbackQuestions { get; set; }
    }
}
