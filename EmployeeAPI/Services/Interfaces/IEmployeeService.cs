using EmployeeAPI.Models;

namespace EmployeeAPI.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<bool> SaveEmployeeAsync(Employee employee);
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<bool?> UpdateEmployeeAsync(int id, Employee employee);
        Task<bool?> DeleteEmployeeAsync(int id);
        Task<List<Employee>> GetEmployeeAsync();
        Task<List<Employee>?> SearchEmployeeAsync(string searchString);
        bool? IsEmployeeExist(Employee employee, int? id = null);
    }
}
