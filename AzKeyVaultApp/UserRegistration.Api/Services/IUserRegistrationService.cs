using UserRegistration.Api.Dtos;
using UserRegistration.Api.Models;

namespace UserRegistration.Api.Services;

public interface IUserRegistrationService
{
    Task<Registration> RegisterUser(Request request);
}
