# FileSplitter

A powerful C# application for splitting large files based on specific keywords. Designed specifically for CNC G-code files but can work with any text-based files.

## Features

- **Dual Interface**: Both GUI (Windows Forms) and Console versions
- **Efficient Processing**: Handles large files with memory-efficient streaming
- **Keyword-Based Splitting**: Split files based on any keyword (default: M30)
- **Smart Naming**: Output files automatically named from program numbers (e.g., O00109181.nc)
- **Progress Tracking**: Real-time progress updates and detailed logging
- **User-Friendly**: Intuitive interface with input validation
- **Flexible Output**: Option to add end-of-line characters after keywords

## Use Cases

- **CNC Programming**: Split combined G-code files into individual programs
- **Data Processing**: Split large data files based on delimiters
- **Log Analysis**: Split log files based on timestamps or events
- **General Text Processing**: Any text file that needs splitting based on keywords

## Installation

### Prerequisites
- .NET 6.0 or later
- Windows OS (for GUI version)
- Visual Studio 2022 or later (for development)

### Build from Source
1. Clone or download the source code
2. Open in Visual Studio or use .NET CLI:
   ```bash
   dotnet build
   ```

### Run the Application
```bash
dotnet run
```

Or compile and run the executable:
```bash
dotnet build -c Release
./bin/Release/net6.0-windows/FileSplitter.exe
```

## Usage

### Launch Options
- **Default**: Shows version selector menu
- **GUI Mode**: `FileSplitter.exe gui`
- **Console Mode**: `FileSplitter.exe console`

### GUI Version
1. **Select Input File**: Click "Browse" to choose your file
2. **Set Keyword**: Enter the keyword to split on (default: M30)
3. **End of Line Option**: Check if you want to add a line break after the keyword
4. **Select Output Folder**: Choose where to save the split files
5. **Click Process**: Start the file splitting process

### Console Version
Follow the interactive prompts:
1. Enter the input file path
2. Enter the keyword (or press Enter for default: M30)
3. Choose end-of-line option (y/n)
4. Enter the output folder path

## File Format Support

The program is designed to work with CNC G-code files but supports any text file format:

### CNC G-code Files
- Automatically detects program numbers (O followed by digits)
- Splits on M30 (end of program) commands by default
- Names output files based on program numbers (e.g., O00109181.nc)

### General Text Files
- Split based on any keyword
- Sequential numbering for output files if no program numbers found

## Example

### Input File Structure
```
O00109181(CQMILL_DOOSAN_NHP4000 )
(MACHINE GROUP-1)
N10G20 
N20G00G17G20G40G80G90
...
N8530M30

O00109182(CQMILL_DOOSAN_NHP4000 )
(MACHINE GROUP-2)
N10G20 
...
N11420M30
```

### Output
- `O00109181.nc` - Contains first program
- `O00109182.nc` - Contains second program
- Additional files for each program found

## Performance

- **Memory Efficient**: Processes files line by line
- **Async Processing**: Non-blocking file operations
- **Progress Tracking**: Real-time updates during processing
- **Error Handling**: Comprehensive error handling and logging

## Configuration

### Default Settings
- **Keyword**: M30
- **Output Extension**: .nc
- **Encoding**: UTF-8
- **End of Line**: Optional

### Customization
- Modify `DefaultKeyword` in source code
- Change `ProgramNumberPattern` regex for different naming schemes
- Adjust file extensions and encoding as needed

## Error Handling

The application includes comprehensive error handling:
- File not found errors
- Invalid path errors
- Permission errors
- Memory issues with large files
- Malformed program numbers

## Logging

### GUI Version
- Real-time log display in the application
- Timestamped entries
- Color-coded console-style output

### Console Version
- Console output with progress updates
- Line count and program count tracking
- Summary information

## Technical Details

### Architecture
- **Streaming**: Uses StreamReader for memory efficiency
- **Async/Await**: Asynchronous file operations
- **Regex**: Pattern matching for program numbers
- **StringBuilder**: Efficient string concatenation

### Dependencies
- .NET 6.0 Framework
- System.Windows.Forms (GUI version)
- System.Text.RegularExpressions
- System.IO

## Troubleshooting

### Common Issues
1. **File not found**: Check file path and permissions
2. **Output folder access**: Ensure write permissions
3. **Large file processing**: Monitor memory usage
4. **No programs found**: Verify file format and program number pattern

### Performance Tips
- Use SSD storage for better I/O performance
- Ensure adequate free disk space
- Close other applications when processing very large files

## Support

For issues or questions:
1. Check the log output for detailed error messages
2. Verify file format matches expected structure
3. Ensure sufficient disk space and permissions

## License

This project is provided as-is for educational and commercial use. 