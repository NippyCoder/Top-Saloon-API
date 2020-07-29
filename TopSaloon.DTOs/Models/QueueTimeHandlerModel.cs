using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
   public class QueueTimeHandlerModel
    {
        public int QueueId { get; set; }
        public int QueueEstimatedFinishTime { get; set; }
        public List <OrderTimeDTO> Orders { get; set; }
    }
}
