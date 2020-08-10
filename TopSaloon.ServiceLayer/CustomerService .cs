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
using AutoMapper;

namespace TopSaloon.ServiceLayer
{
    public class CustomerService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private readonly IMapper mapper;


        public CustomerService(UnitOfWork unitOfWork, IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper;
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
                
                // CWO is Customer With there Orders 
                var CWO = await unitOfWork.CustomersManager.GetAsync(b => b.Id == UserId, includeProperties: "Order");
                List<Customer> customer = CWO.ToList(); 
                if (CWO != null)
                {
                     
                    result.Data = mapper.Map<CustomerDetailsModelDTO>(customer[0]);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("It Is New Customer !");
                    result.ErrorType = ErrorType.LogicalError;
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

        public async Task<ApiResponse<bool>> AddNewCustomer(AddCustomerModel model)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                Customer newCustomer = new Customer();
                newCustomer.Name = model.Name;
                newCustomer.PhoneNumber = model.PhoneNumber;
                newCustomer.TotalNumberOfVisits = 0;

                Customer customerResult = await unitOfWork.CustomersManager.GetCustomerByPhoneNumber(model.PhoneNumber);

                if (customerResult == null)
                {
                    var createCustomerResult = await unitOfWork.CustomersManager.CreateAsync(newCustomer);

                    await unitOfWork.SaveChangesAsync();

                    if (createCustomerResult != null)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to create customer !");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }
                }
                result.Succeeded = false;
                result.Errors.Add("Customer already exists !");
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

        public async Task<ApiResponse<CustomerInfoDTO>> Login(string phoneNumber)
        {
            ApiResponse<CustomerInfoDTO> result = new ApiResponse<CustomerInfoDTO>();

            try
            {
                Customer customer = await unitOfWork.CustomersManager.GetCustomerByPhoneNumber(phoneNumber);

                if (customer != null)
                {
                    CustomerInfoDTO customerInfo = new CustomerInfoDTO();
                    customerInfo.Id = customer.Id;
                    customerInfo.Name = customer.Name;
                    customerInfo.LastBarberId = customer.LastBarberId;
                    customerInfo.LastVisitDate = customer.LastVisitDate;
                    customerInfo.TotalNumberOfVisits = customer.TotalNumberOfVisits;
                    customerInfo.PhoneNumber = customer.PhoneNumber;


                    result.Data = customerInfo;
                    result.Succeeded = true;
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("Customer already exists !");
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

        public async Task<ApiResponse<int>> GetNumberOfCustomerVisitForToday(DateTime date)
        {
            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                var AllOrdersByDay = await unitOfWork.OrdersManager.TotalVisitperDay(date);

                if (AllOrdersByDay != 0)
                {
                    result.Data = AllOrdersByDay;
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
        public async Task<ApiResponse<float>> TotalAmountOfServiceCostForToday(DateTime date)
        {
            ApiResponse<float> result = new ApiResponse<float>();

            try
            {
                var AllOrdersByDay = await unitOfWork.OrdersManager.TotalAmountOfServiceCostPerDay(date);

                if (AllOrdersByDay != 0)
                {
                    result.Data = AllOrdersByDay;
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
        public async Task<ApiResponse<float>> AverageWaitingForToday(DateTime date)
        {
            ApiResponse<float> result = new ApiResponse<float>();

            try
            {
                var AllOrdersByDay = await unitOfWork.OrdersManager.AverageWaitingTimePerDay(date);

                if (AllOrdersByDay != 0)
                {
                    result.Data = AllOrdersByDay;
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

