using AzStorageProfileApi.Dtos;
using AzStorageProfileApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AzStorageProfileApi.Services;

public class UserProfileService : IUserProfileService
{
    private readonly AppDbContext _dbContext;
    private readonly IBlobStorageService _blobStorageService;
    private readonly ILogger<UserProfileService> _logger;
    private readonly IOptions<AzureBlobStorageSettings> _blobStorageSettings;
    
    public UserProfileService(AppDbContext dbContext, 
                              IBlobStorageService blobStorageService,
                              ILogger<UserProfileService> logger,
                              IOptions<AzureBlobStorageSettings> blobStorageSettings)
    {
        _dbContext = dbContext;
        _blobStorageService = blobStorageService;
        _logger = logger;
        _blobStorageSettings = blobStorageSettings;
    }

    public async Task<UserProfile> CreateUserProfile(UserProfileCreateRequest request)
    {
        _logger.LogInformation("Validating the request");

        _logger.LogInformation("Processing the Blob Storage upload");
        string blobImageUrl = await _blobStorageService.UploadFileAsync(request.ProfileImage, 
                                    _blobStorageSettings.Value.ProfileImageContainer,
                                    request.Email);

        _logger.LogInformation("Creating Profile request in Database");
        UserProfile newProfile = new UserProfile();
        newProfile.FirstName = request.FirstName;
        newProfile.LastName = request.LastName;
        newProfile.Email = request.Email;
        newProfile.ProfileImageUrl = blobImageUrl;
        newProfile.CreatedOn = DateTime.UtcNow;

        _dbContext.userProfiles.Add(newProfile);
        await _dbContext.SaveChangesAsync();

        return newProfile;
    }

    public async Task<UserProfile> GetUserProfile(string email)
    {
        var profile = await _dbContext.userProfiles.Where(t => t.Email == email).FirstOrDefaultAsync();

        if (profile == null)
            throw new ApplicationException($"Profile with the Email {email} not found.");
        

        string profileImageWithSAS = _blobStorageService.GenerateSasToken(profile.ProfileImageUrl, _blobStorageSettings.Value.ProfileImageContainer, TimeSpan.FromMinutes(60));

        profile.ProfileImageUrl = profileImageWithSAS;
        return profile;
    }
}
