using FunctionApp.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Services;

public class LocalEmailService : IEmailService
{
    private readonly ITemplateService _templateService;
    private readonly ILogger<LocalEmailService> _logger;
    private string _baseUrl = "";
    public LocalEmailService(ITemplateService templateService, ILogger<LocalEmailService> logger)
    {
        _templateService = templateService;
        _logger = logger;
        _baseUrl = Environment.GetEnvironmentVariable("BaseUrl") ?? "https://localhost:7071";
    }
    public async Task SendManagerApprovalEmailAsync(ManagerNotificationInput input)
    {
        var template = await _templateService.LoadTemplateAsync("Leave_ManagerNotification");
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

        var htmlContent = _templateService.ReplaceTokens(template, tokens);
        await SendToMailHog(input.ManagerEmail, $"{input.RequestType} Request - {input.EmployeeName}", htmlContent);
    }

    public async Task SendEmployeeNotificationEmailAsync(EmployeeNotificationInput input)
    {
        var template = await _templateService.LoadTemplateAsync("Leave_EmployeeNotification");
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

        var htmlContent = _templateService.ReplaceTokens(template, tokens);
        await SendToMailHog(input.EmployeeEmail, $"Your {input.RequestType} Request - {input.Status}", htmlContent);
    }

    private async Task SendToMailHog(string toEmail, string subject, string htmlContent)
    {
        using var client = new SmtpClient("localhost", 1025);
        var message = new MailMessage("noreply@company.com", toEmail, subject, htmlContent)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(message);
        _logger.LogInformation("Email sent to MailHog: {ToEmail} - {Subject}", toEmail, subject);
    }
}
