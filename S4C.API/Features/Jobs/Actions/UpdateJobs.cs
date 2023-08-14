using MediatR;
using NCrontab;
using C4S.DB;
using C4S.DB.Models.Hangfire;
using C4S.Services.Interfaces;
using FluentValidation;

namespace C4S.API.Features.Jobs.Actions
{
    public class UpdateJobs
    {
        public class Command : IRequest<List<ResponseViewModel>>
        {
            public RequestViewModel[] UpdatedJobs { get; set; }
        }

        public class RequestViewModel
        {
            public HangfireJobTypeEnum JobType { get; set; }

            public string CronExpression { get; set; }

            public bool IsEnable { get; set; }
        }

        public class ResponseViewModel
        {
            public HangfireJobTypeEnum JobType { get; set; }

            public string Error { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x)
                    .Must(x=>x.UpdatedJobs
                        .GroupBy(x => x.JobType)
                        .All(group => group.Count() == 1))
                    .WithMessage("Duplicate JobType detected");
            }
        }

        public class Handler : IRequestHandler<Command, List<ResponseViewModel>>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IBackGroundJobService _backgroundJobService;

            public Handler(ReportDbContext dbContext, IBackGroundJobService backGroundJobService)
            {
                _dbContext = dbContext;
                _backgroundJobService = backGroundJobService;
            }

            public async Task<List<ResponseViewModel>> Handle(Command command, CancellationToken cancellationToken)
            {
                var responseViewModelList = new List<ResponseViewModel>();

                foreach (var updatedJob in command.UpdatedJobs)
                {
                    var responseViewModel = new ResponseViewModel
                    {
                        JobType = updatedJob.JobType
                    };

                    var validationResult = CrontabSchedule.TryParse(updatedJob.CronExpression);

                    if (updatedJob.CronExpression is null || validationResult == null)
                    {
                        responseViewModel.Error = "Invalid cron expression";
                    }
                    else
                    {
                        var hangfireJobConfiguration = new HangfireJobConfigurationModel(updatedJob.JobType, updatedJob.CronExpression, updatedJob.IsEnable);
                        await _backgroundJobService.AddOrUpdateRecurringJobAsync(hangfireJobConfiguration);
                    }

                    responseViewModelList.Add(responseViewModel);
                }

                return responseViewModelList;
            }
        }
    }
}