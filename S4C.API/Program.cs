using Hangfire;
using MediatR;
using C4S.API.Extensions;
using C4S.Services.Extensions;
using C4S.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

#region services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddStorage(builder.Configuration);
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddServices();
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
    var service = context.RequestServices.GetService<IBackGroundJobService>() ?? throw new Exception();
    await service.AddOrUpdateRecurringJobAsync(null);
});

app.Run();
#endregion