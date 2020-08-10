using AutoMapper;
using Microsoft.AspNetCore.Identity;
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

namespace TopSaloon.ServiceLayer
{
   public class ServiceService
    {
        private readonly UnitOfWork unitOfWork;
         private readonly IConfiguration config;
        private readonly IMapper mapper;

        public ServiceService(UnitOfWork unitOfWork , IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;

            this.config = config;
        }
        public async Task<ApiResponse<bool>> CreateService(AddServiceDTO model)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                Service newService = new Service();
                newService.Name = model.Name;
                newService.Time = model.Time;
                newService.Price = model.Price;
                Service ServiceResult = await unitOfWork.ServicesManager.GetServiceByName(model.Name);
                if (ServiceResult == null)
                {
                    var createServiceResult = await unitOfWork.ServicesManager.CreateAsync(newService);
                    await unitOfWork.SaveChangesAsync();
                    if (createServiceResult != null)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to create Service !");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }
                }
                result.Succeeded = false;
                result.Errors.Add("Service already exists !");
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
        public async Task<ApiResponse<bool>> Deleteservice(string ServiceId)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                int serviceId = Int32.Parse(ServiceId);
                var service = await unitOfWork.ServicesManager.GetByIdAsync(serviceId);
                var serviceResult = await unitOfWork.ServicesManager.RemoveAsync(service);
                if (serviceResult == true)
                {
                    var res2 = await unitOfWork.SaveChangesAsync();
                    if (res2)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error Deleting Service !");
                        return result;
                    }
                }
                result.Succeeded = false;
                result.Errors.Add("Service doesn't exist !");
                result.ErrorType = ErrorType.LogicalError;
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }
        }
        public async Task<ApiResponse<bool>> EditService( ServiceModelDTO model)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                var service = await unitOfWork.ServicesManager.GetByIdAsync(model.Id);
                Service serv = await unitOfWork.ServicesManager.GetServiceByName(model.Name);
                if (serv == null)
                {
                    service.Name = model.Name;
                }
                service.Price = model.Price;
                service.Time = model.Time;
                var res = await unitOfWork.SaveChangesAsync();
                if (res)
                {
                    result.Data = true;
                    result.Succeeded = true;
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("Failed to edit Service !");
                result.ErrorType = ErrorType.LogicalError;
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
            }
            return result;
        }
    }
}
