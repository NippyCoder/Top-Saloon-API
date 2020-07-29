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
    public class OrdersService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public OrdersService(UnitOfWork unitOfWork, IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }

        public async Task<ApiResponse<List<OrderService>>> GetOrderServicesViaOrderId(int orderId)
        {

            ApiResponse<List<OrderService>> result = new ApiResponse<List<OrderService>>();

            try
            {
                var orderServices = await unitOfWork.OrderServicesManager.GetOrderServices(orderId);
                if (orderServices != null)
                {            
                    result.Data = orderServices;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not fetch order services");
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



        public async Task<ApiResponse<bool>> SetOrderService(int orderId)
        {

            ApiResponse<bool> result = new ApiResponse<bool>();

            try
            {
                var orderServices = await unitOfWork.OrderServicesManager.GetByIdAsync(orderId);
                
                if (orderServices != null)
                {
                    orderServices.IsConfirmed = true;

                    var isUpdated = await unitOfWork.OrderServicesManager.UpdateAsync(orderServices);
                    if (isUpdated)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error updating order service !");
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not fetch order service");
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

        public async Task<ApiResponse<string>> CancelOrder(int orderId)
        {

            ApiResponse<string> result = new ApiResponse<string>();

            try
            {
                var order = await unitOfWork.OrdersManager.GetByIdAsync(orderId);

                if (order != null)
                {
                    order.Status = "Cancelled";

                    var isUpdated = await unitOfWork.OrdersManager.UpdateAsync(order);
                    if (isUpdated)
                    {                      
                        var orderServices = await unitOfWork.OrderServicesManager.GetOrderServices(orderId);
                        bool orderService_Updated = false;

                        for(int i=0; i<orderServices.Count; i++)
                        {
                            orderServices[i].IsConfirmed = false;
                            var isUpdated_OrderService = await unitOfWork.OrderServicesManager.UpdateAsync(orderServices[i]);
                            if (isUpdated_OrderService)
                            {
                                orderService_Updated = true;
                            }
                            else
                            {
                                orderService_Updated = false;
                            }
                        }
                        if (orderService_Updated)
                        {
                            result.Data = "Order cancelled.";
                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            result.Data = "Error with cancelling order";
                            result.Succeeded = false;
                            return result;
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error updating order service !");
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not fetch order service");
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

