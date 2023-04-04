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
        yield return new object[] { "6tag_170414-060046.JPG", new DateTime(2014,04,17,06,00,46) };
        yield return new object[] { "6tag_081213-193905.JPG", new DateTime(2013,12,08,19,39,05) };
        yield return new object[] { "2019-03-16 23-11-28_1554225763228.JPG", new DateTime(2019, 3, 16, 23, 11, 28) };
        yield return new object[] { "2020-02-08 15-07-06_1581277966266.MP4", new DateTime(2020, 2, 8, 15, 7, 6) };
        yield return new object[] { "InstaWeather_o_09_01_23_21-04-2014.JPG", new DateTime(2014, 04, 21,9,1,23) };
        yield return new object[] { "InstaWeather_NW_15_33_30_15-02-2014.JPG", new DateTime(2014, 02, 15,15,33,30) };
        yield return new object[] { "InstaWeather_12_48_00_18-01-2014.JPG", new DateTime(2014, 01, 18,12,48,00) };
    }
}