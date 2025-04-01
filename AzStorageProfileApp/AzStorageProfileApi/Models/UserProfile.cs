namespace AzStorageProfileApi.Models;

public class UserProfile
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string ProfileImageUrl { get; set; } // This will store the blob URL
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
