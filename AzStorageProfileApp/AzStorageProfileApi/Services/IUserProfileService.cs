using AzStorageProfileApi.Dtos;
using AzStorageProfileApi.Models;

namespace AzStorageProfileApi.Services;

public interface IUserProfileService
{
    Task<UserProfile> CreateUserProfile(UserProfileCreateRequest request);
    Task<UserProfile> GetUserProfile(string email);

}
