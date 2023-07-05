using Microsoft.AspNetCore.Mvc;
using EmployeeAPI.Models;
using EmployeeAPI.Services.Interfaces;
using Newtonsoft.Json;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace EmployeeAPI.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService) 
        {
            _logger = logger;
            _employeeService = employeeService;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Get([FromQuery]int? id)
        {
            try
            {
                if (id == null)
                    return new BadRequestObjectResult(new ErrorResponse { Message = "Id is required." });
                var result = await _employeeService.GetEmployeeByIdAsync((int)id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured while processing request for Id: {id} with exception :{ex.Message}", ex);
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                                new ErrorResponse
                                {
                                    Message = "Internal Server Error"
                                });
        }

        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status201Created, contentType: MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Post([FromBody] Employee employee)
        {
            try
            {
                if (employee == null)
                    return new BadRequestObjectResult(new ErrorResponse { Message = "Employee cannot be empty." });
            
                var isExist = _employeeService.IsEmployeeExist(employee);
                if (isExist.HasValue && isExist.Value)
                    return new BadRequestObjectResult(new ErrorResponse
                                                                    {
                                                                        Message = $"Employee with firstName: {employee.FirstName}, lastname: {employee.LastName} and email: {employee.Email} already exist"
                                                                    });

                var result = await _employeeService.SaveEmployeeAsync(employee);
                if (result)
                    return CreatedAtAction(nameof(Post), result);
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured while processing request for payload: {JsonConvert.SerializeObject(employee)} with exception :{ex.Message}", ex);
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                                new ErrorResponse
                                                {
                                                    Message = "Internal Server Error"
                                                });
        }

        [HttpPut]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound, contentType: MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Put(int? id, [FromBody] Employee employee)
        {
            try
            {
                if (id == null)
                    return new BadRequestObjectResult(new ErrorResponse { Message = "Id is required." });

                if (id == null || employee == null)
                    return new BadRequestObjectResult(new ErrorResponse { Message = "Employee cannot be empty." });

                var isExist = _employeeService.IsEmployeeExist(employee, id);
                if (isExist.HasValue && isExist.Value)
                    return new BadRequestObjectResult(new ErrorResponse
                                                        {
                                                            Message = $"Employee with firstName {employee.FirstName}, lastname {employee.LastName} and email {employee.Email} already exist"
                                                        });

                var result = await _employeeService.UpdateEmployeeAsync((int) id, employee);
                if (result == null)
                    return new NotFoundObjectResult(new ErrorResponse
                                                        {
                                                            Message = "Employee not found"
                                                        });
                if (result.HasValue && result.Value)
                    return Ok(result);
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured while processing request for payload: {JsonConvert.SerializeObject(employee)} with exception :{ex.Message}", ex);
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                                new ErrorResponse
                                {
                                    Message = "Internal Server Error"
                                });
        }

        [HttpDelete]
        [ProducesResponseType(typeof(bool),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound, contentType: MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                    return new BadRequestObjectResult(new ErrorResponse { Message = "Id is required." });

                var result = await _employeeService.DeleteEmployeeAsync((int)id);
                if (result == null)
                    return new NotFoundObjectResult(new ErrorResponse
                            {
                                Message = "Employee not found"
                            });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured while processing request for Id: {id} with exception :{ex.Message}", ex);
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                                new ErrorResponse
                                {
                                    Message = "Internal Server Error"
                                });
        }

        [HttpGet("Search")]
        [ProducesResponseType(typeof(IEnumerable<Employee>),StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound, contentType: MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Search([FromQuery] string searchString)
        {
            try
            {
                if (searchString == null)
                    return new BadRequestObjectResult(new ErrorResponse { Message = "Search string is required." });

                var result = await _employeeService.SearchEmployeeAsync(searchString);

                if (result == null)
                    return new NotFoundObjectResult(new ErrorResponse { Message = "Employee not found for provided search string"
                            });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured while processing request for search string: {searchString} with exception :{ex.Message}", ex);
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                                new ErrorResponse
                                {
                                    Message = "Internal Server Error"
                                });
        }
    }
}
