namespace Organizer.Domain.Entities;

public class PhotoFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public string FilePath { get; set; }
    public long? FileSize { get; set; }
    public string? FileDescription { get; set; }
    public DateTime? PhotoTaken { get; set; }
    public DateTime? FileCreated { get; set; }
    public string? Hash { get; set; }
    public double ? Longitude { get; set; }
    public double ? Latitude { get; set; }
    public string? Location { get; set; }
}