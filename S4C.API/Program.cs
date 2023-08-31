using Hangfire;
using MediatR;
using C4S.API.Extensions;
using C4S.Services.Extensions;
using C4S.ApiHelpers.Helpers.Swagger;
using FluentValidation;
using Ñ4S.API.Extensions;
using S4C.YandexGateway.DeveloperPageGateway;

var builder = WebApplication.CreateBuilder(args);

#region services
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.CustomSchemaIds(RenameSchemaClassesId.Selector));
builder.Services.AddStorage(builder.Configuration);
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddAutoMapper(
    typeof(Program),
    typeof(IDeveloperPageGetaway)); 
builder.Services.AddServices();
#endregion

var app = builder.Build();

#region middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard();
app.UseHttpsRedirection();
app.MapControllers();

await app.InitApplicationAsync();

app.Run();
#endregion