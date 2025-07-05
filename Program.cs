namespace FileSplitter
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Check if GUI argument is provided
            if (args.Length > 0 && args[0].ToLower() == "gui")
            {
                // Launch GUI version
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FileSplitterForm());
            }
            else if (args.Length > 0 && args[0].ToLower() == "console")
            {
                // Launch console version directly
                FileSplitterProgram.Main(args).Wait();
            }
            else
            {
                // Show selection dialog
                ShowVersionSelector();
            }
        }

        private static void ShowVersionSelector()
        {
            Console.WriteLine("=== File Splitter ===");
            Console.WriteLine("Choose the version you want to use:");
            Console.WriteLine("1. GUI Version (Windows Forms)");
            Console.WriteLine("2. Console Version");
            Console.WriteLine("3. Exit");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Enter your choice (1-3): ");
                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        // Launch GUI version
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new FileSplitterForm());
                        return;

                    case "2":
                        // Launch console version
                        FileSplitterProgram.Main(new string[0]).Wait();
                        return;

                    case "3":
                        Console.WriteLine("Goodbye!");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                        break;
                }
            }
        }
    }
}