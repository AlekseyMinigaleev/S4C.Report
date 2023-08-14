using MediatR;
using NCrontab;
using C4S.DB;
using C4S.DB.Models.Hangfire;
using C4S.Services.Interfaces;

namespace C4S.API.Features.Jobs.Actions
{
    public class UpdateJobs
    {
        public class Command : IRequest<List<ResponseViewModel>>
        {
            public HangfireJobConfigurationModel[] UpdatedJobs { get; set; }
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

            public Exception Error { get; set; }
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
                        JobType = updatedJob.JopType
                    };

                    var validationResult = CrontabSchedule.TryParse(updatedJob.CronExpression);

                    if (updatedJob.CronExpression is null || validationResult == null)
                    {
                        responseViewModel.Error = new CrontabException("Invalid cron expression");
                        await _backgroundJobService.AddOrUpdateRecurringJobAsync(updatedJob);
                    }

                    responseViewModelList.Add(responseViewModel);
                }

                return responseViewModelList;
            }
        }
    }
}