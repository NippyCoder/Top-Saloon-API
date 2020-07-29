
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
    public class CustomerService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public CustomerService(UnitOfWork unitOfWork, IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }
        public async Task<ApiResponse<int>> GetCustomerTotalNumberOfVisit(int UserId)
        {
            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                Customer customer = await unitOfWork.CustomersManager.GetByIdAsync(UserId);

                if (customer != null)
                {
                    result.Data = (int)customer.TotalNumberOfVisits;
                    result.Succeeded = true;
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("It Is New Customer !");
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


        public async Task<ApiResponse<CustomerDetailsModelDTO>> GetCustomerVisitDetails(int UserId)
        {
            ApiResponse<CustomerDetailsModelDTO> result = new ApiResponse<CustomerDetailsModelDTO>();

            try
            {
                Customer customer = await unitOfWork.CustomersManager.GetByIdAsync(UserId);

                if (customer != null)
                {
                    CustomerDetailsModelDTO customerDetailsModelDTO = new CustomerDetailsModelDTO();
                    customerDetailsModelDTO.Name = customer.Name;
                    customerDetailsModelDTO.PhoneNumber = customer.PhoneNumber;
                    customerDetailsModelDTO.TotalNumberOfVisit = (int)customer.TotalNumberOfVisits;
                    customerDetailsModelDTO.dateVisitDate = (DateTime)customer.LastVisitDate;



                    result.Data = customerDetailsModelDTO;
                    result.Succeeded = true;
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("It Is New Customer !");
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

