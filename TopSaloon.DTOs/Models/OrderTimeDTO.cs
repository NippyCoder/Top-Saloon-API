using System;
using System.Collections.Generic;

namespace TopSaloon.DTOs.Models
{
    public partial class OrderTimeDTO
    {
        public int Id { get; set; }
        public float? OrderTotal { get; set; }
        public int? WaitingTimeInMinutes { get; set; }
        public int? OrderIdentifier { get; set; }
        public int CustomerId { get; set; }
        public int BarberQueueId { get; set; }
        public string Status { get; set; } // Order status
        public DateTime? OrderDate { get; set; } // Order creation time
        public DateTime? FinishTime { get; set; } // Order Finish Time
        public int? OrderServicesTimeTotal { get; set; } // calculated services time
        public virtual List<OrderTimeServiceDTO> OrderServices { get; set; }
        public virtual CustomerDTO Customer {get; set;}
        public virtual OrderFeedbackDTO OrderFeedback { get; set; }
    }
}
