using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopSaloon.ServiceLayer
{
    public class DailyReportService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public DailyReportService(UnitOfWork unitOfWork, IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }


        public async Task<ApiResponse<DailyReport>> SaveDailyReport(DailyReportDTO dailyReport)
        {

            ApiResponse<DailyReport> result = new ApiResponse<DailyReport>();

            try
            {
                DailyReport report = new DailyReport();
                report.ReportDate = dailyReport.ReportDate;
                report.TotalAmountOfServicesCost = dailyReport.TotalAmountOfServicesCost;
                report.TotalNumberOfBarbersSignedIn = dailyReport.TotalNumberOfBarbersSignedIn;
                report.TotalNumberOfCustomers = dailyReport.TotalNumberOfCustomers;
                report.AverageCustomerWaitingTimeInMinutes = dailyReport.AverageCustomerWaitingTimeInMinutes;

                var CreatedReport = await unitOfWork.DailyReportsManager.CreateAsync(report);

                result.Data = report;
                result.Succeeded = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
    }
}

