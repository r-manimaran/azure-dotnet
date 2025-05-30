
using Azure.Data.Tables;
using Bogus;
using EmployeesApi.Models;
using EmployeesApi.Services;

namespace EmployeesApi.Endpoints;

public static class EmployeeEndpoints 
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api").WithOpenApi();

        MapGetAllEmployees(endpoints);
        MapGetAllEmployeesAsync(endpoints);
        MapGetAllEmployeesByFirstLetter(endpoints);
        MapGetEmployeesGroupByFirstNameAsync(endpoints);
        MapGenerateEmployees(endpoints);
    }

    

    private static void MapGetAllEmployees(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/GetEmployeesAsync", (IAzStorageTablesService service) => GetAllEmployeeAsync(service))
            .WithName("GetAllEmployees")
            .WithDescription("Get all employees from the table storage");
    }

    private static void MapGetAllEmployeesAsync(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/GetEmployeesByCountryAsync", (IAzStorageTablesService service) => GetEmployeeByCountryAsync(service))
            .WithName("GetEmployeesByCountry")
            .WithDescription("Get all employees grouped by country from the table storage")
            .WithTags("Async","Country");
    }

    private static void MapGetAllEmployeesByFirstLetter(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/employees/{firstLetter}", (string firstLetter, IAzStorageTablesService service) => GetEmployeesByFirstLetter(firstLetter, service))
            .WithName("GetEmployeesByFirstLetter")
            .WithDescription("Get all employees grouped by the first letter of their first name from the table storage")
            .WithTags("Async", "FirstLetter")
            .Produces<Dictionary<char, List<Employee>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);        
    }
    private static void MapGetEmployeesGroupByFirstNameAsync(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/GetEmployeesGroupByFirstNameAsync", (IAzStorageTablesService service) => GetEmployeesGroupByFirstNameAsync(service))
            .WithName("GetEmployeesGroupByFirstNameAsync")
            .WithDescription("Get all employees grouped by first name from the table storage")
            .WithTags("Async", "FirstName");
    }

    private static void MapGenerateEmployees(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/generate/{quantity?}", (int quantity, IAzStorageTablesService service) => MapGenerateEmployees(quantity, service))
            .WithName("GenerateEmployees")
            .WithDescription("Generate a specified number of employees and save them to the table storage")
            .WithTags("Async", "Generate")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    
    private static async Task<IResult> GetAllEmployeeAsync(IAzStorageTablesService service)
    {
        try
        {
            var employees = await service.GetAllEmployeesAsync();

            return Results.Ok(employees);
        }
        catch (Exception ex)
        {

            return Results.Problem(
                detail: ex.Message,
                title: "Error retrieving employees",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> GetEmployeeByCountryAsync(IAzStorageTablesService service)
    {
        try
        {
            var employeeByCountry = await service.GetEmployeeByCountryAsync();
            return Results.Ok(employeeByCountry);
        }    
    
        catch(Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                title: "Error retrieving employees by country",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static IResult GetEmployeesByFirstLetter(string firstLetter, IAzStorageTablesService service)
    {
        try
        {
            var employees = service.GetEmployeeStartingBy(firstLetter);
            return Results.Ok(employees);
        }
        catch (Exception ex)
        {

            return Results.Problem(
                detail: ex.Message,
                title: "Error retrieving employees by first letter",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> GetEmployeesGroupByFirstNameAsync(IAzStorageTablesService service)
    {
        try
        {
            var employees = await service.GetEmployeesGroupByFirstLetterFirstNameAsync();
            return Results.Ok(employees);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                title: "Error retrieving employees grouped by first name",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> MapGenerateEmployees(int? quantity, IAzStorageTablesService service)
    {
        try
        {   
            if (quantity <= 0)
            {
                return Results.BadRequest("Quantity must be greater than zero.");
            }
             var EmployeeFaker = new Faker<Employee>()
                .RuleFor(e=> e.RowKey, f=>f.Random.Guid().ToString())
                .RuleFor(e => e.FirstName,f=>f.Person.FirstName)
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e=> e.PartitionKey,(f,e) => e.LastName?.Substring(0, 1).ToUpper() ?? string.Empty)
                .RuleFor(e => e.Email, (f,e) => f.Internet.Email(e.FirstName,e.LastName))
                .RuleFor(e => e.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(e => e.Address, f => f.Address.FullAddress());

            int qty = quantity ?? 500; // Default to 500 if quantity is not provided

            var employees = EmployeeFaker.Generate(qty);

            var result = await service.SaveEmployeesAsync(employees);

            return result ? Results.Ok($"{employees.Count} employees generated") : Results.Problem("Failed to save employees.");
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                title: "Error generating employees",
                statusCode: StatusCodes.Status500InternalServerError);
        }        
    }


}
