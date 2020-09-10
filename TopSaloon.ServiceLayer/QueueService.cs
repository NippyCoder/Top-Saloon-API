﻿using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;

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

            try
            {
                var barberQueueToFetch = await unitOfWork.BarbersQueuesManager.GetAsync(b=> b.BarberId == barberId, includeProperties: "Orders");
                var BarberQueue = barberQueueToFetch.ToList().FirstOrDefault();
                if (BarberQueue != null)
                {
                    result.Data = mapper.Map<BarberQueueDTO>(BarberQueue);
                    result.Succeeded = true;
                    return result;
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

        public async Task<ApiResponse<bool>> AddOrderToQueue(OrderToAddDTO addOrderRequest)
        {
            var result = new ApiResponse<bool>();
            BarberQueue currentQueue ;
            Customer currentCustomer ;
            Order orderToAdd = new Order();
            TimeSpan? waitingTime;
            try
            {

                //Map order services from the add order request .
                orderToAdd.OrderServices = new List<OrderService>();
                orderToAdd.OrderServices = mapper.Map<List<OrderService>>(addOrderRequest.OrderServices);

                //Set queue waiting times to check for inactive orders.
                await SetQueueWaitingTimes();

                //Fetch barber queue to add order to . 
                var barberQueueResult = await unitOfWork.BarbersQueuesManager.GetAsync(b => b.Id == addOrderRequest.BarberQueueId, includeProperties:"Orders");
                currentQueue = barberQueueResult.FirstOrDefault();

                //Fetch customer.
                currentCustomer = await unitOfWork.CustomersManager.GetByIdAsync(addOrderRequest.CustomerId);

                //Calculate order total and total service time .
                orderToAdd.TotalServicesWaitingTime = 0;
                orderToAdd.OrderTotal = 0;
                orderToAdd.CustomerId = currentCustomer.Id;
                orderToAdd.CustomerName = currentCustomer.Name;
                orderToAdd.CustomerMobile = currentCustomer.PhoneNumber;
                orderToAdd.BarberQueueId = currentQueue.Id;
                orderToAdd.OrderDate = DateTime.Now;
                orderToAdd.WaitingTimeInMinutes = 0;

                for(int i = 0; i < orderToAdd.OrderServices.Count; i++)
                {
                    orderToAdd.TotalServicesWaitingTime += orderToAdd.OrderServices[i].Time;    
                    orderToAdd.OrderTotal += orderToAdd.OrderServices[i].Price;
                    orderToAdd.OrderServices[i].IsConfirmed = false;
                }


                if (currentQueue.Orders.Count == 0) // if queue is empty . 
                {
                    orderToAdd.FinishTime = DateTime.Now.AddMinutes(Convert.ToDouble(orderToAdd.TotalServicesWaitingTime));
                    orderToAdd.Status = "Inprogress";
                    currentQueue.QueueStatus = "busy";

                    var addOrderResult = await unitOfWork.OrdersManager.CreateAsync(orderToAdd);

                    if (addOrderResult != null)
                    {
                        int addOrderServiceErrorCounter = 0;

                        for(int i = 0; i < orderToAdd.OrderServices.Count; i++)
                        {
                            var addOrderServiceResult =  await unitOfWork.OrderServicesManager.CreateAsync(orderToAdd.OrderServices[i]);

                            if(addOrderServiceResult == null)
                            {
                                addOrderServiceErrorCounter++;
                            }
                        }

                        if(addOrderServiceErrorCounter == 0)
                        {
                            await unitOfWork.SaveChangesAsync();
                            await SetQueueWaitingTimes(); // adjust queue time
                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Error adding order services !");
                            return result;
                        }
                    }
                    else
                    {
                       result.Succeeded = false;
                       result.Errors.Add("Failed to add order !");
                       return result;

                    }
                }
                else // Queue has orders
                {
                    // Calculate order waiting time from last order in queue
                    waitingTime = currentQueue.Orders[currentQueue.Orders.Count-1].FinishTime.Value - orderToAdd.OrderDate;
                    if (waitingTime.HasValue)
                    {
                        orderToAdd.WaitingTimeInMinutes = Convert.ToInt32(waitingTime.Value.TotalMinutes);
                        orderToAdd.FinishTime = currentQueue.Orders[currentQueue.Orders.Count - 1].FinishTime.Value.AddMinutes(Convert.ToDouble(orderToAdd.WaitingTimeInMinutes.Value));
                    }
                    orderToAdd.Status = "Pending";
                    var addOrderResult = await unitOfWork.OrdersManager.CreateAsync(orderToAdd);
                    if (addOrderResult != null)
                    {
                        int addOrderServiceErrorCounter = 0;

                        for (int i = 0; i < orderToAdd.OrderServices.Count; i++)
                        {
                            var addOrderServiceResult = await unitOfWork.OrderServicesManager.CreateAsync(orderToAdd.OrderServices[i]);

                            if (addOrderServiceResult == null)
                            {
                                addOrderServiceErrorCounter++;
                            }
                        }

                        if (addOrderServiceErrorCounter == 0)
                        {
                            await unitOfWork.SaveChangesAsync();
                            await SetQueueWaitingTimes(); //adjust queue waiting time.
                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Error adding order services !");
                            return result;
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error creating Order !");
                        return result;
                    }
                } 
            } 
            catch(Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
        //Barber Queue order addition
        //public async Task<ApiResponse<bool>> AddOrderToQueue(OrderToAddDTO order)
        //{
        //    var result = new ApiResponse<bool>();
        //    BarberQueue Queue = new BarberQueue();
        //    Order currentOrder = new Order();

        //    currentOrder.CustomerId = order.CustomerId;
        //    currentOrder.BarberQueueId = order.BarberQueueId;

        //    currentOrder.OrderServices = mapper.Map<List<OrderService>>(order.OrderServices);
        //    currentOrder.TotalServicesWaitingTime = 0;
        //    currentOrder.OrderTotal = 0;



        //    for (int i = 0; i < currentOrder.OrderServices.Count; i++)
        //    {
        //        currentOrder.TotalServicesWaitingTime += currentOrder.OrderServices[i].Time;

        //        currentOrder.OrderTotal += currentOrder.OrderServices[i].Price;
        //        currentOrder.OrderServices[i].IsConfirmed = false;
        //    }


        //    try
        //    {
        //        var BarberQueue = await unitOfWork.BarbersQueuesManager.GetAsync(b => b.Id == currentOrder.BarberQueueId, 0, 0, null, includeProperties: "Orders");
        //        Queue = BarberQueue.FirstOrDefault();
        //        if (Queue != null)
        //        {
        //            if (Queue.Orders.Count == 0 && Queue.QueueStatus == "idle") // if queue is empty 
        //             {
        //                if (currentOrder.OrderDate.HasValue)
        //                {
        //                    currentOrder.OrderDate = DateTime.Now;
        //                }
        //                else
        //                {
        //                    currentOrder.OrderDate = DateTime.Now;
        //                }
        //                currentOrder.FinishTime = DateTime.Now.AddMinutes(Convert.ToDouble(currentOrder.TotalServicesWaitingTime));
        //                currentOrder.Status = "Inprogress";
        //                var customer = await unitOfWork.CustomersManager.GetByIdAsync(currentOrder.CustomerId);
        //                currentOrder.CustomerMobile = customer.PhoneNumber;
        //                currentOrder.CustomerName = customer.Name;

        //                var CreationResult = await unitOfWork.OrdersManager.CreateAsync(currentOrder);

        //              //  await unitOfWork.SaveChangesAsync();

        //                if (CreationResult != null)
        //                {
        //                    OrderService ServiceCreationResult;

        //                    for(int i=0; i<CreationResult.OrderServices.Count; i++)
        //                    {
        //                        ServiceCreationResult = await unitOfWork.OrderServicesManager.CreateAsync(currentOrder.OrderServices[i]);

        //                        if(ServiceCreationResult == null)
        //                        {
        //                            result.Succeeded = false;
        //                            return result;
        //                        }
        //                    }



        //                   BarberQueue queueToUpdateWaitingTime = await unitOfWork.BarbersQueuesManager.GetByIdAsync(CreationResult.BarberQueueId);


        //                    queueToUpdateWaitingTime.QueueStatus = "busy";

        //                    queueToUpdateWaitingTime.QueueWaitingTime = currentOrder.TotalServicesWaitingTime.Value;

        //                    var BarberQueueUpdateResult = await unitOfWork.BarbersQueuesManager.UpdateAsync(queueToUpdateWaitingTime);

        //                    var res = await unitOfWork.SaveChangesAsync();

        //                    if (BarberQueueUpdateResult == true)
        //                    {

        //                       var setQueueTimesResult =  await SetQueueWaitingTimes();

        //                        if (setQueueTimesResult.Data == true)
        //                        {
        //                            result.Succeeded = true;
        //                            result.Data = true;
        //                            return result;
        //                        }
        //                        else
        //                        {

        //                            result.Succeeded = false;
        //                            result.Errors.Add("Failed to update queue waiting times !");
        //                            result.Data = false;
        //                            return result;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        result.Succeeded = false;
        //                        result.Errors.Add("Unable to save changes");
        //                        return result;
        //                    }
        //                }
        //                else
        //                {
        //                    result.Succeeded = false;
        //                    return result;
        //                }
        //            }
        //            else
        //            {
        //                currentOrder.OrderDate = DateTime.Now;
        //                currentOrder.Status = "pending";
        //                var customer = await unitOfWork.CustomersManager.GetByIdAsync(currentOrder.CustomerId);
        //                currentOrder.CustomerMobile = customer.PhoneNumber;
        //                currentOrder.CustomerName = customer.Name;
        //                for (int i = 0; i < Queue.Orders.Count; i++)
        //                {
        //                    if(i == Queue.Orders.Count - 1)
        //                    {
        //                        currentOrder.FinishTime = Queue.Orders[i].FinishTime.Value.AddMinutes(Convert.ToDouble(currentOrder.TotalServicesWaitingTime));
        //                    }
        //                }
        //                var CreationResult = await unitOfWork.OrdersManager.CreateAsync(currentOrder);

        //                if (CreationResult != null)
        //                {
        //                    OrderService ServiceCreationResult;
        //                    for (int i = 0; i < CreationResult.OrderServices.Count; i++)
        //                    {
        //                        ServiceCreationResult = await unitOfWork.OrderServicesManager.CreateAsync(currentOrder.OrderServices[i]);
        //                    }
        //                    await unitOfWork.SaveChangesAsync();
        //                    await SetQueueWaitingTimes();
        //                    result.Succeeded = true;
        //                    result.Data = true;
        //                    return result;
        //                }
        //                else
        //                {
        //                    result.Succeeded = false;
        //                    return result;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            result.Succeeded = false;
        //            result.Errors.Add("Barber queue not found");
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Succeeded = false;
        //        result.Errors.Add(ex.Message);
        //        return result;
        //    }
        //}

        public async Task<ApiResponse<QueueTimeHandlerModel>> SetBarberQueueWaitingTime(int QueueId)
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

                        //end of else .

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

        public async Task<ApiResponse<bool>> SetQueueWaitingTimes()
        {
            var result = new ApiResponse<bool>();
            try
            {
                var barberQueuesToFetch = await unitOfWork.BarbersQueuesManager.GetAsync(includeProperties: "Orders");
                List<BarberQueue> barberQueue = barberQueuesToFetch.ToList();
                TimeSpan? waitingTime;
                if(barberQueue != null)
                {
                    int countErrors = 0;

                    //Iterate and set time per queue
                    for (int i = 0; i < barberQueue.Count; i++)
                    {
                        if(barberQueue[i].Orders.Count > 0) { // validate orders count in queue

                            if(DateTime.Now > barberQueue[i].Orders[barberQueue[i].Orders.Count - 1].FinishTime) // Last order finish time in queue is passed.
                            {
                                barberQueue[i].QueueWaitingTime = 0;
                                barberQueue[i].QueueStatus = "idle";

                                barberQueue[i].Orders.Clear();

                            }
                            else
                            {
                                waitingTime =  barberQueue[i].Orders[barberQueue[i].Orders.Count - 1].FinishTime - DateTime.Now ; // calculate waiting from last order finish time in queue
                                barberQueue[i].QueueWaitingTime = Convert.ToInt32(waitingTime.Value.TotalMinutes); // set waiting time in minutes.
                            }
                        }
                    }                
                    if(countErrors > 0 )
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error fetching Barber Queues!");
                        return result;
                     }
                    else
                    {
                        await unitOfWork.SaveChangesAsync();

                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                     


                }//End OF MAIN IF  . 
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error fetching Barber Queues");
                    return result;
                }

            }
            catch(Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }

        public async Task<ApiResponse<bool>> ReassignOrderToDifferentQueue(string orderId, string newQueueId)
        {
            OrderToReassign orderToReassign = new OrderToReassign();
            orderToReassign.OrderId = Int32.Parse(orderId);
            orderToReassign.NewQueueId = Int32.Parse(newQueueId);

            //Return Barber Name, barber status, barber queue status, total waiting time (orders)

            var result = new ApiResponse<bool>();


            Order orderToUpdate = new Order();
            OrderToAddDTO orderToCreate = new OrderToAddDTO();
            try
            {
                var orderToFetch = await unitOfWork.OrdersManager.GetAsync(b=> b.Id == orderToReassign.OrderId, includeProperties: "OrderServices");
                var order = orderToFetch.FirstOrDefault();

                if (order != null)
                {
                    orderToCreate.BarberQueueId = orderToReassign.NewQueueId.Value;
                    orderToCreate.CustomerId = order.CustomerId;
                    orderToCreate.OrderServices = mapper.Map<List<OrderServiceToAddDTO>>(order.OrderServices);


                    var isRemoved = await RemoveOrder(order.Id, order.CustomerId);
                    if (isRemoved.Succeeded)
                    {
                        var orderAdditionResult = await AddOrderToQueue(orderToCreate);
                        if (orderAdditionResult.Succeeded)
                        {
                            result.Data = true;
                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            result.Errors.Add("Error adding order to new queue !");
                            result.Succeeded = false;
                            return result;
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error removing order from original queue !");
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

        public async Task<ApiResponse<string>> RemoveOrder(int orderId, int customerId)
        {
            //Cancel Order: set order status to cancelled, pop order from corresponding queue.

            ApiResponse<string> result = new ApiResponse<string>();

            try
            {
                var order = await unitOfWork.OrdersManager.GetAsync(b => b.Id == orderId, 0, 0, null, includeProperties: "OrderServices");
                Order OrderToUpdate = order.FirstOrDefault();
                if (OrderToUpdate != null)
                {
                    var barberQueue = await unitOfWork.BarbersQueuesManager.GetAsync(b => b.Id == OrderToUpdate.BarberQueueId, 0, 0, null, includeProperties: "Orders");
                    if (barberQueue != null)
                    {
                        BarberQueue QueueToUpdate = barberQueue.FirstOrDefault();
                        for (int i = 0; i < QueueToUpdate.Orders.Count; i++)
                        {
                            if (QueueToUpdate.Orders[i].Id == orderId)
                            {
                                QueueToUpdate.Orders.Remove(QueueToUpdate.Orders[i]);
                            }
                        }
                        if (QueueToUpdate.Orders.Count == 0)
                        {
                            QueueToUpdate.QueueStatus = "idle";
                            QueueToUpdate.QueueWaitingTime = 0;
                        }

                        var queueUpdateRes = await unitOfWork.BarbersQueuesManager.UpdateAsync(QueueToUpdate);
                        if (queueUpdateRes)
                        {
                           var queueAdjustmentRes = await SetQueueWaitingTimes();
                           await unitOfWork.SaveChangesAsync();
                            if (queueAdjustmentRes.Succeeded)
                            {
                                 result.Data = "Order cancelled successfully.";
                                 result.Succeeded = true;
                                 return result;
                            }
                            else
                            {
                                result.Errors.Add("Error adjusting queue times");
                                result.Succeeded = false;
                                return result;
                            }
                        }
                        else
                        {
                            result.Errors.Add("Error updating barber queue !");
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
                }
                else
                {
                    result.Data = "Error.";
                    result.Errors.Add("Error fetching customer details !");
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

