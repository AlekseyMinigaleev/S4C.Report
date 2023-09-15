using Hangfire;
using MediatR;
using FluentValidation;
using C4S.Services.Extensions;
using C4S.API.Extensions;
using S4C.YandexGateway.DeveloperPage;
using C4S.Helpers.ApiHeplers.Swagger;
using System.Reflection;
using Ñ4S.API.Extensions;
using C4S.Services.Interfaces;
using C4S.DB;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

#region services
services.AddHttpClient();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments($"{AppContext.BaseDirectory}C4S.API.xml");
    options.CustomSchemaIds(ShemaClassesIdsRenamer.Selector) ;
});
services.AddStorages(configuration);
services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssemblies(typeof(Program).GetTypeInfo().Assembly));
services.AddValidatorsFromAssemblyContaining<Program>();
services.AddValidatorsFromAssemblyContaining<ReportDbContext>();
services.AddAutoMapper(
    typeof(Program),
    typeof(IDeveloperPageGetaway),
    typeof(IGameDataService));
services.AddServices(configuration);
#endregion

var app = builder.BuildWithHangfireStorage(configuration);

#region middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard();
app.UseHttpsRedirection();
app.MapControllers();

await app.InitApplicationAsync();
app.Run();
#endregion