

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
    public class ServiceService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public ServiceService(UnitOfWork unitOfWork, IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }

        public async Task<ApiResponse<List<Service>>> GetAllServices()
        {
            ApiResponse<List<Service>> result = new ApiResponse<List<Service>>();

            try
            {
                List<Service> services = await unitOfWork.ServicesManager.getallservices();

               
                if (services != null)
                {
                    result.Data = services.ToList();
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
