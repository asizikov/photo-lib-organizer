using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Organizer.Application.Commands;
using Organizer.Application.Configuration;
using Organizer.Application.Services;

namespace PhotoLib.Organizer.Application.Tests.Services;

public class WorkflowServiceTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<WorkflowService>> _logger;
    private readonly Mock<IHostApplicationLifetime> _hostApplicationLifetime;

    public WorkflowServiceTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<WorkflowService>>();
        _hostApplicationLifetime = new Mock<IHostApplicationLifetime>();
    }

    private WorkflowService CreateService(string[] directories)
    {
        var options = Options.Create(new OrganizerOptions
        {
            DirectoriesToProcess = directories
        });
        return new WorkflowService(_mediator.Object, _logger.Object, _hostApplicationLifetime.Object, options);
    }

    [Fact]
    public async Task ExecuteAsync_SendsBuildIndexCommandForEachDirectory()
    {
        // Arrange
        var dirs = new[] { "/photos/dir1", "/photos/dir2" };
        var service = CreateService(dirs);

        _mediator
            .Setup(x => x.Send(It.IsAny<BuildIndexCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mediator
            .Setup(x => x.Send(It.IsAny<UpdateFileCreatedDatesCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        using var cts = new CancellationTokenSource();

        // Act
        await service.StartAsync(cts.Token);
        // Give the background service time to complete
        await Task.Delay(500);

        // Assert
        _mediator.Verify(
            x => x.Send(It.Is<BuildIndexCommand>(c => c.SourceDirectory == "/photos/dir1"), It.IsAny<CancellationToken>()),
            Times.Once);
        _mediator.Verify(
            x => x.Send(It.Is<BuildIndexCommand>(c => c.SourceDirectory == "/photos/dir2"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_SendsUpdateFileCreatedDatesCommand()
    {
        // Arrange
        var service = CreateService(new[] { "/photos/dir1" });

        _mediator
            .Setup(x => x.Send(It.IsAny<BuildIndexCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mediator
            .Setup(x => x.Send(It.IsAny<UpdateFileCreatedDatesCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        using var cts = new CancellationTokenSource();

        // Act
        await service.StartAsync(cts.Token);
        await Task.Delay(500);

        // Assert
        _mediator.Verify(
            x => x.Send(It.IsAny<UpdateFileCreatedDatesCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_StopsApplicationAfterCompletion()
    {
        // Arrange
        var service = CreateService(Array.Empty<string>());

        _mediator
            .Setup(x => x.Send(It.IsAny<UpdateFileCreatedDatesCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        using var cts = new CancellationTokenSource();

        // Act
        await service.StartAsync(cts.Token);
        await Task.Delay(500);

        // Assert
        _hostApplicationLifetime.Verify(x => x.StopApplication(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoDirectories_StillSendsUpdateAndStops()
    {
        // Arrange
        var service = CreateService(Array.Empty<string>());

        _mediator
            .Setup(x => x.Send(It.IsAny<UpdateFileCreatedDatesCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        using var cts = new CancellationTokenSource();

        // Act
        await service.StartAsync(cts.Token);
        await Task.Delay(500);

        // Assert
        _mediator.Verify(x => x.Send(It.IsAny<BuildIndexCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        _mediator.Verify(x => x.Send(It.IsAny<UpdateFileCreatedDatesCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        _hostApplicationLifetime.Verify(x => x.StopApplication(), Times.Once);
    }
}
