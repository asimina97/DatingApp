using API.Extensions;
using API.MiddleWares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//extensions
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);


var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>(); 
//Configure the HTTP request pipeline

app.UseCors(builder=>builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

//Bearer authentication should be placed there
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
