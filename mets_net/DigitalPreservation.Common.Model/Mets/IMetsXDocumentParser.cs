using System.Xml.Linq;

namespace DigitalPreservation.Common.Model.Mets;

public interface IMetsXDocumentParser
{
    MetsFileWrapper GetMetsFileWrapperFromXDocument(Uri metsUri, XDocument metsXDocument);
}