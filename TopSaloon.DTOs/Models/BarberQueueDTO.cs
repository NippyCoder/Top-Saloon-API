using System;
using System.Collections.Generic;
using System.Text;
using TopSaloon.DTOs.Models;

namespace TopSaloon.DTOs.Models
{
    public class BarberQueueDTO
    {
        public int Id { get; set; }
        public string QueueStatus { get; set; }
        public int BarberId { get; set; }
        public int QueueWaitingTime { get; set; }
        public virtual BarberDTO Barber { get; set; }
        public virtual List<OrderDTO> Orders { get; set; }
    }
}
