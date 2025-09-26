using Castle.Core.Logging;
using FunctionApp.DTOs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Services;

public interface IEmailService
{
    Task SendManagerApprovalEmailAsync(ManagerNotificationInput input);
    Task SendEmployeeNotificationEmailAsync(EmployeeNotificationInput input);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly ISendGridClient _sendGridClient;
    private readonly ITemplateService _templateService;
    private readonly string _baseUrl;
    public EmailService(ISendGridClient sendGridClient, ITemplateService templateService, ILogger<EmailService> logger)
    {
        _logger = logger;
        _sendGridClient = sendGridClient;
        _templateService = templateService;
        _baseUrl = Environment.GetEnvironmentVariable("BaseUrl") ?? "https://localhost:7071";
    }
    public async Task SendManagerApprovalEmailAsync(ManagerNotificationInput input)
    {
        var from = new EmailAddress("noreply@company.com", "Contoso");
        var subject = $"{input.RequestType} Request - {input.EmployeeName}";
        var to = new EmailAddress(input.ManagerEmail);
        var htmlTemplate = await _templateService.LoadTemplateAsync("Leave_ManagerNotification");
        var tokens = new Dictionary<string, string>
        {
            { "RequestType", input.RequestType },
            { "EmployeeName", input.EmployeeName },
            { "StartDate", input.StartDate.ToString("yyyy-MM-dd") },
            { "EndDate", input.EndDate.ToString("yyyy-MM-dd") },
            { "TotalDays", input.TotalDays.ToString() },
            { "Reason", input.Reason },
            { "RequestId", input.RequestId },
            { "ApproveUrl", $"{_baseUrl}/api/ManagerApproval/ApproveLeave/{input.InstanceId}" },
            { "RejectUrl", $"{_baseUrl}/api/ManagerApproval/RejectLeave/{input.InstanceId}" }
        };
        var htmlContent = _templateService.ReplaceTokens(htmlTemplate, tokens);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);

        await _sendGridClient.SendEmailAsync(msg);
    }

    public async Task SendEmployeeNotificationEmailAsync(EmployeeNotificationInput input)
    {
        var from = new EmailAddress("noreply@company.com", "Contoso");
        var subject = $"{input.RequestType} Request - {input.Status}";
        var to = new EmailAddress(input.EmployeeEmail);
        var htmlTemplate = await _templateService.LoadTemplateAsync("Leave_EmployeeNotification");
        var statusColor = input.Status == "Approved" ? "#28a745" : "#dc3545";
        var tokens = new Dictionary<string, string>
        {
            { "RequestType", input.RequestType },
            { "EmployeeName", input.EmployeeName },
            { "Status", input.Status.ToUpper() },
            { "StatusColor", statusColor },
            { "StartDate", input.StartDate.ToString("yyyy-MM-dd") },
            { "EndDate", input.EndDate.ToString("yyyy-MM-dd") },
            { "Comments", string.IsNullOrEmpty(input.Comments) ? "" : $"<p><strong>Comments:</strong> {input.Comments}</p>" }
        };
        var htmlContent = _templateService.ReplaceTokens(htmlTemplate, tokens);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
        await _sendGridClient.SendEmailAsync(msg);
    }
}
