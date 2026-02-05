using Organizer.Application.Services;

namespace PhotoLib.Organizer.Application.Tests.Services;

public class FileNameParserEdgeCaseTests
{
    private readonly FileNameParser _sut = new();

    [Theory]
    [InlineData("")]
    [InlineData("random-file.jpg")]
    [InlineData("IMG_1234.HEIC")]
    [InlineData("DSC00042.JPG")]
    [InlineData("screenshot.png")]
    [InlineData("no-date-here")]
    [InlineData(".hidden")]
    [InlineData("...")]
    public void ExtractDateFromFileName_UnrecognizedFormats_ReturnsNull(string fileName)
    {
        var result = _sut.ExtractDateFromFileName(fileName);
        Assert.Null(result);
    }

    [Fact]
    public void ExtractDateFromFileName_StandardFormat_WithPathSeparator()
    {
        // Path.GetFileNameWithoutExtension handles paths
        var result = _sut.ExtractDateFromFileName("2021-06-15 08-30-00.jpg");
        Assert.Equal(new DateTime(2021, 6, 15, 8, 30, 0), result);
    }

    [Theory]
    [InlineData("2020-12-31 23-59-59.jpg", 2020, 12, 31, 23, 59, 59)]
    [InlineData("2000-01-01 00-00-00.png", 2000, 1, 1, 0, 0, 0)]
    public void ExtractDateFromFileName_StandardFormat_BoundaryDates(
        string fileName, int y, int m, int d, int h, int min, int s)
    {
        var result = _sut.ExtractDateFromFileName(fileName);
        Assert.Equal(new DateTime(y, m, d, h, min, s), result);
    }

    [Theory]
    [InlineData("6tag_010120-120000.JPG", 2020, 1, 1, 12, 0, 0)]
    [InlineData("6tag_311219-235959.JPG", 2019, 12, 31, 23, 59, 59)]
    public void ExtractDateFromFileName_SixTagFormat_VariousDates(
        string fileName, int y, int m, int d, int h, int min, int s)
    {
        var result = _sut.ExtractDateFromFileName(fileName);
        Assert.Equal(new DateTime(y, m, d, h, min, s), result);
    }

    [Theory]
    [InlineData("2021-05-10 14-20-30_9999999999999.MP4", 2021, 5, 10, 14, 20, 30)]
    [InlineData("2019-12-25 00-00-01_1.JPG", 2019, 12, 25, 0, 0, 1)]
    public void ExtractDateFromFileName_WhatsAppFormat_VariousSuffixes(
        string fileName, int y, int m, int d, int h, int min, int s)
    {
        var result = _sut.ExtractDateFromFileName(fileName);
        Assert.Equal(new DateTime(y, m, d, h, min, s), result);
    }

    [Theory]
    [InlineData("InstaWeather_o_23_59_59_31-12-2020.JPG", 2020, 12, 31, 23, 59, 59)]
    [InlineData("InstaWeather_NW_00_00_00_01-01-2015.JPG", 2015, 1, 1, 0, 0, 0)]
    [InlineData("InstaWeather_12_00_00_15-06-2018.JPG", 2018, 6, 15, 12, 0, 0)]
    public void ExtractDateFromFileName_InstaWeatherFormat_VariousPrefixes(
        string fileName, int y, int m, int d, int h, int min, int s)
    {
        var result = _sut.ExtractDateFromFileName(fileName);
        Assert.Equal(new DateTime(y, m, d, h, min, s), result);
    }

    [Theory]
    [InlineData("photo_2023-11-05_10-30-45.jpg", 2023, 11, 5, 10, 30, 45)]
    [InlineData("photo_2000-01-01_00-00-00.png", 2000, 1, 1, 0, 0, 0)]
    public void ExtractDateFromFileName_PhotoFormat_VariousDates(
        string fileName, int y, int m, int d, int h, int min, int s)
    {
        var result = _sut.ExtractDateFromFileName(fileName);
        Assert.Equal(new DateTime(y, m, d, h, min, s), result);
    }

    [Theory]
    [InlineData("9999-99-99 99-99-99.jpg")]
    [InlineData("2021-13-01 00-00-00.jpg")]
    [InlineData("2021-00-01 00-00-00.jpg")]
    [InlineData("2021-01-32 00-00-00.jpg")]
    public void ExtractDateFromFileName_InvalidDateValues_ReturnsNull(string fileName)
    {
        var result = _sut.ExtractDateFromFileName(fileName);
        Assert.Null(result);
    }

    [Fact]
    public void ExtractDateFromFileName_FileWithNoExtension()
    {
        // "2021-01-01 12-34-56" with no extension
        var result = _sut.ExtractDateFromFileName("2021-01-01 12-34-56");
        Assert.Equal(new DateTime(2021, 1, 1, 12, 34, 56), result);
    }
}
