using AutoMapper;
using StockManagement.Domain;
using StockManagement.Model;

namespace StockManagement.Config
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<StockDetail, StockDetailDto>().ReverseMap();
        }
    }
}
