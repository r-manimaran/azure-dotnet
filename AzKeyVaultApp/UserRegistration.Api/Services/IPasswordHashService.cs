namespace UserRegistration.Api.Services;

public interface IPasswordHashService 
{
    string HashPassword(string password);
    bool Verify(string password, string passwordHash);
}
