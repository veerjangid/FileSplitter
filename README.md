# FileSplitter - CNC File Splitting Tool

## Overview

FileSplitter is a C# Windows Forms application (.NET 6.0) designed to split large files based on specific keywords. It was specifically created for splitting CNC program files but can be used for any text-based files that need to be divided based on keyword markers.

## Features

- **User-friendly Windows Forms GUI** with file browsers and progress tracking
- **Console version** for automation and batch processing
- **Configurable split keywords** via dropdown menu loaded from config.json
- **Efficient processing** of large files using streaming methods
- **CNC program detection** with automatic file naming (O-numbers)
- **Progress tracking** and detailed logging
- **Customizable output** with optional end-of-line insertion

## Requirements

- .NET 6.0 or higher
- Windows OS (for Windows Forms GUI)
- Visual Studio 2022 or Visual Studio Enterprise (for development)

## Installation

1. Clone or download the project
2. Open in Visual Studio Enterprise
3. Build the solution (`Ctrl+Shift+B`)
4. Run the application

## Usage

### GUI Version (Recommended)

1. **Launch the application**:
   ```bash
   dotnet run gui
   ```
   Or use the batch file: `run_gui.bat`

2. **Select input file**: Click "Browse..." to select your file
3. **Choose split keyword**: Select from the dropdown menu (loaded from config.json)
4. **Select output folder**: Choose where to save the split files
5. **Optional**: Check "Add end of line after keyword" if needed
6. **Click "Process File"** to start splitting

### Console Version

```bash
dotnet run console
```
Or use the batch file: `run_console.bat`

### Version Selector

```bash
dotnet run
```

## Configuration

The application uses a `config.json` file to configure various settings:

### Split Keywords Configuration

The dropdown menu in the GUI loads its options from the `AvailableKeywords` array in the config file:

```json
{
  "FileSplitter": {
    "DefaultSettings": {
      "SplitKeyword": "M30",
      "AvailableKeywords": [
        "M30",
        "M02",
        "M01",
        "M00",
        "END",
        "ENDSUB",
        "%",
        "G28",
        "REWIND",
        "STOP"
      ]
    }
  }
}
```

### Adding Custom Keywords

To add your own keywords to the dropdown:

1. Open `config.json`
2. Add new keywords to the `AvailableKeywords` array
3. Save the file
4. Restart the application

### Other Configuration Options

- **OutputFileExtension**: Default `.nc` for CNC files
- **ProgramNumberPattern**: Regex pattern for detecting program numbers
- **BufferSize**: File reading buffer size for performance
- **LogMaxLines**: Maximum lines to display in the log

## Sample Usage

### CNC Program Files

Perfect for splitting large CNC program files:

**Input**: `sample_ALL-PROG.TXT` (1.4 MB, 81,822 lines, 67 programs)
**Split Keyword**: `M30` (selected from dropdown)
**Output**: 67 individual `.nc` files named `O00109181.nc`, `O00109182.nc`, etc.

### Performance

- **Large files**: Efficiently handles files of any size using streaming
- **Memory efficient**: Processes files without loading entire content into memory
- **Fast processing**: Optimized for quick splitting with progress tracking

## File Structure

```
FileSplitter/
├── FileSplitter.sln           # Visual Studio solution
├── FileSplitter.csproj        # Project file
├── Program.cs                 # Main entry point with version selector
├── FileSplitter.cs            # Console version logic
├── FileSplitterGUI.cs         # Windows Forms GUI
├── config.json               # Configuration file
├── README.md                 # Documentation
├── run_gui.bat               # GUI launcher
├── run_console.bat           # Console launcher
└── sample_ALL-PROG.TXT       # Sample test file
```

## Development

### Building

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Publishing

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

## Technical Details

### Architecture

- **Main Application**: `Program.cs` - Entry point with version selector
- **Console Logic**: `FileSplitter.cs` - Command-line interface and core logic
- **GUI Logic**: `FileSplitterGUI.cs` - Windows Forms interface
- **Configuration**: `config.json` - Configurable settings including dropdown options

### Key Features

1. **Configurable Keywords Dropdown**: 
   - Loads available keywords from `config.json`
   - Easy to add/remove keywords without code changes
   - Dropdown prevents typos and provides standard options

2. **Streaming File Processing**:
   - Reads files line by line to handle large files efficiently
   - Memory usage remains constant regardless of file size
   - Progress tracking for user feedback

3. **CNC Program Detection**:
   - Uses regex patterns to identify program numbers (O-numbers)
   - Automatically names output files based on detected program numbers
   - Handles standard CNC file formats

4. **Error Handling**:
   - Comprehensive validation of inputs
   - Graceful handling of file access errors
   - User-friendly error messages

## Common Keywords

The default configuration includes common CNC and text processing keywords:

- **M30**: Program end (CNC)
- **M02**: Program end (CNC)
- **M01**: Optional stop (CNC)
- **M00**: Program stop (CNC)
- **END**: Generic end marker
- **ENDSUB**: Subroutine end (CNC)
- **%**: Program delimiter (CNC)
- **G28**: Reference position return (CNC)
- **REWIND**: Tape rewind (CNC)
- **STOP**: Generic stop command

## Troubleshooting

### GUI Not Showing Keywords

1. Check that `config.json` exists in the application directory
2. Verify JSON syntax is correct
3. Ensure `AvailableKeywords` array is properly formatted
4. Check application logs for config loading errors

### File Processing Issues

1. Ensure input file exists and is readable
2. Check output folder permissions
3. Verify selected keyword exists in the file
4. Review processing logs for detailed error information

## License

This project is open source and available under the MIT License.

## Support

For issues or questions:
1. Check the processing logs for detailed error information
2. Verify your `config.json` file is properly formatted
3. Ensure all file permissions are correct
4. Review the troubleshooting section above

---

**Version**: 1.0.0  
**Last Updated**: January 6, 2025  
**Compatibility**: .NET 6.0, Windows Forms, Visual Studio Enterprise 