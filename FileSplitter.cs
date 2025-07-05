using System.Text;
using System.Text.RegularExpressions;

namespace FileSplitter
{
    public class FileSplitterProgram
    {
        private static readonly string DefaultKeyword = "M30";
        private static readonly string ProgramNumberPattern = @"^O(\d+)";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== File Splitter Program ===");
            Console.WriteLine("Split files based on specific keywords");
            Console.WriteLine();

            try
            {
                // Get input file path
                string inputFile = GetInputFilePath();

                // Get keyword for splitting
                string keyword = GetSplitKeyword();

                // Get option for end-of-line handling
                bool addEndOfLine = GetEndOfLineOption();

                // Get output folder
                string outputFolder = GetOutputFolder();

                // Process the file
                await ProcessFileAsync(inputFile, keyword, outputFolder, addEndOfLine);

                Console.WriteLine("\nFile splitting completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static string GetInputFilePath()
        {
            string inputFile;
            do
            {
                Console.Write("Enter the input file path: ");
                inputFile = Console.ReadLine()?.Trim().Trim('"');

                if (string.IsNullOrEmpty(inputFile))
                {
                    Console.WriteLine("Please enter a valid file path.");
                    continue;
                }

                if (!File.Exists(inputFile))
                {
                    Console.WriteLine("File does not exist. Please check the path and try again.");
                    continue;
                }

                break;
            } while (true);

            return inputFile;
        }

        private static string GetSplitKeyword()
        {
            Console.Write($"Enter the keyword to split on (default: {DefaultKeyword}): ");
            string keyword = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                keyword = DefaultKeyword;
            }

            Console.WriteLine($"Using keyword: {keyword}");
            return keyword;
        }

        private static bool GetEndOfLineOption()
        {
            Console.Write("Add end of line after keyword? (y/n, default: n): ");
            string response = Console.ReadLine()?.Trim().ToLower();

            bool addEndOfLine = response == "y" || response == "yes";
            Console.WriteLine($"Add end of line: {(addEndOfLine ? "Yes" : "No")}");

            return addEndOfLine;
        }

        private static string GetOutputFolder()
        {
            string outputFolder;
            do
            {
                Console.Write("Enter the output folder path: ");
                outputFolder = Console.ReadLine()?.Trim().Trim('"');

                if (string.IsNullOrEmpty(outputFolder))
                {
                    Console.WriteLine("Please enter a valid folder path.");
                    continue;
                }

                try
                {
                    if (!Directory.Exists(outputFolder))
                    {
                        Directory.CreateDirectory(outputFolder);
                        Console.WriteLine($"Created output folder: {outputFolder}");
                    }
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating folder: {ex.Message}");
                }
            } while (true);

            return outputFolder;
        }

        private static async Task ProcessFileAsync(string inputFile, string keyword, string outputFolder, bool addEndOfLine)
        {
            Console.WriteLine("\nProcessing file...");

            var fileInfo = new FileInfo(inputFile);
            Console.WriteLine($"Input file size: {fileInfo.Length:N0} bytes");

            var programs = new List<ProgramData>();
            var currentProgram = new StringBuilder();
            string currentProgramNumber = null;
            int lineCount = 0;
            int programCount = 0;

            // Read file line by line for memory efficiency
            using (var reader = new StreamReader(inputFile, Encoding.UTF8))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineCount++;

                    // Check if this line contains a program number
                    var match = Regex.Match(line, ProgramNumberPattern);
                    if (match.Success)
                    {
                        // Save previous program if exists
                        if (currentProgram.Length > 0 && !string.IsNullOrEmpty(currentProgramNumber))
                        {
                            programs.Add(new ProgramData
                            {
                                ProgramNumber = currentProgramNumber,
                                Content = currentProgram.ToString()
                            });
                            programCount++;
                        }

                        // Start new program
                        currentProgramNumber = match.Groups[1].Value;
                        currentProgram.Clear();
                        currentProgram.AppendLine(line);

                        Console.WriteLine($"Found program: O{currentProgramNumber}");
                    }
                    else
                    {
                        // Add line to current program
                        currentProgram.AppendLine(line);

                        // Check if this line contains the split keyword
                        if (line.Contains(keyword))
                        {
                            if (addEndOfLine)
                            {
                                currentProgram.AppendLine();
                            }

                            // Save current program
                            if (!string.IsNullOrEmpty(currentProgramNumber))
                            {
                                programs.Add(new ProgramData
                                {
                                    ProgramNumber = currentProgramNumber,
                                    Content = currentProgram.ToString()
                                });
                                programCount++;
                            }

                            // Reset for next program
                            currentProgram.Clear();
                            currentProgramNumber = null;
                        }
                    }

                    if (lineCount % 10000 == 0)
                    {
                        Console.WriteLine($"Processed {lineCount:N0} lines, found {programCount} programs...");
                    }
                }

                // Save the last program if it exists
                if (currentProgram.Length > 0 && !string.IsNullOrEmpty(currentProgramNumber))
                {
                    programs.Add(new ProgramData
                    {
                        ProgramNumber = currentProgramNumber,
                        Content = currentProgram.ToString()
                    });
                    programCount++;
                }
            }

            Console.WriteLine($"Total lines processed: {lineCount:N0}");
            Console.WriteLine($"Total programs found: {programs.Count}");

            // Write output files
            await WriteOutputFilesAsync(programs, outputFolder);
        }

        private static async Task WriteOutputFilesAsync(List<ProgramData> programs, string outputFolder)
        {
            Console.WriteLine("\nWriting output files...");

            int filesWritten = 0;
            var tasks = new List<Task>();

            foreach (var program in programs)
            {
                var task = WriteIndividualFileAsync(program, outputFolder);
                tasks.Add(task);

                // Limit concurrent file operations to avoid overwhelming the system
                if (tasks.Count >= 10)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(t => t.IsCompleted);
                }
            }

            // Wait for all remaining tasks
            await Task.WhenAll(tasks);

            Console.WriteLine($"Successfully wrote {programs.Count} files to: {outputFolder}");

            // Display summary
            Console.WriteLine("\n=== Summary ===");
            Console.WriteLine($"Programs processed: {programs.Count}");
            Console.WriteLine($"Output folder: {outputFolder}");
            Console.WriteLine("\nFile list:");

            foreach (var program in programs.Take(10))
            {
                var fileName = $"O{program.ProgramNumber}.nc";
                Console.WriteLine($"  - {fileName}");
            }

            if (programs.Count > 10)
            {
                Console.WriteLine($"  ... and {programs.Count - 10} more files");
            }
        }

        private static async Task WriteIndividualFileAsync(ProgramData program, string outputFolder)
        {
            try
            {
                var fileName = $"O{program.ProgramNumber}.nc";
                var filePath = Path.Combine(outputFolder, fileName);

                await File.WriteAllTextAsync(filePath, program.Content, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing file for program O{program.ProgramNumber}: {ex.Message}");
            }
        }
    }

    public class ProgramData
    {
        public string ProgramNumber { get; set; }
        public string Content { get; set; }
    }
}