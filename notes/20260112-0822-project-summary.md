# METS Parser - Project Summary

This is a **Python METS XML parser** designed to extract simplified abstractions from METS (Metadata Encoding & Transmission Standard) files.

## What is METS?

METS is an XML standard developed by the Library of Congress for encoding metadata about digital objects. It's widely used in digital libraries and archives to describe:

- File structures and hierarchies
- Technical metadata (checksums, file sizes, MIME types)
- Descriptive metadata (titles, creators)
- Rights and access information

## What the Parser Does

The parser reads METS XML files and builds a working representation of the digital object structure:

1. **Parses administrative, technical, and descriptive metadata** from various METS sections (amdSec, techMD, dmdSec)
2. **Reconstructs the physical file/directory hierarchy** from the structMap element
3. **Extracts file details** including SHA256 checksums, sizes, and content types from PREMIS metadata

## Key Components

| File | Description |
|------|-------------|
| `mets_parser.py` | Core parsing logic with main entry points |
| `mets_wrapper.py` | `MetsWrapper` class holding parsed results |
| `model/working_filesystem.py` | `WorkingFile` and `WorkingDirectory` abstractions |
| `vocab.py` | XML namespace constants (METS, MODS, PREMIS, XLink) |
| `util.py` | Helper functions for path manipulation |

## Main Entry Points

- `get_mets_wrapper_from_file_like_object()` - Parse from file
- `get_mets_wrapper_from_string()` - Parse from XML string
- `build_mets_wrapper()` - Creates MetsWrapper from root element
- `populate_from_mets()` - Extracts metadata and structure

## Data Models

### MetsWrapper
Contains:
- `name`, `agent` - Descriptive metadata
- `physical_structure` - Root WorkingDirectory
- `files` - List of all WorkingFile objects
- `amd_map`, `file_map`, `tech_map` - Internal caches for quick lookups

### WorkingFile
Represents files with:
- `content_type` - MIME type
- `digest` - SHA256 checksum
- `size` - File size in bytes

### WorkingDirectory
Represents directories with methods to find files/directories by path

## Supported Formats

The fixtures show it handles METS files from various sources:

- **DLIP** - Digital Library Infrastructure Project format
- **EPrints** - EPrints repository format
- **Wellcome Collection Goobi** - Digitization workflow format
- **Wellcome Collection Archivematica** - Preservation format

## Dependencies

- `lxml~=5.3.1` - XML parsing library

## Notes

This is a companion to a .NET version of the parser.
