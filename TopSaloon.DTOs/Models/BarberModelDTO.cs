using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class BaberModelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int NumberOfCustomerHandled { get; set; }
        public string QueueStatus { get; set; }

    }
}
