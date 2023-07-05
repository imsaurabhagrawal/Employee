using EmployeeAPI.Setup;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.RegisterServiceConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
