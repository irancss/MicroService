using NotificationService.Domain.Interfaces;
using Scriban;

namespace NotificationService.Infrastructure.Services;

public class ScribanTemplateEngine : ITemplateEngine
{
    public string ProcessTemplate(string template, Dictionary<string, object> parameters)
    {
        try
        {
            var scribanTemplate = Template.Parse(template);
            return scribanTemplate.Render(parameters);
        }
        catch (Exception ex)
        {
            // Log the error and return the original template
            // In production, you might want to handle this differently
            return template;
        }
    }
}
