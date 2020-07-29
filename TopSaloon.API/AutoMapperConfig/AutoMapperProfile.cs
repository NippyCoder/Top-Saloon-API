using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;

namespace TopSaloon.API.AutoMapperConfig
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<Administrator, AdministratorDTO>().ReverseMap();
            CreateMap<Barber, BarberDTO>().ForMember(dest => dest.Shop, opt=> opt.Ignore()).ReverseMap();
            CreateMap<Order, OrderDTO>().ForMember(dest => dest.OrderFeedback, opt => opt.Ignore()).ReverseMap();
            CreateMap<OrderService, OrderServiceDTO>().ForMember(dest => dest.Order, opt => opt.Ignore()).ReverseMap();
            CreateMap<OrderFeedback, OrderFeedbackDTO>().ForMember(dest => dest.Order, opt => opt.Ignore()).ReverseMap();

        }
    }
}
