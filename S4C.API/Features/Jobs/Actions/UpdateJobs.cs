using C4S.DB.Models.Hangfire;
using C4S.Services.Extensions;
using C4S.Services.Interfaces;
using FluentValidation;
using MediatR;

namespace C4S.API.Features.Jobs.Actions
{
    public class UpdateJobs
    {
        public class Command : IRequest<List<ResponseViewModel>>
        {
            /*TODO: почему я не могу передать HangfireJobModel*/
            public RequestViewModel[] UpdatedJobs { get; set; }
        }

        public class RequestViewModel
        {
            public HangfireJobTypeEnum JobType { get; set; }

            public string? CronExpression { get; set; }

            public bool IsEnable { get; set; }
        }

        public class ResponseViewModel
        {
            public HangfireJobTypeEnum JobType { get; set; }

            public string? Error { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x)
                    .Must(x => x.UpdatedJobs
                        .GroupBy(x => x.JobType)
                        .All(group => group.Count() == 1))
                    .WithMessage("Duplicate JobType detected");
            }
        }

        public class Handler : IRequestHandler<Command, List<ResponseViewModel>>
        {
            private readonly IBackGroundJobService _backgroundJobService;

            public Handler(IBackGroundJobService backGroundJobService)
            {
                _backgroundJobService = backGroundJobService;
            }

            public async Task<List<ResponseViewModel>> Handle(Command command, CancellationToken cancellationToken)
            {
                var responseViewModelList = new List<ResponseViewModel>();

                foreach (var updatedJob in command.UpdatedJobs)
                {
                    var responseVieModel = await CreateResponseAndUpdateRecurringJobAsync(updatedJob);
                    responseViewModelList.Add(responseVieModel);
                }

                return responseViewModelList;
            }

            private async Task<ResponseViewModel> CreateResponseAndUpdateRecurringJobAsync(RequestViewModel updatedJob)
            {
                var (errorMessage, isValidCron) = IsValidCronExpression(updatedJob.CronExpression);

                if (isValidCron)
                    await UpdateRecurringJobAsync(updatedJob);

                var responseViewModel = new ResponseViewModel
                {
                    JobType = updatedJob.JobType,
                    Error = errorMessage
                };

                return responseViewModel;
            }

            //TODO: уточнить, сейчас пользователь может оставить пустое значение для CronExpression и в таком случае IsEnable = false,
            //т.е.пользователь не должен иметь возможности седлать IsEnable = false и CronExpression = string.Empty
            private static (string?, bool) IsValidCronExpression(string? cronExpression)
            {
                var result = NCrontabExtenstions.TryParseWithNullOrEmpty(cronExpression, out var _);

                return result
                      ? (null, result) // TODO: уточнить нужно ли сообщение, о том что c пустым CronExpression джоба всегда будет выключена
                      : ("Invalid cron expression", result);
            }

            private async Task UpdateRecurringJobAsync(RequestViewModel updatedJob)
            {
                var hangfireJobConfiguration = new HangfireJobConfigurationModel(
                            updatedJob.JobType,
                            updatedJob.CronExpression,
                            updatedJob.IsEnable);

                await _backgroundJobService.UpdateRecurringJobAsync(hangfireJobConfiguration);
            }
        }
    }
}