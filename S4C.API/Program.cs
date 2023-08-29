using Hangfire;
using MediatR;
using C4S.API.Extensions;
using C4S.Services.Extensions;
using C4S.ApiHelpers.Helpers.Swagger;
using FluentValidation;
using С4S.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

#region services
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.CustomSchemaIds(RenameSchemaClassesId.Selector));
builder.Services.AddStorage(builder.Configuration);
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddServices();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
#endregion

var app = builder.Build();

#region middleware

/*TODO: похоже на очень жестки костыль, пока не знаю как исправить*/
app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.InitApplicationAsync();

app.Run();
#endregion