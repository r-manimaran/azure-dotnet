namespace AzStorageProfileApi.Dtos;

public class UserProfileCreateRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public IFormFile ProfileImage { get; set; }
}
