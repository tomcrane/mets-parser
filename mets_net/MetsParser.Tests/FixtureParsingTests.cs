using System.Xml.Linq;
using DigitalPreservation.Common.Model.Mets;
using DigitalPreservation.Common.Model.Transit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Storage.Repository.Common.Mets;

namespace MetsParser.Tests;

public class FixtureParsingTests
{
    private ILogger<MetsXDocumentParser> logger;
    
    private static readonly string FixturesPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "fixtures"));
    
    public FixtureParsingTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();
        logger = factory!.CreateLogger<MetsXDocumentParser>();    
    }

    private MetsFileWrapper GetMetsFileWrapper(string fixture)
    {
        var fullPath = Path.Combine(FixturesPath, fixture);
        var parser = new MetsXDocumentParser(logger);
        var xDoc = XDocument.Load(fullPath);
        var wrapper = parser.GetMetsFileWrapperFromXDocument(new Uri(fullPath), xDoc);
        return wrapper;
    }
    
    [Fact]
    public void Can_Parse_Goobi_METS_For_Wrapper()
    {
        var wrapper = GetMetsFileWrapper("wc-goobi/b29356350.xml");
        var phys = wrapper.PhysicalStructure;
        
        // phys!.Files.Should().Contain(f => f.Name == "goobi-wc-b29356350-2.xml");
        phys.Directories.Should().HaveCount(2);
        var objDir = phys.Directories.Single(d => d.Name == FolderNames.Objects);
        objDir.Directories.Should().HaveCount(0);
        objDir.Files.Should().HaveCount(32);
        objDir.LocalPath.Should().Be(FolderNames.Objects);
        var altoDir = phys.Directories.Single(d => d.Name == "alto");
        altoDir.Directories.Should().HaveCount(0);
        altoDir.Files.Should().HaveCount(32);
        altoDir.LocalPath.Should().Be("alto");
        
        objDir.Files[10].LocalPath.Should().Be("objects/b29356350_0011.jp2");
        objDir.Files[10].Name.Should().Be("b29356350_0011.jp2");
        objDir.Files[10].ContentType.Should().Be("image/jp2");
        
        altoDir.Files[10].LocalPath.Should().Be("alto/b29356350_0011.xml");
        altoDir.Files[10].Name.Should().Be("b29356350_0011.xml");
        altoDir.Files[10].ContentType.Should().Be("application/xml");
    }


    [Fact]
    public void Can_Parse_EPrints_METS()
    {
        var wrapper = GetMetsFileWrapper("eprints/w5b3cz4c.xml");
        var phys = wrapper.PhysicalStructure;
        
        // phys!.Files.Should().Contain(f => f.Name == "EPrints.10315.METS.xml");

        wrapper.Name.Should().Be("Marie Hartley and Joan Ingilby. Marie standing by (?painting) the big picture of Askrigg (December 1984)");
        phys.Directories.Should().HaveCount(1);
        phys.Directories[0].Files.Should().HaveCount(1);
        
        phys.Directories[0].Name.Should().Be(FolderNames.Objects);
        phys.Directories[0].Files[0].Digest.Should().Be("37d42893961434c50d310b28e37d468c4bf8ea9451b46665629a206d55464121");

        phys.Directories[0].Files[0].LocalPath.Should().Be("objects/LEEUA_1999.015.731_01.tif");
        phys.Directories[0].Files[0].Name.Should().Be("LEEUA_1999.015.731_01.tif");
    }

    [Fact]
    public void Can_Parse_Archivematica_METS()
    {
        var wrapper = GetMetsFileWrapper("wc-archivematica/METS.299eb16f-1e62-4bf6-b259-c82146153711.xml");
        var phys = wrapper.PhysicalStructure;
        
        // Only possible with self
        // phys!.Files.Should().Contain(f => f.Name == "archivematica-wc-METS.299eb16f-1e62-4bf6-b259-c82146153711.xml");

        wrapper.Name.Should().BeNull(); // No name in Archivematica METS
        phys.Directories.Should().HaveCount(1);
        
        wrapper.Files.Count.Should().Be(38); // 39 with self
        wrapper.Files.Should().Contain(f => f.LocalPath == "objects/Edgware_Community_Hospital/03_05_01.tif");
        wrapper.Files.Should().Contain(f => f.LocalPath == "objects/Edgware_Community_Hospital/presentation_site_plan_A3.pdf");
        wrapper.Files.Should().Contain(f => f.LocalPath == "objects/metadata/transfers/ARTCOOB9-4840a241-d397-4554-abfe-69f1ad674126/rights.csv");
            
        var objDir = phys.Directories[0]; 
        objDir.Files.Should().HaveCount(0);
        objDir.Directories.Should().HaveCount(5);
        objDir.Files.Should().HaveCount(0); // no direct children
        objDir.LocalPath.Should().Be(FolderNames.Objects);
        objDir.Name.Should().Be(FolderNames.Objects);
        objDir.Directories.Should().Contain(d => d.LocalPath == "objects/Edgware_Community_Hospital");
        objDir.Directories.Should().Contain(d => d.LocalPath == "objects/West_Middlesex");
        objDir.Directories.Should().Contain(d => d.LocalPath == "objects/GJW_King_s_College_Hospital");
        objDir.Directories.Should().Contain(d => d.LocalPath == "objects/submissionDocumentation");
        objDir.Directories.Should().Contain(d => d.LocalPath == "objects/metadata");
        var kings = objDir.FindDirectory("GJW_King_s_College_Hospital", false);
        kings.Should().NotBeNull();
        kings!.Name.Should().Be("GJW_King_s_College_Hospital"); // unaltered
        kings.Directories.Should().HaveCount(0);
        kings.Files.Should().HaveCount(13);
        var plan = kings.FindFile("Kings_1913_plan_altered.jpg");
        plan.Should().NotBeNull();
        plan!.Name.Should().Be("Kings 1913 plan altered.jpg"); // note with spaces from LABEL
        var edgware = objDir.FindDirectory("Edgware_Community_Hospital");
        edgware.Should().NotBeNull();
        edgware!.Name.Should().Be("Edgware Community Hospital"); // with spaces
        edgware.Directories.Should().HaveCount(0);
        edgware.Files.Should().HaveCount(11);
    }
}