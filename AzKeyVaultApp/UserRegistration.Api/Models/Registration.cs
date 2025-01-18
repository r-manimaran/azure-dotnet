namespace UserRegistration.Api.Models;

public class Registration
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedOn { get; set; }
}
