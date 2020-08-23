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
        public async Task<ApiResponse<int>> GetTotalNumberCustomerForToday()
        {
            
            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                //complete order object
                var myday = DateTime.Today;
                var CO = await unitOfWork.CompleteOrdersManager.GetAsync(A=>A.OrderDateTime.Value.Date >= myday );
                if (CO != null)
                {
                    result.Data = CO.Count();
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not Find Any Complete order");
                    return result;
                }


            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
        public async Task<ApiResponse<int>> GetTotalServiceCostForToday( )
        {

            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                //complete order object
                var myday = DateTime.Today;
                var CO = await unitOfWork.CompleteOrdersManager.GetAsync(A => A.OrderDateTime.Value.Date >= myday);
                if (CO != null)
                {
                    int total = 0;
                    var COList = CO.ToList(); 
                    for (int i = 0; i < CO.Count(); i++)
                    {
                        total +=(int)COList[i].OrderTotalAmount;  
                    }
                    result.Data = total;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not Find Any Complete order");
                    return result;
                }


            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
        public async Task<ApiResponse<float>> GetAverageOfWaitingTimeForToday( )
        {

            ApiResponse<float> result = new ApiResponse<float>();

            try
            {
                //complete order object
                var myday = DateTime.Today;

                var CO = await unitOfWork.CompleteOrdersManager.GetAsync(A => A.OrderDateTime.Value.Date >= myday);
                if (CO != null)
                {
                    var COList = CO.ToList();
                    float total = 0;
                   
                    for (int i = 0; i < CO.Count(); i++)
                    {
                        total += (float) COList[i].CustomerWaitingTimeInMinutes;
                    }
                    total = total / COList.Count(); 
                    result.Data = total;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not Find Any Complete order");
                    return result;
                }


            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
        public async Task<ApiResponse<int>> GetNumberOfSignedInBarbersForToday( )
        {

            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                //complete order object
                var myday = DateTime.Today;

                var CO = await unitOfWork.DailyReportsManager.GetSigndInbarbers();

                if (CO != 0)
                {
                   
                    result.Data = CO;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not Find Any Complete order");
                    return result;
                }


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

