using AutoMapper;
using Microsoft.OpenApi.Extensions;
using C4S.DB.Models.Hangfire;

namespace C4S.API.Features.Jobs.ViewModels
{
    public class JobsViewModel
    {
        public string Name { get; set; }

        public HangfireJobTypeEnum JobType { get; set; }

        public string CronExpression { get; set; }

        public string Frequency { get; set; }

        public bool IsEnable { get; set; }
    }

    public class JobsViewModelProfiler : Profile
    {
        public JobsViewModelProfiler()
        {
            CreateMap<HangfireJobConfigurationModel, JobsViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.JopType.GetDisplayName()));
        }
    }
}
