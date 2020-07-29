using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.Entities.Models;
using TopSaloon.DTOs.Models;

namespace TopSaloon.API.AutoMapperConfig
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // CreateMap<TestEntity, TestEntityDTO>().ReverseMap();
            CreateMap<Order, OrderTimeDTO>().ReverseMap();
            CreateMap<OrderService, OrderTimeServiceDTO>().ReverseMap();
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<OrderFeedback, OrderFeedbackDTO>().ReverseMap();
            CreateMap<OrderFeedbackQuestion, OrderFeedbackQuestionDTO>().ReverseMap();

        }
    }
}
