using System.Globalization;
using System.Text.RegularExpressions;

namespace Organizer.Application.Services;

public class FileNameParser : IFileNameParser
{
    private readonly Regex regex = new(@"^\d{4}-\d{2}-\d{2} \d{2}-\d{2}-\d{2}$", RegexOptions.Compiled);
    private readonly Regex sixTagRegex = new(@"^6tag_\d{6}-\d{6}$", RegexOptions.Compiled);
    private readonly Regex whatsappRegex = new(@"^(\d{4}-\d{2}-\d{2} \d{2}-\d{2}-\d{2})_\d+$", RegexOptions.Compiled);
    private readonly Regex instaWheatherRegex = new(@"^InstaWeather(_o|_NW)?_(\d{2}_\d{2}_\d{2}_\d{2}-\d{2}-\d{4})$", RegexOptions.Compiled);

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
        else if (sixTagRegex.Match(fileNameWithoutExtension).Success)
        {
            var dateComponent = fileNameWithoutExtension.Substring(5, 13);
            if (DateTime.TryParseExact(dateComponent, "ddMMyy-HHmmss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date))
            {
                return date;
            }
        }
        else if (whatsappRegex.Match(fileNameWithoutExtension).Success)
        {
            var dateComponent = whatsappRegex.Match(fileNameWithoutExtension).Groups[1].Value;
            if (DateTime.TryParseExact(dateComponent, "yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date))
            {
                return date;
            }
        }
        else if (instaWheatherRegex.Match(fileNameWithoutExtension).Success)
        {
            var dateComponent = instaWheatherRegex.Match(fileNameWithoutExtension).Groups[2].Value;
            if (DateTime.TryParseExact(dateComponent, "HH_mm_ss_dd-MM-yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date))
            {
                return date;
            }
        }

        return null;
    }
}