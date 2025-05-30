using Azure.Data.Tables;
using EmployeesApi.Models;

namespace EmployeesApi.Services;

public class AzStorageTablesService : IAzStorageTablesService
{
    private readonly TableClient _employeeTableClient;
    public AzStorageTablesService(TableServiceClient client)
    {
        client.CreateTableIfNotExists("Employee");
        _employeeTableClient = client.GetTableClient("Employee");
    }

    /// <summary>
    /// Retrieves all Employee entities from the table.
    /// </summary>
    /// <returns></returns>     
    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        var lstEmployees = new List<Employee>();
        var queryResult = _employeeTableClient.QueryAsync<Employee>();

        await foreach (var emp in queryResult.AsPages().ConfigureAwait(false))
        {
            lstEmployees.AddRange(emp.Values);
        }
        return lstEmployees;
    }

    /// <summary>
    /// Retrive a dictionary of employee count group by country.
    /// </summary>
    /// <returns></returns>
    public async Task<Dictionary<string, int>> GetEmployeeByCountryAsync()
    {
        var countryCount = new Dictionary<string, int>();
        var queryResult = _employeeTableClient.QueryAsync<Employee>();

        await foreach (var emp in queryResult.AsPages().ConfigureAwait(false))
        {
            foreach (var item in emp.Values)
            {
                string country = item.Address?.Split(',').LastOrDefault()?.Trim() ?? "Unknown";
                if (countryCount.ContainsKey(country))
                {
                    countryCount[country]++;
                }
                else
                {
                    countryCount[country] = 1;
                }
            }
        }
        return countryCount;
    }

    /// <summary>
    /// Asynchronously retrieves all employees and groups them by the first letter of their first name.
    /// </summary>
    /// <remarks>This method queries the employee data source and organizes the results into a dictionary
    /// where the keys are the first letters of employee first names, and the values are lists of employees whose first
    /// names start with the corresponding letter.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task result contains a dictionary where the keys are
    /// characters representing the first letter of employee first names, and the values are lists of <see
    /// cref="Employee"/> objects.</returns>
    public async Task<Dictionary<char, List<Employee>>> GetEmployeesGroupByFirstLetterFirstNameAsync()
    {
        var lstEmployees = new List<Employee>();
        var queryResult = _employeeTableClient.QueryAsync<Employee>();

        await foreach(var emp in queryResult.AsPages().ConfigureAwait(false))
        {
            lstEmployees.AddRange(emp.Values);
        }

        var groupedEmployees = lstEmployees
            .GroupBy(x => x.FirstName![0])
            .ToDictionary(g => g.Key, g => g.ToList());
        return groupedEmployees;
    }
    /// <summary>
    /// Retrieves a list of employees whose partition key starts with the specified letter.
    /// </summary>
    /// <param name="firstLetter">The first letter of the partition key to filter employees by. Must not be null or empty.</param>
    /// <returns>A list of <see cref="Employee"/> objects that match the specified partition key. Returns an empty list if no
    /// employees are found.</returns>
    public List<Employee> GetEmployeeStartingBy(string firstLetter)
    {
        var queryResult = _employeeTableClient.Query<Employee>(e=>e.PartitionKey == firstLetter);
        return queryResult.ToList();
    }

    /// <summary>
    /// Saves a list of employee entities to the table
    /// </summary>
    /// <param name="employees">The list of <see cref="Employee"/> to save.</param>
    /// <returns>A boolean indicating whether the operation was successful.</returns>
    public async Task<bool> SaveEmployeesAsync(List<Employee> employees)
    {
        foreach (var employee in employees)
        {
            await _employeeTableClient.AddEntityAsync(employee).ConfigureAwait(false);
        }
        return true;
    }
}


