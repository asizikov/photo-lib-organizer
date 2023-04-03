using Organizer.Application.Services;

namespace PhotoLib.Organizer.Application.Tests;

public class FileNameParserTests
{
    [Theory]
    [MemberData(nameof(GetFileNameAndDate))]
    public void ExtractsDateFromFileName(string fileName, DateTime? expectedDate)
    {
        var fileNameParser = new FileNameParser();
        var date = fileNameParser.ExtractDateFromFileName(fileName);
        Assert.Equal(expectedDate, date);
    }
    
    // Inline Data method
    public static IEnumerable<object[]> GetFileNameAndDate()
    {
        yield return new object[] { "2021-01-01 12-34-56.jpg", new DateTime(2021, 1, 1, 12, 34, 56) };
        yield return new object[] { "2020-03-02 22-34-55.MOV", new DateTime(2020, 3, 2, 22, 34, 55) };
        yield return new object[] { "2018-07-20 21-46-28.JPG", new DateTime(2018, 7, 20, 21, 46, 28) };
    }
}