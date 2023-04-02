namespace Organizer.Application.Configuration;

public class OrganizerOptions
{
    public const string Key = "AppConfiguration:OrganizerConfiguration";

    public string[] DirectoriesToProcess { get; set; } = Array.Empty<string>();
}