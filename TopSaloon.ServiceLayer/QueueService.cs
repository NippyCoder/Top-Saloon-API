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
using AutoMapper;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace TopSaloon.ServiceLayer
{
    public class QueueService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private IMapper mapper;

        public QueueService(UnitOfWork unitOfWork, IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<BarberQueueDTO>> GetBarberQueueByBarberId(int barberId)
        {
            //Return Barber Name, barber status, barber queue status, total waiting time (orders)

            var result = new ApiResponse<BarberQueueDTO>();

           
            BarberQueueDTO Queue = new BarberQueueDTO();

            try
            {
                var BarberQueue = await unitOfWork.BarbersQueuesManager.GetBarberQueueByBarberId(barberId);
                if (BarberQueue != null)
                {
                    Queue.Id = BarberQueue.Id;
                    Queue.BarberId = BarberQueue.BarberId;
                    Queue.QueueStatus = BarberQueue.QueueStatus;
                    
                    var Barber = await unitOfWork.BarbersManager.GetByIdAsync(barberId);
                    if (Barber != null)
                    {
                        Queue.Barber.Name = Barber.Name;
                        Queue.Barber.Status = Barber.Status;
                        var QueueTotalTime = await unitOfWork.OrdersManager.GetOrderByBarberQueue(Queue.Id);
                       // Queue. = QueueTotalTime;

                        result.Data = Queue;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Barber not found!");
                        return result;
                    }  
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Barber queue not found");
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

        //Barber Queue order addition
        public async Task<ApiResponse<bool>> AddOrderToQueue(OrderToAddDTO order, int QueueId)
        {
            var result = new ApiResponse<bool>();
            BarberQueue Queue = new BarberQueue();
            Order currentOrder = new Order();
            try
            {
                var BarberQueue = await unitOfWork.BarbersQueuesManager.GetAsync(b => b.Id == QueueId, 0, 0, null, includeProperties: "Orders");
                if (BarberQueue != null)
                {
                    Queue = BarberQueue.FirstOrDefault();
                    // var QueueOrders = await unitOfWork.OrdersManager.GetAsync(b => b.BarberQueueId == QueueId, 0, 0, null, includeProperties: "OrderServices");

                    if (Queue.Orders.Count == 0 && Queue.QueueStatus == "idle") // Empty queue 
                    {
                        currentOrder.Id = order.Id;
                        currentOrder.OrderDate = DateTime.Now;
                        currentOrder.FinishTime = DateTime.Now.AddMinutes(Convert.ToDouble(order.WaitingTimeInMinutes));
                        currentOrder.OrderIdentifier = order.OrderIdentifier;
                        currentOrder.OrderTotal = order.OrderTotal;
                        currentOrder.Status = "inprogress";
                        currentOrder.WaitingTimeInMinutes = order.WaitingTimeInMinutes;
                        currentOrder.BarberQueueId = order.BarberQueueId;
                        currentOrder.CustomerId = order.CustomerId;
                        currentOrder.OrderServices = mapper.Map<List<OrderService>>(order.OrderServices.ToList());
                        var CreationResult = await unitOfWork.OrdersManager.CreateAsync(currentOrder);
            
                        if (CreationResult != null)
                        {
                            OrderService ServiceCreationResult;
                            for(int i=0; i<CreationResult.OrderServices.Count; i++)
                            {
                                ServiceCreationResult = await unitOfWork.OrderServicesManager.CreateAsync(currentOrder.OrderServices[i]);
                            }
                            Queue.QueueStatus = "busy";
                            var BarberUpdateResult = await unitOfWork.BarbersQueuesManager.UpdateAsync(Queue);
                            var FinalRes = await unitOfWork.SaveChangesAsync();
                            if (FinalRes)
                            {
                                result.Succeeded = true;
                                result.Data = true;
                                return result;
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("Unable to save changes");
                                return result;
                            }
                        }
                        else
                        {
                            result.Succeeded = false;
                            return result;
                        }
                    }
                    else
                    {
                        currentOrder.Id = order.Id;
                        currentOrder.OrderDate = DateTime.Now;
                        currentOrder.FinishTime = null;
                        currentOrder.OrderIdentifier = order.OrderIdentifier;
                        currentOrder.OrderTotal = order.OrderTotal;
                        currentOrder.Status = "pending";
                        currentOrder.WaitingTimeInMinutes = order.WaitingTimeInMinutes;
                        currentOrder.BarberQueueId = order.BarberQueueId;
                        currentOrder.CustomerId = order.CustomerId;
                        currentOrder.OrderServices = mapper.Map<List<OrderService>>(order.OrderServices.ToList());
                        var CreationResult = await unitOfWork.OrdersManager.CreateAsync(currentOrder);

                        if (CreationResult != null)
                        {
                            OrderService ServiceCreationResult;
                            for (int i = 0; i < CreationResult.OrderServices.Count; i++)
                            {
                                ServiceCreationResult = await unitOfWork.OrderServicesManager.CreateAsync(currentOrder.OrderServices[i]);
                            }
                            var FinalRes = await unitOfWork.SaveChangesAsync();
                            if (FinalRes)
                            {
                                result.Succeeded = true;
                                result.Data = true;
                                return result;
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("Unable to save changes");
                                return result;
                            }
                        }
                        else
                        {
                            result.Succeeded = false;
                            return result;
                        }
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Barber queue not found");
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

        public async Task<ApiResponse<QueueTimeHandlerModel>> GetBarberQueueWaitingTime(int QueueId)
        {
            //Return Barber Name, barber status, barber queue status, total waiting time (orders)

            var result = new ApiResponse<QueueTimeHandlerModel>();
            QueueTimeHandlerModel handler = new QueueTimeHandlerModel();

            try
            {
                var barberQueue = await unitOfWork.BarbersQueuesManager.GetByIdAsync(QueueId);

                if (barberQueue != null)
                {
                    handler.QueueId = barberQueue.Id;

                    var queueOrders = await unitOfWork.OrdersManager.GetAsync(b => b.BarberQueueId == handler.QueueId, 0,0,null, includeProperties: "OrderServices");
                    //Check for queue availability
                    if (queueOrders != null)
                    {
                        TimeSpan? CalculatedDateTime;
                        handler.Orders = mapper.Map<List<OrderDTO>>(queueOrders.ToList());
                        if (handler.Orders.Count == 0)
                        {
                            handler.QueueEstimatedFinishTime = null;
                            handler.QueueEstimatedWaitingTime = 0;
                            handler.QueueId = QueueId;
                            handler.Orders = null;
                            result.Data = handler;
                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            for (int i = 0; i < handler.Orders.Count; i++)
                            {
                                if (i == 0)
                                {
                                    handler.QueueEstimatedFinishTime = handler.Orders[i].FinishTime;
                                    handler.QueueEstimatedWaitingTime = 0;
                                }
                                else
                                {
                                    handler.QueueEstimatedFinishTime = handler.QueueEstimatedFinishTime.Value.AddMinutes(Convert.ToDouble(handler.Orders[i].WaitingTimeInMinutes.Value));
                                    CalculatedDateTime = (handler.QueueEstimatedFinishTime - handler.Orders[i].OrderDate);
                                    handler.QueueEstimatedWaitingTime = CalculatedDateTime.Value.TotalMinutes;
                                    //Calculated DateTime: time difference between General estimate finish time and current order creation date.
                                }

                            }
                            CalculatedDateTime = (handler.QueueEstimatedFinishTime - DateTime.Now);          
                            handler.QueueEstimatedWaitingTime = CalculatedDateTime.Value.TotalMinutes;
                            if(handler.QueueEstimatedWaitingTime < 0)
                            {
                                handler.QueueEstimatedWaitingTime = 0;
                            }
                            result.Data = handler;
                            result.Succeeded = true;
                            return result;
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error fetching queue orders!");
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Barber Queue not found");
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

        public async Task<ApiResponse<bool>> ReassignOrderToDifferentQueue(int orderId, int newBarberQueue)
        {
            //Return Barber Name, barber status, barber queue status, total waiting time (orders)

            var result = new ApiResponse<bool>();


            Order orderToUpdate = new Order();

            try
            {
                var order = await unitOfWork.OrdersManager.GetByIdAsync(orderId);

                if (order != null)
                {
                    orderToUpdate.Id = order.Id;
                    orderToUpdate.OrderDate = order.OrderDate;
                    orderToUpdate.OrderFeedback = order.OrderFeedback;
                    orderToUpdate.OrderIdentifier = order.OrderIdentifier;
                    orderToUpdate.OrderServices = order.OrderServices;
                    orderToUpdate.OrderTotal = order.OrderTotal;
                    orderToUpdate.Status = order.Status;
                    orderToUpdate.WaitingTimeInMinutes = order.WaitingTimeInMinutes;
                    orderToUpdate.BarberQueueId = newBarberQueue;


                    bool isUpdated = await unitOfWork.OrdersManager.UpdateAsync(orderToUpdate);
                    if (isUpdated)
                    {

                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error changing Order queue");
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error fetching order");
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

