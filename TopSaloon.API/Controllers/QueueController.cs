using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.API.Controllers.Common;
using TopSaloon.DTOs.Models;
using TopSaloon.ServiceLayer;

namespace TopSaloon.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueueController : BaseResultHandlerController<QueueService>
    {
        public QueueController(QueueService _service) : base(_service)
        {

        }

        [HttpGet("GetBarberQueue/{barberId}")]
        public async Task<IActionResult> GetBarberQueue(int barberId)
        {
            return await GetResponseHandler(async () => await service.GetBarberQueueByBarberId(barberId));
        }

        [HttpPut("Re-AssignOrderToDifferentQueue/{orderId}")]

        public async Task<IActionResult> ReassignOrderToDifferentQueue(int orderId, int newBarberQueue)
        {
            return await EditItemResponseHandler(async () => await service.ReassignOrderToDifferentQueue(orderId, newBarberQueue));
        }

        [HttpGet("GetBarberQueueWaitingTime")]
        public async Task<IActionResult> GetBarberQueueWaitingTime(int QueueId)
        {
            return await GetResponseHandler(async () => await service.GetBarberQueueWaitingTime(QueueId));
        }
        [HttpPost("AddOrderToQueue")]
        public async Task<IActionResult> AddOrderToQueue(OrderToAddDTO order, int QueueId)
        {
            return await AddItemResponseHandler(async () => await service.AddOrderToQueue(order, QueueId));
        }
    }
}
