using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace FileSplitter
{
    public partial class FileSplitterForm : Form
    {
        private static readonly string DefaultKeyword = "M30";
        private static readonly string ProgramNumberPattern = @"^O(\d+)";

        private TextBox txtInputFile;
        private ComboBox cboKeyword;
        private TextBox txtOutputFolder;
        private CheckBox chkAddEndOfLine;
        private Button btnBrowseInput;
        private Button btnBrowseOutput;
        private Button btnProcess;
        private ProgressBar progressBar;
        private TextBox txtLog;
        private Label lblStatus;

        public FileSplitterForm()
        {
            InitializeComponent();
            LoadKeywordsFromConfig();
        }

        private void LoadKeywordsFromConfig()
        {
            try
            {
                var configPath = Path.Combine(Application.StartupPath, "config.json");
                if (File.Exists(configPath))
                {
                    var jsonString = File.ReadAllText(configPath);
                    var config = JsonDocument.Parse(jsonString);
                    
                    if (config.RootElement.TryGetProperty("FileSplitter", out var fileSplitterSection) &&
                        fileSplitterSection.TryGetProperty("DefaultSettings", out var defaultSettings) &&
                        defaultSettings.TryGetProperty("AvailableKeywords", out var keywords))
                    {
                        cboKeyword.Items.Clear();
                        foreach (var keyword in keywords.EnumerateArray())
                        {
                            cboKeyword.Items.Add(keyword.GetString());
                        }
                        
                        // Set default keyword
                        if (defaultSettings.TryGetProperty("SplitKeyword", out var defaultKeyword))
                        {
                            cboKeyword.Text = defaultKeyword.GetString();
                        }
                        else if (cboKeyword.Items.Count > 0)
                        {
                            cboKeyword.SelectedIndex = 0;
                        }
                    }
                }
                else
                {
                    // Fallback keywords if config file doesn't exist
                    cboKeyword.Items.AddRange(new string[] { "M30", "M02", "M01", "M00", "END", "ENDSUB", "%", "G28", "REWIND", "STOP" });
                    cboKeyword.Text = DefaultKeyword;
                }
            }
            catch (Exception ex)
            {
                // Fallback to default keywords on error
                cboKeyword.Items.AddRange(new string[] { "M30", "M02", "M01", "M00", "END", "ENDSUB", "%", "G28", "REWIND", "STOP" });
                cboKeyword.Text = DefaultKeyword;
                LogMessage($"Warning: Could not load keywords from config: {ex.Message}");
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "File Splitter - Split files based on keywords";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(600, 400);

            // Input file section
            var lblInputFile = new Label
            {
                Text = "Input File:",
                Location = new Point(20, 20),
                Size = new Size(100, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtInputFile = new TextBox
            {
                Location = new Point(130, 20),
                Size = new Size(500, 23),
                ReadOnly = true,
                BackColor = Color.White
            };

            btnBrowseInput = new Button
            {
                Text = "Browse...",
                Location = new Point(640, 19),
                Size = new Size(100, 25)
            };
            btnBrowseInput.Click += BtnBrowseInput_Click;

            // Keyword section - Changed from TextBox to ComboBox
            var lblKeyword = new Label
            {
                Text = "Split Keyword:",
                Location = new Point(20, 60),
                Size = new Size(100, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cboKeyword = new ComboBox
            {
                Location = new Point(130, 60),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // End of line option
            chkAddEndOfLine = new CheckBox
            {
                Text = "Add end of line after keyword",
                Location = new Point(300, 60),
                Size = new Size(200, 23),
                CheckAlign = ContentAlignment.MiddleLeft
            };

            // Output folder section
            var lblOutputFolder = new Label
            {
                Text = "Output Folder:",
                Location = new Point(20, 100),
                Size = new Size(100, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtOutputFolder = new TextBox
            {
                Location = new Point(130, 100),
                Size = new Size(500, 23),
                ReadOnly = true,
                BackColor = Color.White
            };

            btnBrowseOutput = new Button
            {
                Text = "Browse...",
                Location = new Point(640, 99),
                Size = new Size(100, 25)
            };
            btnBrowseOutput.Click += BtnBrowseOutput_Click;

            // Process button
            btnProcess = new Button
            {
                Text = "Process File",
                Location = new Point(20, 140),
                Size = new Size(120, 35),
                BackColor = Color.LightGreen,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold)
            };
            btnProcess.Click += BtnProcess_Click;

            // Progress bar
            progressBar = new ProgressBar
            {
                Location = new Point(150, 150),
                Size = new Size(400, 15),
                Visible = false
            };

            // Status label
            lblStatus = new Label
            {
                Text = "Ready",
                Location = new Point(560, 150),
                Size = new Size(180, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Log text box
            var lblLog = new Label
            {
                Text = "Processing Log:",
                Location = new Point(20, 190),
                Size = new Size(100, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtLog = new TextBox
            {
                Location = new Point(20, 220),
                Size = new Size(720, 320),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 8F)
            };

            // Add all controls to form
            this.Controls.AddRange(new Control[] {
                lblInputFile, txtInputFile, btnBrowseInput,
                lblKeyword, cboKeyword, chkAddEndOfLine,
                lblOutputFolder, txtOutputFolder, btnBrowseOutput,
                btnProcess, progressBar, lblStatus,
                lblLog, txtLog
            });

            // Handle form resize
            this.Resize += FileSplitterForm_Resize;

            this.ResumeLayout(false);
        }

        private void FileSplitterForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) return;

            var width = this.ClientSize.Width;
            var height = this.ClientSize.Height;

            // Adjust input file controls
            txtInputFile.Width = width - 260;
            btnBrowseInput.Left = width - 120;

            // Adjust output folder controls
            txtOutputFolder.Width = width - 260;
            btnBrowseOutput.Left = width - 120;

            // Adjust progress bar and status
            progressBar.Width = width - 320;
            lblStatus.Left = width - 180;

            // Adjust log text box
            txtLog.Width = width - 60;
            txtLog.Height = height - 260;
        }

        private void BtnBrowseInput_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select input file to split";
                dialog.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|NC files (*.nc)|*.nc";
                dialog.FilterIndex = 1;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtInputFile.Text = dialog.FileName;
                    LogMessage($"Selected input file: {dialog.FileName}");

                    // Auto-suggest output folder
                    if (string.IsNullOrEmpty(txtOutputFolder.Text))
                    {
                        var suggestedFolder = Path.Combine(Path.GetDirectoryName(dialog.FileName), "Split_Output");
                        txtOutputFolder.Text = suggestedFolder;
                    }
                }
            }
        }

        private void BtnBrowseOutput_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select output folder for split files";
                dialog.ShowNewFolderButton = true;

                if (!string.IsNullOrEmpty(txtOutputFolder.Text))
                {
                    dialog.SelectedPath = txtOutputFolder.Text;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtOutputFolder.Text = dialog.SelectedPath;
                    LogMessage($"Selected output folder: {dialog.SelectedPath}");
                }
            }
        }

        private async void BtnProcess_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                btnProcess.Enabled = false;
                progressBar.Visible = true;
                lblStatus.Text = "Processing...";
                txtLog.Clear();

                await ProcessFileAsync(txtInputFile.Text, cboKeyword.Text.Trim(),
                                     txtOutputFolder.Text, chkAddEndOfLine.Checked);

                lblStatus.Text = "Completed";
                MessageBox.Show("File splitting completed successfully!", "Success",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error";
                LogMessage($"Error: {ex.Message}");
                MessageBox.Show($"Error occurred: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnProcess.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtInputFile.Text))
            {
                MessageBox.Show("Please select an input file.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!File.Exists(txtInputFile.Text))
            {
                MessageBox.Show("Input file does not exist.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(cboKeyword.Text?.Trim()))
            {
                MessageBox.Show("Please enter a keyword to split on.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(txtOutputFolder.Text))
            {
                MessageBox.Show("Please select an output folder.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private async Task ProcessFileAsync(string inputFile, string keyword, string outputFolder, bool addEndOfLine)
        {
            LogMessage("Starting file processing...");

            var fileInfo = new FileInfo(inputFile);
            LogMessage($"Input file size: {fileInfo.Length:N0} bytes");

            // Ensure output folder exists
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
                LogMessage($"Created output folder: {outputFolder}");
            }

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

                        LogMessage($"Found program: O{currentProgramNumber}");
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
                        LogMessage($"Processed {lineCount:N0} lines, found {programCount} programs...");
                        Application.DoEvents(); // Keep UI responsive
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

            LogMessage($"Total lines processed: {lineCount:N0}");
            LogMessage($"Total programs found: {programs.Count}");

            // Write output files
            await WriteOutputFilesAsync(programs, outputFolder);
        }

        private async Task WriteOutputFilesAsync(List<ProgramData> programs, string outputFolder)
        {
            LogMessage("Writing output files...");

            progressBar.Maximum = programs.Count;
            progressBar.Value = 0;

            int filesWritten = 0;

            foreach (var program in programs)
            {
                try
                {
                    var fileName = $"O{program.ProgramNumber}.nc";
                    var filePath = Path.Combine(outputFolder, fileName);

                    // Add % at the beginning and end of the program
                    var content = $"%\r\n{program.Content.TrimEnd()}\r\n%\r\n";

                    await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
                    filesWritten++;

                    progressBar.Value = filesWritten;
                    lblStatus.Text = $"Writing files... {filesWritten}/{programs.Count}";

                    if (filesWritten % 10 == 0)
                    {
                        Application.DoEvents(); // Keep UI responsive
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Error writing file for program O{program.ProgramNumber}: {ex.Message}");
                }
            }

            LogMessage($"Successfully wrote {filesWritten} files to: {outputFolder}");

            // Display summary
            LogMessage("=== Summary ===");
            LogMessage($"Programs processed: {programs.Count}");
            LogMessage($"Files written: {filesWritten}");
            LogMessage($"Output folder: {outputFolder}");
            LogMessage("");
            LogMessage("First 10 files created:");

            foreach (var program in programs.Take(10))
            {
                var fileName = $"O{program.ProgramNumber}.nc";
                LogMessage($"  - {fileName}");
            }

            if (programs.Count > 10)
            {
                LogMessage($"  ... and {programs.Count - 10} more files");
            }
        }

        private void LogMessage(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action<string>(LogMessage), message);
                return;
            }

            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtLog.ScrollToCaret();
        }
    }

    // Entry point for GUI version
    public static class ProgramGUI
    {
        [STAThread]
        public static void MainGUI()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FileSplitterForm());
        }
    }
}