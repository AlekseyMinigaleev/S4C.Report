using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using S4C.DB;
using S4C.DB.Enums;
using S4C.DB.Models;

var builder = WebApplication.CreateBuilder(args);

#region services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("ReportDbDev");
builder.Services.AddDbContext<ReportDbContext>(options=>
    options.UseSqlServer(connectionString));

#endregion

var app = builder.Build();

#region middleware

app.UseSwagger();

app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run( async context =>
    {
        var dbContext = context.RequestServices.GetService(typeof(ReportDbContext)) as ReportDbContext ?? throw new Exception();
        await context.Response.WriteAsJsonAsync(dbContext.GameModels);
    });
#endregion