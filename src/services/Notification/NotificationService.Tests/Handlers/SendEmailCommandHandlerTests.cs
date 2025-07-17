using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Application.Commands;
using NotificationService.Application.Handlers.Commands;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Tests.Handlers;

public class SendEmailCommandHandlerTests
{
    private readonly Mock<IEmailProvider> _mockEmailProvider;
    private readonly Mock<INotificationLogRepository> _mockLogRepository;
    private readonly Mock<ILogger<SendEmailCommandHandler>> _mockLogger;
    private readonly SendEmailCommandHandler _handler;

    public SendEmailCommandHandlerTests()
    {
        _mockEmailProvider = new Mock<IEmailProvider>();
        _mockLogRepository = new Mock<INotificationLogRepository>();
        _mockLogger = new Mock<ILogger<SendEmailCommandHandler>>();
        _handler = new SendEmailCommandHandler(_mockEmailProvider.Object, _mockLogRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidEmailCommand_ShouldReturnTrue()
    {
        // Arrange
        var command = new SendEmailCommand
        {
            To = "test@example.com",
            Subject = "Test Subject",
            Body = "Test Body",
            UserId = "user123"
        };

        _mockEmailProvider
            .Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, null))
            .ReturnsAsync(true);

        _mockLogRepository
            .Setup(x => x.CreateAsync(It.IsAny<NotificationLog>()))
            .ReturnsAsync((NotificationLog log) => log);

        _mockLogRepository
            .Setup(x => x.UpdateAsync(It.IsAny<NotificationLog>()))
            .ReturnsAsync((NotificationLog log) => log);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        
        _mockEmailProvider.Verify(
            x => x.SendEmailAsync(command.To, command.Subject, command.Body, null, null),
            Times.Once);

        _mockLogRepository.Verify(
            x => x.CreateAsync(It.Is<NotificationLog>(log => 
                log.UserId == command.UserId &&
                log.Type == NotificationType.Email &&
                log.Recipient == command.To &&
                log.Status == NotificationStatus.Pending)),
            Times.Once);

        _mockLogRepository.Verify(
            x => x.UpdateAsync(It.Is<NotificationLog>(log => 
                log.Status == NotificationStatus.Sent)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmailProviderFails_ShouldReturnFalse()
    {
        // Arrange
        var command = new SendEmailCommand
        {
            To = "test@example.com",
            Subject = "Test Subject",
            Body = "Test Body",
            UserId = "user123"
        };

        _mockEmailProvider
            .Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, null))
            .ReturnsAsync(false);

        _mockLogRepository
            .Setup(x => x.CreateAsync(It.IsAny<NotificationLog>()))
            .ReturnsAsync((NotificationLog log) => log);

        _mockLogRepository
            .Setup(x => x.UpdateAsync(It.IsAny<NotificationLog>()))
            .ReturnsAsync((NotificationLog log) => log);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();

        _mockLogRepository.Verify(
            x => x.UpdateAsync(It.Is<NotificationLog>(log => 
                log.Status == NotificationStatus.Failed &&
                log.ErrorMessage == "Failed to send email through provider")),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmailProviderThrowsException_ShouldThrowAndLogFailure()
    {
        // Arrange
        var command = new SendEmailCommand
        {
            To = "test@example.com",
            Subject = "Test Subject",
            Body = "Test Body",
            UserId = "user123"
        };

        var expectedException = new Exception("Provider error");

        _mockEmailProvider
            .Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, null))
            .ThrowsAsync(expectedException);

        _mockLogRepository
            .Setup(x => x.CreateAsync(It.IsAny<NotificationLog>()))
            .ReturnsAsync((NotificationLog log) => log);

        _mockLogRepository
            .Setup(x => x.UpdateAsync(It.IsAny<NotificationLog>()))
            .ReturnsAsync((NotificationLog log) => log);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

        _mockLogRepository.Verify(
            x => x.UpdateAsync(It.Is<NotificationLog>(log => 
                log.Status == NotificationStatus.Failed &&
                log.ErrorMessage == "Provider error")),
            Times.Once);
    }
}
