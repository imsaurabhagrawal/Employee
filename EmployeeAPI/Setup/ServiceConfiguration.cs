using EmployeeAPI.Data;
using EmployeeAPI.Services;
using EmployeeAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Setup
{
    public static class ServiceConfiguration
    {
        public static void RegisterServiceConfiguration(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddDbContext<AppDbContext>();
        }
    }
}
