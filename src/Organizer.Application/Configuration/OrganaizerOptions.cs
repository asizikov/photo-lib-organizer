namespace Organizer.Application.Configuration;

public class OrganizerOptions
{
    public const string Key = "AppConfiguration:OrganizerConfiguration";

    public string DirectoryToProcess { get; set; } = string.Empty;
}