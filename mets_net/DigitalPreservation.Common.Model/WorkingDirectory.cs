namespace DigitalPreservation.Common.Model;

public class WorkingDirectory : WorkingBase
{
    public WorkingDirectory()
    {
        Type = "WorkingDirectory";
    }

    public List<WorkingFile> Files { get; set; } = [];
    public List<WorkingDirectory> Directories { get; set; } = [];

    public WorkingFile? FindFile(string path)
    {
        var parent = FindDirectory(GetParent(path));
        if (parent == null)
        {
            return null;
        }
        var slug = GetSlugFromPath(path);
        return parent.Files.FirstOrDefault(f => f.GetSlug() == slug);
    }

    public WorkingDirectory? FindDirectory(string? path, bool create = false)
    {
        if (string.IsNullOrWhiteSpace(path) || path == "/")
        {
            return this;
        }

        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
        var directory = this;

        for (var index = 0; index < parts.Count; index++)
        {
            var part = parts[index];
            var potentialDirectory = directory.Directories.FirstOrDefault(d => d.GetSlug() == part);

            if (create)
            {
                if (potentialDirectory == null)
                {
                    potentialDirectory = new WorkingDirectory
                    {
                        LocalPath = string.Join('/', parts.Take(index + 1))
                    };
                    directory.Directories.Add(potentialDirectory);
                }
            }
            else
            {
                if (potentialDirectory == null)
                {
                    return null;
                }
            }

            directory = potentialDirectory;
        }

        return directory;
    }

    private static string? GetParent(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        var lastSlash = path.LastIndexOf('/');
        if (lastSlash <= 0)
        {
            return null;
        }
        return path[..lastSlash];
    }

    private static string GetSlugFromPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }
        var parts = path.Split('/');
        return parts[^1];
    }
}
