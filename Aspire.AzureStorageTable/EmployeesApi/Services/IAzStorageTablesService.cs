using EmployeesApi.Models;

namespace EmployeesApi.Services;

public interface IAzStorageTablesService
{
    Task<List<Employee>> GetAllEmployeesAsync();
    Task<Dictionary<string, int>> GetEmployeeByCountryAsync();
    List<Employee> GetEmployeeStartingBy(string firstLetter);
    Task<Dictionary<char, List<Employee>>> GetEmployeesGroupByFirstLetterFirstNameAsync();
    Task<bool> SaveEmployeesAsync(List<Employee> employees);
}
