# Functional Differences: Python vs .NET METS Parser

This document details functionality present in the .NET version that is missing from the Python version.

## 1. Metadata Extraction

| Feature | .NET | Python |
|---------|------|--------|
| **Access conditions** | Extracts `mods:accessCondition` with type "restriction on access" | Not extracted |
| **Rights statements** | Extracts `mods:accessCondition` with type "use and reproduction" as URI | Not extracted |
| **Storage location** | Extracts `premis:contentLocation` URI | Not extracted |
| **Original name** | Extracts `premis:originalName` and stores on files | Only used for directory path, not stored on files |
| **PRONOM format key** | Extracts `premis:formatRegistryKey` | Not extracted |
| **Format name** | Extracts `premis:formatName` | Not extracted |
| **Virus scan data** | Full extraction from `digiprovMD` (ClamAV events) | Not extracted |

## 2. Wrapper Properties

| Property | .NET `MetsFileWrapper` | Python `MetsWrapper` |
|----------|------------------------|----------------------|
| `RootUri` | Parent directory URI | Missing |
| `MetsUri` | Full URI to METS file | Missing |
| `Self` | WorkingFile for METS file itself | Missing |
| `XDocument` | Keeps parsed XML | Missing |
| `ETag` | For caching | Missing |
| `Editable` | Flag based on agent | Missing |
| `RootAccessConditions` | List of access conditions | Missing |
| `RootRightsStatement` | Rights URI | Missing |
| `LogicalStructures` | Logical structure ranges | Missing |

## 3. Model Classes

| .NET Class | Purpose | Python Equivalent |
|------------|---------|-------------------|
| `MetsExtensions` | Stores `admId`, `divId`, `href` per item | Missing |
| `StorageMetadata` | Original name + storage location | Missing |
| `FileFormatMetadata` | PRONOM key, format name, digest, size | Missing |
| `VirusScanMetadata` | Virus detection results | Missing |
| `DigestMetadata` | Hash with source tracking | Missing |
| `FileLink` | Links on files | Missing |

## 4. Behavioral Differences

### MIME Type Fallback
- **.NET**: Uses `MimeTypes.TryGetMimeType()` to deduce content type from file extension when MIMETYPE attribute is missing
- **Python**: Uses hardcoded `"dlip/not-identified"`

### Directory Metadata
- **.NET**: Attaches `MetsExtensions` and `StorageMetadata` to directories
- **Python**: Only sets path and name on directories

### Label Case Handling
- **Python**: Lowercases labels (`label = div.get("LABEL", "").lower()`)
- **.NET**: Preserves original case

This could cause matching issues when labels have mixed case.

### Metadata Aggregation
- **.NET**: `WorkingFile` has methods to merge metadata from multiple sources:
  - `GetFileFormatMetadata()` - combines format info from multiple tools
  - `GetDigestMetadata()` - validates digests match across sources
  - `GetVirusScanMetadata()` - retrieves virus scan results
- **Python**: No metadata aggregation capabilities

## 5. Lookup Maps

### .NET (after optimization)
```csharp
MetsLookupMaps(
    Dictionary<string, XElement> AmdSecMap,
    Dictionary<string, XElement> FileMap,
    Dictionary<string, XElement> TechMdMap,
    Dictionary<string, XElement> DigiprovMdMap  // For virus scan lookups
)
```

### Python
```python
mets_wrapper.amd_map = {}
mets_wrapper.file_map = {}
mets_wrapper.tech_map = {}
# No digiprov_map - virus scan data not extracted
```

## 6. Summary

The Python version is a minimal implementation focused on:
- Basic file/directory structure extraction
- SHA256 digest extraction
- File size extraction
- MIME type from METS attribute

The .NET version is production-ready with:
- Rich metadata model with JSON serialization
- Access control and rights management
- Virus scan tracking
- Format identification (PRONOM)
- Storage location tracking
- Metadata source tracking and aggregation
- Extensible metadata system

## Recommendations for Python Parity

To bring Python to feature parity with .NET:

1. Add metadata classes (`StorageMetadata`, `FileFormatMetadata`, `VirusScanMetadata`)
2. Extract access conditions and rights from MODS
3. Extract PRONOM format information from PREMIS
4. Add digiprovMD parsing for virus scan results
5. Add storage location extraction
6. Fix label case handling (preserve original case)
7. Add MIME type fallback using file extension
8. Add `MetsExtensions` equivalent for admId/divId tracking
