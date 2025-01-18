namespace UserRegistration.Api.Dtos;

public sealed record Request(string Email, string FirstName,  string LastName, string Password);
