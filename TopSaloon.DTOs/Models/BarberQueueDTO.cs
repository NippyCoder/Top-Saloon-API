using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TopSaloon;

namespace TopSaloon.DTOs.Models
{
    public class BarberQueueDTO
    {
        public int QueueId { get; set; }
        public int BarberId { get; set; }
        public string QueueStatus { get; set; }
        public string BarberName { get; set; }
        public string BarberStatus { get; set; }
        public int? WaitingTimeInMinutes { get; set; }
    }
}
