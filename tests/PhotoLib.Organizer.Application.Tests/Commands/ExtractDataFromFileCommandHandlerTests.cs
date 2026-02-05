using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Organizer.Application.Commands;
using Organizer.Application.Services;
using Organizer.Domain.Entities;
using Organizer.Infrastructure.Persistence;

namespace PhotoLib.Organizer.Application.Tests.Commands;

public class ExtractDataFromFileCommandHandlerTests
{
    private readonly Mock<IFileDataExtractorService> _fileDataExtractorService;
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<ExtractDataFromFileCommandHandler>> _logger;
    private readonly ExtractDataFromFileCommandHandler _sut;

    public ExtractDataFromFileCommandHandlerTests()
    {
        _fileDataExtractorService = new Mock<IFileDataExtractorService>();
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<ExtractDataFromFileCommandHandler>>();
        // dbContext parameter was removed; if still required, add Mock<IApplicationDbContext>
        _sut = new ExtractDataFromFileCommandHandler(
            _fileDataExtractorService.Object,
            new Mock<IApplicationDbContext>().Object,
            _logger.Object,
            _mediator.Object);
    }

    [Fact]
    public async Task Handle_ExtractsFileDataAndSendsCreateOrUpdateCommand()
    {
        // Arrange
        var filePath = "/photos/test.jpg";
        var photoFile = new PhotoFile
        {
            Id = Guid.NewGuid(),
            FileName = "test.jpg",
            FilePath = filePath
        };
        var expectedId = Guid.NewGuid();

        _fileDataExtractorService
            .Setup(x => x.ExtractFileDataAsync(filePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(photoFile);

        _mediator
            .Setup(x => x.Send(It.Is<CreateOrUpdateFileCommand>(c => c.File == photoFile), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var command = new ExtractDataFromFileCommand { FilePath = filePath };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
        _fileDataExtractorService.Verify(x => x.ExtractFileDataAsync(filePath, It.IsAny<CancellationToken>()), Times.Once);
        _mediator.Verify(x => x.Send(It.Is<CreateOrUpdateFileCommand>(c => c.File == photoFile), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSaveFails_ReturnsEmptyGuid()
    {
        // Arrange
        var filePath = "/photos/test.jpg";
        var photoFile = new PhotoFile
        {
            FileName = "test.jpg",
            FilePath = filePath
        };

        _fileDataExtractorService
            .Setup(x => x.ExtractFileDataAsync(filePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(photoFile);

        _mediator
            .Setup(x => x.Send(It.IsAny<CreateOrUpdateFileCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var command = new ExtractDataFromFileCommand { FilePath = filePath };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public async Task Handle_PassesCancellationTokenToExtractor()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        var filePath = "/photos/test.jpg";
        var photoFile = new PhotoFile { FileName = "test.jpg", FilePath = filePath };

        _fileDataExtractorService
            .Setup(x => x.ExtractFileDataAsync(filePath, token))
            .ReturnsAsync(photoFile);

        _mediator
            .Setup(x => x.Send(It.IsAny<CreateOrUpdateFileCommand>(), token))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        await _sut.Handle(new ExtractDataFromFileCommand { FilePath = filePath }, token);

        // Assert
        _fileDataExtractorService.Verify(x => x.ExtractFileDataAsync(filePath, token), Times.Once);
        _mediator.Verify(x => x.Send(It.IsAny<CreateOrUpdateFileCommand>(), token), Times.Once);
    }
}
