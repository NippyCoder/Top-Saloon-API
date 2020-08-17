using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class BarberQueue
    {
        public int Id { get; set; }
        public string QueueStatus { get; set; }
        public int BarberId { get; set; }
        public int QueueWaitingTime { get; set; }
        public virtual Barber Barber { get; set; }
        public virtual List<Order> Orders { get; set; }
    }
}
