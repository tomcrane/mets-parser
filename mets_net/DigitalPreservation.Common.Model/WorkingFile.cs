namespace DigitalPreservation.Common.Model;

public class WorkingFile : WorkingBase
{
    public WorkingFile()
    {
        Type = "WorkingFile";
    }

    public string? ContentType { get; set; }
    public string? Digest { get; set; }
    public long? Size { get; set; }
}
