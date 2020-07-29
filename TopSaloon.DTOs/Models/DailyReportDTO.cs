using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class DailyReportDTO
    {
        public int Id { get; set; }
        public int? TotalNumberOfCustomers { get; set; }
        public float? TotalAmountOfServicesCost { get; set; }
        public int? TotalNumberOfBarbersSignedIn { get; set; }
        public int? AverageCustomerWaitingTimeInMinutes { get; set; }
        public DateTime? ReportDate { get; set; }
    }
}
