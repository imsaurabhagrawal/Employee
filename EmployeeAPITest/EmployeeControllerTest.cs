using EmployeeAPI.Controllers.V1;
using EmployeeAPI.Models;
using EmployeeAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EmployeeAPITest
{
    public class EmployeeControllerTest
    {
        private Mock<ILogger<EmployeeController>> mockLogger;
        private Mock<IEmployeeService> mockEmployeeService;
        private EmployeeController sut;

        [SetUp]
        public void Setup()
        {
            mockLogger = new Mock<ILogger<EmployeeController>>();
            mockEmployeeService = new Mock<IEmployeeService>();
            sut = new EmployeeController(mockLogger.Object, mockEmployeeService.Object);
        }

        [Test]
        public void GetEmployeeById_WhenNull()
        {
            var result = sut.Get(null);
            mockEmployeeService.Verify(x => x.GetEmployeeByIdAsync(It.IsAny<int>()), Times.Never);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public void GetEmployeeById_WhenNotNull()
        {
            var result = sut.Get(1);
            mockEmployeeService.Verify(x => x.GetEmployeeByIdAsync(It.IsAny<int>()), Times.Once);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public void SaveEmployee_WhenNull()
        {
            var result = sut.Post(null);
            mockEmployeeService.Verify(x => x.SaveEmployeeAsync(It.IsAny<Employee>()), Times.Never);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public void SaveEmployee_WhenNotNull()
        {
            var emp = new Employee
            {
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@test.com",
                Age = 31,
                Address = new Address
                {
                    Street = "street",
                    City = "TestCity",
                    Country = "testCountry",
                    PostalCode = "213312",
                    State = "NY"
                }
            };
            mockEmployeeService.Setup(x => x.SaveEmployeeAsync(It.IsAny<Employee>())).ReturnsAsync(true);
            var result = sut.Post(emp);
            mockEmployeeService.Verify(x => x.SaveEmployeeAsync(It.IsAny<Employee>()), Times.Once);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        }

        [Test]
        public void UpdateEmployee_When_EmpIdNull()
        {
            var result = sut.Put(null, new Employee {
                                                      FirstName = "Test",
                                                      LastName = "Test",
                                                      Email = "Test@test.com",
                                                      Age = 31,
                                                      Address = new Address
                                                      {
                                                            Street = "street",
                                                            City = "TestCity",
                                                            Country = "testCountry",
                                                            PostalCode = "213312",
                                                            State = "NY"
                                                      }
                                                    });
            mockEmployeeService.Verify(x => x.UpdateEmployeeAsync(It.IsAny<int>(), It.IsAny<Employee>()), Times.Never);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public void UpdateEmployee_When_EmpObjectNull()
        {
            var result = sut.Put(2, null);
            mockEmployeeService.Verify(x => x.UpdateEmployeeAsync(It.IsAny<int>(), It.IsAny<Employee>()), Times.Never);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public void UpdateEmployee_WhenNotFound()
        {
            var empId = 2;
            var emp = new Employee
            {
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@test.com",
                Age = 31,
                Address = new Address
                {
                    Street = "street",
                    City = "TestCity",
                    Country = "testCountry",
                    PostalCode = "213312",
                    State = "NY"
                }
            };
            mockEmployeeService.Setup(x => x.UpdateEmployeeAsync(It.IsAny<int>(), It.IsAny<Employee>())).ReturnsAsync(() => null);
            var result = sut.Put(empId, emp);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public void UpdateEmployee_WhenNotNull()
        { 
            var empId = 2;
            var emp = new Employee
                                {
                                    FirstName = "Test",
                                    LastName = "Test",
                                    Email = "Test@test.com",
                                    Age = 31,
                                    Address = new Address
                                    {
                                        Street = "street",
                                        City = "TestCity",
                                        Country = "testCountry",
                                        PostalCode = "213312",
                                        State = "NY"
                                    }
                                };
            mockEmployeeService.Setup(x => x.UpdateEmployeeAsync(It.IsAny<int>(), It.IsAny<Employee>())).ReturnsAsync(true);
            var result = sut.Put(empId, emp);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public void DeleteEmployee_WhenNull()
        {
            var result = sut.Delete(null);
            mockEmployeeService.Verify(x => x.DeleteEmployeeAsync(It.IsAny<int>()), Times.Never);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public void DeleteEmployee_WhenNotFound()
        {
            mockEmployeeService.Setup(x => x.DeleteEmployeeAsync(It.IsAny<int>())).ReturnsAsync(() => null);
            var result = sut.Delete(2);
            mockEmployeeService.Verify(x => x.DeleteEmployeeAsync(It.IsAny<int>()), Times.Once);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public void DeleteEmployee_WhenNotNull()
        {
            var emp = new Employee
            {
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@test.com",
                Age = 31,
                Address = new Address
                {
                    Street = "street",
                    City = "TestCity",
                    Country = "testCountry",
                    PostalCode = "213312",
                    State = "NY"
                }
            };
            mockEmployeeService.Setup(x => x.SaveEmployeeAsync(It.IsAny<Employee>())).ReturnsAsync(true);
            var result = sut.Post(emp);
            mockEmployeeService.Verify(x => x.SaveEmployeeAsync(It.IsAny<Employee>()), Times.Once);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        }

        [Test]
        public void SearchEmployee_WhenNull()
        {
            var result = sut.Search(null);
            mockEmployeeService.Verify(x => x.SaveEmployeeAsync(It.IsAny<Employee>()), Times.Never);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public void SearchEmployee_WhenNotFound()
        {
            mockEmployeeService.Setup(x => x.SearchEmployeeAsync(It.IsAny<string>())).ReturnsAsync(() => null);
            var result = sut.Search("agrawal");
            mockEmployeeService.Verify(x => x.SearchEmployeeAsync(It.IsAny<string>()), Times.Once);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public void SearchEmployee_WhenNotNull()
        {
            var searchString = "Agrawal";
            mockEmployeeService.Setup(x => x.SearchEmployeeAsync(It.IsAny<string>())).ReturnsAsync(new List<Employee>() {new Employee
            {
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@test.com",
                Age = 31,
                Address = new Address
                {
                    Street = "street",
                    City = "TestCity",
                    Country = "testCountry",
                    PostalCode = "213312",
                    State = "NY"
                }
            } });
            var result = sut.Search(searchString);
            mockEmployeeService.Verify(x => x.SearchEmployeeAsync(It.IsAny<string>()), Times.Once);
            Assert.That(((ObjectResult)result.Result).StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }
    }
}