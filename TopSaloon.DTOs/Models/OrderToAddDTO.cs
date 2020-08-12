using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class OrderToAddDTO
    {
        public int BarberQueueId { get; set; }
        public int CustomerId { get; set; }
        public float? OrderTotal { get; set; }
        public int? TotalServicesWaitingTime { get; set; }
        public virtual List<OrderServiceToAddDTO> OrderServices { get; set; }
    }
}
