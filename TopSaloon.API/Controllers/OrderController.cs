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
    public class OrderController : BaseResultHandlerController<OrdersService>
    {
        public OrderController(OrdersService _service) : base(_service)
        {

        }

        [HttpGet("GetOrderServices/{orderId}")]
        public async Task<IActionResult> GetOrderServicesViaOrderId(int orderId)
        {
            return await GetResponseHandler(async () => await service.GetOrderServicesViaOrderId(orderId));
        }

        [HttpPut("SetOrderService")]

        public async Task<IActionResult> SetOrderService(int orderServiceId)
        {
            return await EditItemResponseHandler(async () => await service.SetOrderService(orderServiceId));
        }

        [HttpPut("CancelOrder")]

        public async Task<IActionResult> CancelOrder(int orderId)
        {
            return await EditItemResponseHandler(async () => await service.CancelOrder(orderId));
        }
    }
}
