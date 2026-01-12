# .NET METS Parser Implementation Analysis

## MetsXDocumentParser Overview

The parser follows the same algorithm as the Python version:

1. **Entry point**: `GetMetsFileWrapperFromXDocument()` creates a `MetsFileWrapper` and calls `PopulateFromMets()`
2. **Metadata extraction**: Pulls title, agent, access conditions from MODS/metsHdr
3. **StructMap traversal**: Recursively processes `mets:div` elements to build directory/file tree
4. **File metadata**: Extracts digest, size, format, storage location from PREMIS sections

## Key Differences from Python

| Aspect | Python | .NET |
|--------|--------|------|
| Metadata storage | Pre-built dicts (`amd_map`, `file_map`, `tech_map`) | Direct LINQ lookups per element |
| Models | Simple classes | Rich typed classes with JSON attributes |
| Extracted data | Basic (content_type, digest, size) | Extended (virus scan, PRONOM format, storage location) |
| Namespaces | XPath strings | Type-safe `XNames` constants |

## New Classes Added

### Models
- `MetsFileWrapper` - Container for parsed METS with physical structure, files list, and metadata
- `MetsExtensions` - JSON-serializable METS-specific metadata (admId, divId)
- `StorageMetadata` - Original name and storage location
- `FileFormatMetadata` - PRONOM key, format name, digest, size
- `VirusScanMetadata` - ClamAV scan results with virus detection, definition version
- `DigestMetadata` - Hash verification with source tracking

### Utilities
- `XNames` - 87 static XNamespace/XName constants for all METS/MODS/PREMIS namespaces
- `StringUtils` - Extension methods: HasText(), GetSlug(), GetParent(), FormatFileSize()
- `UriX` - Extension methods: GetParentUri(), AppendEscapedSlug(), Escape/UnEscapeForUri()

### New Project
- `DigitalPreservation.Utils` - Shared utilities for strings and URIs

## Solution Structure

```
DigitalPreservation.sln
├── DigitalPreservation.Common.Model/
│   └── Models for METS parsing, working filesystem, metadata
├── Storage.Repository.Common/
│   └── MetsXDocumentParser, XNames, parsing logic
├── DigitalPreservation.Utils/
│   └── StringUtils, UriX helper methods
└── MetsParser.Tests/
    └── FixtureParsingTests with real METS files
```

## FixtureParsingTests Coverage

| Test | Fixture | Validates |
|------|---------|-----------|
| `Can_Parse_Goobi_METS_For_Wrapper` | wc-goobi/b29356350.xml | 2 dirs, 64 files (32 JP2 + 32 ALTO) |
| `Can_Parse_EPrints_METS` | eprints/w5b3cz4c.xml | Title extraction, SHA256 digest, single TIFF |
| `Can_Parse_Archivematica_METS` | wc-archivematica/METS...xml | 5 nested dirs, 38 files, deep path navigation |

## Observations

### Strengths
- Richer metadata extraction than Python (virus scans, PRONOM keys, storage locations)
- Type-safe XML handling via `XNames` class
- Good test coverage with real fixtures
- Clean separation across 4 projects
- Fluent use of LINQ for XML queries
- Extension methods improve code readability

### Areas for Improvement

#### Performance TODO
At `MetsXDocumentParser.cs:188`:
> "TODO - put these andSecs into a dictionary - have done in Python version"

The Python version pre-builds lookup dictionaries for O(1) access, while .NET currently does repeated LINQ queries. This could be optimized with `Dictionary<string, XElement>` caching.

#### Goobi ADMID Assumption
At lines 240-242: When multiple `fptr` elements share a single ADMID, the parser assumes all point to the same admin section. This is documented as a heuristic assumption.

#### Other Notes
- Label handling: Python converts to lowercase but .NET preserves case - could cause matching issues
- Some exceptions are generic (e.g., "Our folder logic is wrong")

## Dependencies

- `System.Xml.Linq` (built-in)
- `MimeTypes` NuGet package (for content type detection)
- `Microsoft.Extensions.Logging` (for structured logging)
- Test: `xUnit`, `FluentAssertions`
