using System;
using System.Collections.Generic;
using TopSaloon.DTOs.Models;

namespace TopSaloon.DTOs.Models
{
    public partial class OrderDTO
    {
        public int Id { get; set; }
        public float? OrderTotal { get; set; }
        public DateTime? FinishTime { get; set; }
        public int? WaitingTimeInMinutes { get; set; }
        public int? OrderIdentifier { get; set; }
        public string Status { get; set; }
        public DateTime? OrderDate { get; set; }
        public int BarberQueueId { get; set; }
        public int CustomerId { get; set; }
        public virtual BarberQueueDTO BarberQueue { get; set; }
        public virtual CustomerDTO Customer { get; set; }
        public virtual OrderFeedbackDTO OrderFeedback { get; set; }
        public virtual List<OrderServiceDTO> OrderServices { get; set; }
    }
}
