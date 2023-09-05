using Hangfire;
using MediatR;
using FluentValidation;
using C4S.Services.Extensions;
using C4S.API.Extensions;
using С4S.API.Extensions;
using S4C.YandexGateway.DeveloperPage;
using C4S.Helpers.ApiHeplers.Swagger;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

#region services
var configuration = builder.Configuration;
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
    options.CustomSchemaIds(ShemaClassesIdsRenamer.Selector));
builder.Services.AddStorages(configuration);
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssemblies(typeof(Program).GetTypeInfo().Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddAutoMapper(
    typeof(Program),
    typeof(IDeveloperPageGetaway));
builder.Services.AddServices(configuration);
#endregion

var app = builder.Build();

#region middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard();
app.UseHttpsRedirection();
app.MapControllers();

/* TODO: чу тут с токеном делать*/
await app.InitApplicationInfrastructureAsync();

app.Run();
#endregion