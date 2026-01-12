namespace DigitalPreservation.Common.Model;

public abstract class WorkingBase
{
    public string? Type { get; protected set; }
    public string? LocalPath { get; set; }
    public string? Name { get; set; }
    public DateTime? Modified { get; set; }
    public string? AccessCondition { get; set; }
    public string? Rights { get; set; }

    public string GetSlug()
    {
        if (string.IsNullOrEmpty(LocalPath))
        {
            return string.Empty;
        }
        var parts = LocalPath.Split('/');
        return parts[^1];
    }
}
