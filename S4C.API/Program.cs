using Hangfire;
using MediatR;
using C4S.API.Extensions;
using C4S.Services.Extensions;
using C4S.ApiHelpers.Helpers.Swagger;
using FluentValidation;
using C4S.ApiHelpers.Helpers;

var builder = WebApplication.CreateBuilder(args);

#region services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(RenameSchemaClassesId.Selector);
});
builder.Services.AddStorage(builder.Configuration);
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddServices();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
#endregion

var app = builder.Build();

#region middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHangfireDashboard();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//TODO: похоже на очень жесткий костыль, но я пока не понимаю как сделать по другому.
var jobServiceInitialized = false;
if(!jobServiceInitialized)
{
    app.Run(RequestDelegates.InitMissingHangfireJobs);
    jobServiceInitialized = true;
}


app.Run();
#endregion