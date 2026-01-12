using System.Xml.Linq;

namespace MetsParser.Tests;

public class MetsFixtureTests
{
    private static readonly string FixturesPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "fixtures"));

    private static readonly XNamespace MetsNs = "http://www.loc.gov/METS/";

    public static IEnumerable<object[]> MetsFixtures =>
    [
        ["eprints/10315.METS.xml"],
        ["dlip/mets.xml"],
        ["wc-goobi/b29356350.xml"],
        ["wc-archivematica/METS.299eb16f-1e62-4bf6-b259-c82146153711.xml"]
    ];

    [Theory]
    [MemberData(nameof(MetsFixtures))]
    public void CanLoadFixtureAsXml(string relativePath)
    {
        var fullPath = Path.Combine(FixturesPath, relativePath);

        Assert.True(File.Exists(fullPath), $"Fixture file not found: {fullPath}");

        var doc = XDocument.Load(fullPath);

        Assert.NotNull(doc.Root);

        // Some fixtures have a wrapper element (mets-objects), others have mets as root
        var metsElement = GetMetsElement(doc);
        Assert.NotNull(metsElement);
        Assert.Equal("mets", metsElement.Name.LocalName);
    }

    [Theory]
    [MemberData(nameof(MetsFixtures))]
    public void FixtureHasMetsNamespace(string relativePath)
    {
        var fullPath = Path.Combine(FixturesPath, relativePath);
        var doc = XDocument.Load(fullPath);

        var metsElement = GetMetsElement(doc);
        Assert.NotNull(metsElement);
        Assert.Equal(MetsNs, metsElement.Name.Namespace);
    }

    [Theory]
    [MemberData(nameof(MetsFixtures))]
    public void FixtureHasMetsHeader(string relativePath)
    {
        var fullPath = Path.Combine(FixturesPath, relativePath);
        var doc = XDocument.Load(fullPath);

        var metsElement = GetMetsElement(doc);
        Assert.NotNull(metsElement);

        var metsHdr = metsElement.Element(MetsNs + "metsHdr");
        Assert.NotNull(metsHdr);
    }

    [Theory]
    [MemberData(nameof(MetsFixtures))]
    public void FixtureHasDescriptiveMetadata(string relativePath)
    {
        var fullPath = Path.Combine(FixturesPath, relativePath);
        var doc = XDocument.Load(fullPath);

        var metsElement = GetMetsElement(doc);
        Assert.NotNull(metsElement);

        var dmdSec = metsElement.Element(MetsNs + "dmdSec");
        Assert.NotNull(dmdSec);
    }

    /// <summary>
    /// Gets the mets element, handling both direct root and wrapper element patterns.
    /// Some METS files have mets:mets as root, others have a wrapper like mets-objects.
    /// </summary>
    private static XElement? GetMetsElement(XDocument doc)
    {
        if (doc.Root == null)
        {
            return null;
        }

        // If root is already mets element
        if (doc.Root.Name.LocalName == "mets" && doc.Root.Name.Namespace == MetsNs)
        {
            return doc.Root;
        }

        // Look for mets element as child (wrapper pattern like mets-objects)
        return doc.Root.Element(MetsNs + "mets");
    }
}
