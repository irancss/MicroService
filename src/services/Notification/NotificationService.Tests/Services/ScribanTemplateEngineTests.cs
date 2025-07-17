using FluentAssertions;
using NotificationService.Infrastructure.Services;

namespace NotificationService.Tests.Services;

public class ScribanTemplateEngineTests
{
    private readonly ScribanTemplateEngine _templateEngine;

    public ScribanTemplateEngineTests()
    {
        _templateEngine = new ScribanTemplateEngine();
    }

    [Fact]
    public void ProcessTemplate_WithSimpleVariable_ShouldReplaceCorrectly()
    {
        // Arrange
        var template = "Hello {{ firstName }}!";
        var parameters = new Dictionary<string, object>
        {
            { "firstName", "John" }
        };

        // Act
        var result = _templateEngine.ProcessTemplate(template, parameters);

        // Assert
        result.Should().Be("Hello John!");
    }

    [Fact]
    public void ProcessTemplate_WithMultipleVariables_ShouldReplaceAll()
    {
        // Arrange
        var template = "Hello {{ firstName }} {{ lastName }}! Your order #{{ orderId }} for ${{ amount }} is confirmed.";
        var parameters = new Dictionary<string, object>
        {
            { "firstName", "John" },
            { "lastName", "Doe" },
            { "orderId", "12345" },
            { "amount", 99.99 }
        };

        // Act
        var result = _templateEngine.ProcessTemplate(template, parameters);

        // Assert
        result.Should().Be("Hello John Doe! Your order #12345 for $99.99 is confirmed.");
    }

    [Fact]
    public void ProcessTemplate_WithDateFormatting_ShouldFormatCorrectly()
    {
        // Arrange
        var template = "Registration date: {{ registeredAt | date.to_string '%Y-%m-%d' }}";
        var parameters = new Dictionary<string, object>
        {
            { "registeredAt", new DateTime(2025, 7, 13, 10, 30, 0) }
        };

        // Act
        var result = _templateEngine.ProcessTemplate(template, parameters);

        // Assert
        result.Should().Be("Registration date: 2025-07-13");
    }

    [Fact]
    public void ProcessTemplate_WithMissingVariable_ShouldHandleGracefully()
    {
        // Arrange
        var template = "Hello {{ firstName }} {{ lastName }}!";
        var parameters = new Dictionary<string, object>
        {
            { "firstName", "John" }
            // lastName is missing
        };

        // Act
        var result = _templateEngine.ProcessTemplate(template, parameters);

        // Assert
        result.Should().Be("Hello John !");
    }

    [Fact]
    public void ProcessTemplate_WithComplexEmailTemplate_ShouldProcessCorrectly()
    {
        // Arrange
        var template = @"
            <h1>Welcome {{ firstName }} {{ lastName }}!</h1>
            <p>Thank you for registering with us on {{ registeredAt | date.to_string '%Y-%m-%d' }}.</p>
            <p>We're excited to have you as part of our {{ company_name }} community!</p>
            <p>Best regards,<br>The {{ company_name }} Team</p>
        ";

        var parameters = new Dictionary<string, object>
        {
            { "firstName", "Ahmad" },
            { "lastName", "Rezaei" },
            { "registeredAt", new DateTime(2025, 7, 13) },
            { "company_name", "TechCorp" }
        };

        // Act
        var result = _templateEngine.ProcessTemplate(template, parameters);

        // Assert
        result.Should().Contain("Welcome Ahmad Rezaei!");
        result.Should().Contain("2025-07-13");
        result.Should().Contain("TechCorp community");
        result.Should().Contain("The TechCorp Team");
    }
}
