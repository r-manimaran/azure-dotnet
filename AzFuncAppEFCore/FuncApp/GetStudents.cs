using FuncApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FuncApp;

public class GetStudents
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetStudents> _logger;

    public GetStudents(AppDbContext dbContext, ILogger<GetStudents> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [Function("GetStudents")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request for GetStudents");
        try
        {
            var students = await _dbContext.Students.ToListAsync();
            //var students = new List<Student>
            //{
            //    new Student { Id = 1, FirstName = "John Doe", LastName="Doe", Email="johnd@email.com" },
            //    new Student { Id = 2, FirstName = "Jane Smith", LastName="Smith", Email="johns@email.com" },
            //    new Student { Id = 3, FirstName = "Sam Brown", LastName="Brown", Email="samb@email.com" }
            //};

            return new OkObjectResult(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }        
    }
}