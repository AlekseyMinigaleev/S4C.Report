using Hangfire;
using MediatR;
using S4C.API.Extensions;
using S4C.DB;
using S4C.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

#region services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddStorage(builder.Configuration);
builder.Services.AddMediatR(typeof(Program));
#endregion

var app = builder.Build();

#region middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHangfireDashboard();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run(async context =>
{
    var backgroundJobService = context.RequestServices.GetService(typeof(IBackGroundJobService)) as IBackGroundJobService ?? throw new Exception();
    await backgroundJobService.InitJobsAsync();
});

app.Run();
#endregion