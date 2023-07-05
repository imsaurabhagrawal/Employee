using EmployeeAPI.Data;
using EmployeeAPI.Models;
using EmployeeAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _dbContext;
        
        public EmployeeService(AppDbContext appDbContext) 
        {
            _dbContext = appDbContext;
            if (!_dbContext.Employees.Any())
            {
                _dbContext.Employees.AddRange(LoadInitialEmployeeData());
                _dbContext.SaveChanges();
            }
        }
        public async Task<bool?> DeleteEmployeeAsync(int id)
        {
            await AddDelay();
            var employee = (await GetEmployeeAsync())?.FirstOrDefault(x => x.Id == id);
            if (employee == null)
                return null;
            _dbContext.Employees.Remove(employee);
            var result = await _dbContext.SaveChangesAsync();
            if (result != null)
                return true;

            return false;
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            await AddDelay();
            return (await GetEmployeeAsync())?.FirstOrDefault(x => x.Id == id);
        }

        public async Task<bool> SaveEmployeeAsync(Employee employee)
        {
            await AddDelay();
            _dbContext.Employees.Add(employee);
            var result = await _dbContext.SaveChangesAsync();
            if (result > 0)
                return true;
            
            return false;
        }

        public async Task<bool?> UpdateEmployeeAsync(int id, Employee employee)
        {
            await AddDelay();
            var emp = (await GetEmployeeAsync())?.FirstOrDefault(x => x.Id == id);
            if (emp == null)
                return null;
            emp.FirstName = employee.FirstName;
            emp.LastName = employee.LastName;
            emp.Email = employee.Email;
            emp.Age = employee.Age;
            emp.Address = new Address { 
                                            Street = employee.Address.Street,
                                            City = employee.Address.City,
                                            State = employee.Address.State,
                                            Country = employee.Address.Country,
                                            PostalCode = employee.Address.PostalCode
                                        };
            
            var result = await _dbContext.SaveChangesAsync();
            if (result > 0)
                return true;

            return false;
        }

        public async Task<List<Employee>?> SearchEmployeeAsync(string searchString)
        {
            return (await GetEmployeeAsync())?.Where(x => x.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase) 
                                                        || x.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase))?
                                              .ToList();
        }

        public async Task<List<Employee>> GetEmployeeAsync()
        {
            await AddDelay();
            return _dbContext.Employees.Include(x => x.Address).ToList();
        }

        public bool? IsEmployeeExist(Employee employee, int? id = null)
        {
            if (id == null)
                return _dbContext.Employees?.Any(x => x.FirstName.Contains(employee.FirstName, StringComparison.OrdinalIgnoreCase)
                                                    && x.LastName.Contains(employee.LastName,StringComparison.OrdinalIgnoreCase)
                                                    && x.Email.Contains(employee.Email, StringComparison.OrdinalIgnoreCase));

            return _dbContext.Employees?.Any(x => x.FirstName.Contains(employee.FirstName, StringComparison.OrdinalIgnoreCase)
                                                    && x.LastName.Contains(employee.LastName, StringComparison.OrdinalIgnoreCase)
                                                    && x.Email.Contains(employee.Email, StringComparison.OrdinalIgnoreCase)
                                                    && x.Id != id);
        }

        private async Task AddDelay()
        {
            await Task.Delay(TimeSpan.FromSeconds(4));
        }

        private IEnumerable<Employee> LoadInitialEmployeeData()
        {
            return new List<Employee>
                                {
                                    new Employee
                                    {
                                        FirstName = "Saurabh",
                                        LastName = "Agrawal",Age = 30,
                                        Email = "Saurabh@test.com",
                                        Address = new Address
                                        {
                                            Street = "12 street",
                                            PostalCode = "123312",
                                            City = "Mumbai",
                                            State = "Maharashtra",
                                            Country = "India"
                                        }
                                    },
                                    new Employee
                                    {
                                        FirstName = "Ankit",
                                        LastName = "Agrawal",Age = 27,
                                        Email = "Ankit@test.com",
                                        Address = new Address
                                        {
                                            Street = "23 street",
                                            PostalCode = "324123",
                                            City = "Pune",
                                            State = "Maharashtra",
                                            Country = "India"
                                        }
                                    }
                                };
        }
    }
}
