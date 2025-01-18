using FluentEmail.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UserRegistration.Api.Data;
using UserRegistration.Api.Dtos;
using UserRegistration.Api.Models;

namespace UserRegistration.Api.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly AppDbContext _dbContext;
        private readonly IFluentEmail _fluentEmail;
        private readonly IPasswordHashService _passwordHashService;
        private readonly EmailVerificationLinkService _emailVerificationLinkService;

        public UserRegistrationService(AppDbContext dbContext, 
                                       IFluentEmail fluentEmail, 
                                       IPasswordHashService passwordHashService,
                                       EmailVerificationLinkService emailVerificationLinkService)
        {
            _dbContext = dbContext;
            _fluentEmail = fluentEmail;
            _passwordHashService = passwordHashService;
            _emailVerificationLinkService = emailVerificationLinkService;
        }

        public async Task<Registration> RegisterUser(Request request)
        {
            if (await _dbContext.Registrations.AnyAsync(r=>r.Email.ToLower() == request.Email))
            {
                throw new ApplicationException("The Email is already registered. Try password reset and Login");
            }

            var registration = new Registration
            {
                Id = Guid.NewGuid(),
                Email = request.Email.Trim().ToLower(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                EmailConfirmed = false,
                PasswordHash = _passwordHashService.HashPassword(request.Password)
            };

            _dbContext.Registrations.Add(registration);

            DateTime utcNow = DateTime.UtcNow;
            var verificationToken = new EmailVerificationToken
            {
                Id = Guid.NewGuid(),
                UserId = registration.Id,
                CreatedOn = utcNow,
                ExpiresOn = utcNow.AddDays(1)
            };

            _dbContext.EmailVerificationTokens.Add(verificationToken);

            try
            {
                await _dbContext.SaveChangesAsync();
            }          
            catch(DbUpdateException ex) when (ex.InnerException is SqlException sqlEx &&
               (sqlEx.Number == 2627 || sqlEx.Number == 2601)) // Unique constraint violation error numbers
           
            {
                throw new Exception("The email is already in use", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving the registration", ex);
            }

            string verificationLink = _emailVerificationLinkService.Create(verificationToken);

            await _fluentEmail.To(registration.Email)
                .Subject("Email verification")
                .Body($"Thanks for the registration. To verify your email address <a href='{verificationLink}'>Click here</a>",isHtml:true)
                .SendAsync();

            return registration;
        }
    }
}
