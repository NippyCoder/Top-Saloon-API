﻿using Microsoft.AspNetCore.Identity;
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
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
using AutoMapper;


namespace TopSaloon.ServiceLayer
{
    public class BarberService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private readonly IMapper mapper;

        public BarberService(UnitOfWork unitOfWork, IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper;


        }
        public async Task<ApiResponse<int>> GetNumberOfAvailableBarbers()
        {
            ApiResponse<int> result = new ApiResponse<int>();
            try
            {
                result.Data =  await unitOfWork.BarbersManager.GetNumberOfAvailableBarber();

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
        public async Task<ApiResponse<BarberDTO>> GetBarberDetailsReports(int BarberId)
        {
            ApiResponse<BarberDTO> result = new ApiResponse<BarberDTO>();
            try
            {
                var barber = await unitOfWork.BarbersManager.GetAsync(A=>A.Id==BarberId, includeProperties: "BarberQueue");
                 List<Barber> barberData = barber.ToList();

                if (barber != null)
                {
                    result.Data = mapper.Map<BarberDTO>(barberData[0]);
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
        public async Task<ApiResponse<List<BarberDTO>>> GetAllBarbers()
        {
            ApiResponse<List<BarberDTO>> result = new ApiResponse<List<BarberDTO>>();

            try
            {
                var barbersList = await unitOfWork.BarbersManager.GetAsync();

                List<Barber> barberListToReturn = barbersList.ToList();


                if (barberListToReturn != null)
                {
                    result.Data = mapper.Map<List<BarberDTO>>(barberListToReturn);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to retreive barbers list !");
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
        public async Task<ApiResponse<List<BarberDTO>>> GetAvailableBarbers()
        {
            ApiResponse<List<BarberDTO>> result = new ApiResponse<List<BarberDTO>>();

            try
            {
                var Barbers = await unitOfWork.BarbersManager.GetAsync(b => b.Status == "Available" || b.Status == "Busy", includeProperties: "BarberProfilePhoto");

                if (Barbers != null)
                {
                    result.Data = mapper.Map<List<BarberDTO>>(Barbers.ToList());
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to fetch barber list !");
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

