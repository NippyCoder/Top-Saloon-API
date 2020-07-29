
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TopSalon.DTOs.Models;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
using TopSaloon.DTOs.Models;
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
                result.Data = (await unitOfWork.BarbersQueuesManager.GetAsync(c => c.QueueStatus == "available")).Count();
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

                if (res != null)
                {
                    var res2 = await unitOfWork.BarbersQueuesManager.GetByIdAsync(BarberId);
                    result.Data.Id = res.Id;
                    result.Data.Name = res.Name;
                    result.Data.Status = res.Status;
                    result.Data.NumberOfCustomerHandled = (int)res.NumberOfCustomersHandled;
                   // result.Data.QueueStatus = res.BarberQueue.QueueStatus;
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
        public async Task<ApiResponse<List<Barber>>> GetAllBarbers()
        {
            ApiResponse<List<Barber>> result = new ApiResponse<List<Barber>>();

            try
            {
                List<Barber> barbers = await unitOfWork.BarbersManager.getallBarbers();

                if (barbers != null)
                {
                    result.Data = barbers.ToList();
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to get list !");
                    result.Succeeded = false;
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

