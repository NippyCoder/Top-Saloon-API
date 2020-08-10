using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class CompleteOrderDTO
    {
        public int Id { get; set; }
        public int BarberId { get; set; }
        public float? OrderTotalAmount { get; set; }
        public int? CustomerWaitingTimeInMinutes { get; set; }
        public string Status { get; set; }
        public string OrderServicesList { get; set; }
        public DateTime? OrderDateTime { get; set; }
        public DateTime? OrderFinishTime { get; set; }

        public int CustomerId { get; set; }
        public CustomerDTO Customer { get; set; }
        public OrderFeedbackDTO OrderFeedback { get; set; }
    }
}
