using System.Globalization;
using System.Text.RegularExpressions;

namespace Organizer.Application.Services;

public class FileNameParser : IFileNameParser
{
    private readonly Regex regex = new(@"^\d{4}-\d{2}-\d{2} \d{2}-\d{2}-\d{2}$", RegexOptions.Compiled);

    public DateTime? ExtractDateFromFileName(string fileName)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var match = regex.Match(fileNameWithoutExtension);
        if (match.Success)
        {
            if (DateTime.TryParseExact(match.Value, "yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date))
            {
                return date;
            }
        }

        return null;
    }
}