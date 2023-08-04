using Hangfire;
using S4C.API.Extensions;
using S4C.DB;

var builder = WebApplication.CreateBuilder(args);

#region services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddStorage(builder.Configuration);
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
    var dbContext = context.RequestServices.GetService(typeof(ReportDbContext)) as ReportDbContext ?? throw new Exception();
    await context.Response.WriteAsJsonAsync(dbContext.GameModels);
});

app.Run();
#endregion