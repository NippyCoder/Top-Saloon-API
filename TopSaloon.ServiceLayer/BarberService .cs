
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
 using TopSaloon.Entities.Models;

namespace TopSaloon.ServiceLayer
{
    public class BarberService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public BarberService(UnitOfWork unitOfWork, IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }
        public async Task<ApiResponse<int>> GetNumberOfAvailableBarbers()
        {
            ApiResponse<int> result = new ApiResponse<int>();
            try
            {
                result.Data =  await unitOfWork.BarbersQueuesManager.GetAvilableBarber();

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }
            return result;
        }
        public async Task<ApiResponse<int>> BarberTotalNumberOfHandledCustomer(int BarberId)
        {
            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                Barber barber = await unitOfWork.BarbersManager.GetByIdAsync(BarberId);

                if (barber != null)
                {
                    result.Data = (int)barber.NumberOfCustomersHandled;
                    result.Succeeded = true;
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("It May Be A New Barber !");
                result.ErrorType = ErrorType.LogicalError;
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }

        public async Task<ApiResponse<BaberModelDTO>> GetBarberDetailsReports(int BarberId)
        {
            ApiResponse<BaberModelDTO> result = new ApiResponse<BaberModelDTO>();
            try
            {
                Barber res = await unitOfWork.BarbersManager.GetByIdAsync(BarberId);
                BaberModelDTO Temp = new BaberModelDTO();

                if (res != null)
                {
                    Temp.Id = res.Id;
                    Temp.Name = res.Name;
                    Temp.Status = res.Status;
                    Temp.NumberOfCustomerHandled = (int)res.NumberOfCustomersHandled;
                    var res2 = await unitOfWork.BarbersQueuesManager.GetByIdAsync(res.Id); 
                    Temp.QueueStatus = res2.QueueStatus;
                    result.Succeeded = true;
                    result.Data = Temp;

                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("It May Be A New Barber !");
                result.ErrorType = ErrorType.LogicalError;
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

