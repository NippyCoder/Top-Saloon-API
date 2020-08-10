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
            //Cancel Order: set order status to cancelled, pop order from corresponding queue.

            ApiResponse<string> result = new ApiResponse<string>();

            try
            {
                var order = await unitOfWork.OrdersManager.GetAsync(b => b.Id == orderId, 0, 0, null, includeProperties: "OrderServices");
                Order OrderToUpdate = order.FirstOrDefault();
                if (OrderToUpdate != null)
                {
                    OrderToUpdate.Status = "Cancelled";
                    var isUpdated_OrderService = false;
                    for (int i = 0; i < OrderToUpdate.OrderServices.Count; i++)
                    {
                        OrderToUpdate.OrderServices[i].IsConfirmed = false;
                        isUpdated_OrderService = true;
                    }
                    var OrderUpdateResult = false;
                    if (isUpdated_OrderService)
                    {
                        OrderUpdateResult = await unitOfWork.OrdersManager.UpdateAsync(OrderToUpdate);
                    }
                    //= await unitOfWork.SaveChangesAsync();
                    if (OrderUpdateResult)
                    {
                        var barberQueue = await unitOfWork.BarbersQueuesManager.GetAsync(b => b.Id == OrderToUpdate.BarberQueueId, 0, 0, null, includeProperties: "Orders");
                        if(barberQueue != null)
                        {
                            BarberQueue QueueToUpdate = barberQueue.FirstOrDefault();
                            for(int i=0; i<QueueToUpdate.Orders.Count; i++)
                            {
                                if(QueueToUpdate.Orders[i].Id == orderId)
                                {
                                    QueueToUpdate.Orders.Remove(QueueToUpdate.Orders[i]);
                                }
                            }
                            await unitOfWork.BarbersQueuesManager.UpdateAsync(QueueToUpdate);
                            var finalres = await unitOfWork.SaveChangesAsync();
                            if (finalres)
                            {
                                result.Data = "Order cancelled successfully.";
                                result.Succeeded = true;
                                return result;
                            }
                            else
                            {
                                result.Data = "Cancellation error.";
                                result.Succeeded = false;
                                return result;
                            }
                        }
                        else
                        {
                            result.Data = "Error";
                            result.Errors.Add("Failed to fetch barber Queue !");
                            return result;
                        }                    
                        //if (isUpdated_OrderService)
                        //{
                        //    var UpdateResult = await unitOfWork.SaveChangesAsync();
                        //    if (UpdateResult)
                        //    {
                        //        result.Data = "Order cancelled successfully!";
                        //        result.Succeeded = true;
                        //        return result;
                        //    }
                        //    else
                        //    {
                        //        result.Data = "Error.";
                        //        result.Errors.Add("Error Finalizing cancellation");
                        //        result.Succeeded = false;
                        //        return result;
                        //    }
                        //}
                        //else
                        //{
                        //    result.Data = "Error.";
                        //    result.Errors.Add("Error cancelling order service !");
                        //    result.Succeeded = false;
                        //    return result;
                        //}
                    }
                    else
                    {
                        result.Data = "Error.";
                        result.Errors.Add("Error cancelling order !");
                        result.Succeeded = false;
                        return result;
                    }
                }
                else
                {
                    result.Data = "Error";
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

