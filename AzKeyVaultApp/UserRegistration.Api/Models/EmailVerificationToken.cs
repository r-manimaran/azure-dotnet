namespace UserRegistration.Api.Models;

public class EmailVerificationToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ExpiresOn { get; set; }
}
