using AutoMapper;
using StockReport.Domain;
using StockReport.Model;

namespace StockReport.Config
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<FilteredResult<Domain.StockReport>, FilteredResult<StockReportDto>>().ReverseMap();
            CreateMap<Domain.StockReport, StockReportDto>().ReverseMap();
        }
    }
}
