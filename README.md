# CopyPaste

A Windows application that compresses files/folders and transfers them via clipboard using Base64 encoding.

## Features

### Compression (Drag & Drop)

Drag a file or folder onto the executable to:

- Compress the content in ZIP format with LZMA compression
- Convert to Base64
- Automatically copy to clipboard

**Usage:**

```batch
CopyPaste.exe <file_or_folder_path>
```

or simply drag and drop the file/folder onto the executable.

### Restore (Double Click)

Run the application with a double click when the clipboard contains Base64 data to:

- Decode the content
- Extract the ZIP files
- Save to the executable's folder

## Logging

The application generates a `log.txt` file in the same folder as the executable with operation details.

## Usage

1. **Compress**: Drag file/folder onto `CopyPaste.exe`
2. **Restore**: Double click `CopyPaste.exe` to extract from clipboard

## Dependencies

- **.NET 8.0** (Windows)
- **SharpCompress** - for compression/decompression

```batch
dotnet add package SharpCompress
```

## Compile this application using .NET 8.0 and SharpCompress library for compression.

```batch
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=false -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish
```

## Notes

- Base64 files in the clipboard can be easily transferred between computers
- Compression uses LZMA for maximum compression ratio
- All file paths are preserved during extraction
