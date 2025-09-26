using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Services;

public interface ITemplateService
{
    Task<string> LoadTemplateAsync(string templateName);
    string ReplaceTokens(string template, Dictionary<string, string> tokens);
}
public class TemplateService : ITemplateService
{
    public async Task<string> LoadTemplateAsync(string templateName)
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", $"{templateName}.html");
        return await File.ReadAllTextAsync(templatePath);
    }

    public string ReplaceTokens(string template, Dictionary<string, string> tokens)
    {
        foreach (var token in tokens)
        {
            template = template.Replace($"{{{{{token.Key}}}}}", token.Value);        
        }
        return template;
    }
}
