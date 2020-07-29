using System;
using System.Collections.Generic;

namespace TopSalon.Entities.Models
{
    public partial class OrderDTO
    {
        public int Id { get; set; }
        public float? OrderTotal { get; set; }
        public int? WaitingTimeInMinutes { get; set; }
        public int? OrderIdentifier { get; set; }
        public string Status { get; set; }
        public DateTime? OrderDate { get; set; }
        public int BarberQueueId { get; set; }
        //public virtual OrderFeedback OrderFeedback { get; set; }
        //order public virtual List<OrderService> OrderServices { get; set; }
    }
}
