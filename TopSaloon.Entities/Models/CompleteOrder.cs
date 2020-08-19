using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.Entities.Models
{
    public class CompleteOrder
    {
        public int Id { get; set; }
        public int BarberId { get; set; }
        public float? OrderTotalAmount { get; set; }
        public float? TotalTimeSpent { get; set; }
        public int? CustomerWaitingTimeInMinutes { get; set; }
        public string Status { get; set; }
        public string OrderServicesList { get; set; }
        public DateTime? OrderDateTime { get; set; }
        public DateTime? OrderFinishTime { get; set; }
        public string CustomerName { get; set; }
        public string BarberName { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public OrderFeedback OrderFeedback { get; set; }

    }
}
