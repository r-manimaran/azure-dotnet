using Dapper;
using EmailNotificatonTimer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailNotificatonTimer.Services;

public class EmployeeService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmployeeService> _logger;
    private readonly string _connectionString;
    public EmployeeService(IConfiguration config, ILogger<EmployeeService> logger)
    {
        _config = config;
        _logger = logger;
        _connectionString = _config.GetConnectionString("EmployeeDatabase") ?? 
                            Environment.GetEnvironmentVariable("DbConnectionString") ?? 
                            throw new InvalidOperationException("Connection string for Employee Database is not configured.");
    }

    public async Task InsertEmployeeAsync(Employee employee)
    {
        if (employee == null) throw new ArgumentNullException(nameof(employee), "Employee cannot be null.");
        // Simulate database insertion logic
        _logger.LogInformation("Inserting employee into the database: {EmployeeName}", employee.UserName);
       
        const string insertQuery = @"INSERT INTO Employees (UserName, Email, CreatedOn) 
                                     VALUES (@UserName, @Email, @CreatedOn)";
        try
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(insertQuery, employee);

            _logger.LogInformation("Employee {EmployeeName} inserted successfully into the database.", employee.UserName);
        }
        catch(Exception ex)
        {             
            _logger.LogError(ex, "Error inserting employee {EmployeeName} into the database.", employee.UserName);
            throw; // Re-throw the exception after logging
        }
        // Here you would typically use an ORM like Entity Framework or Dapper to insert the employee into the database.
        _logger.LogInformation("Employee {EmployeeName} inserted successfully.", employee.UserName);
    }
}
