using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class DailyReport
    {
        public int Id { get; set; }
        public int? TotalNumberOfCustomers { get; set; }
        public float? TotalAmountOfServicesCost { get; set; }
        public int? TotalNumberOfBarbersSignedIn { get; set; }
        public int? AverageCustomerWaitingTimeInMinutes { get; set; }
        public DateTime? ReportDate { get; set; }
    }
}
