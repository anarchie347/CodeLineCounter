using System.Security.Cryptography.X509Certificates;

namespace CodeLineCounter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "";
            if (args.Length == 0)
            {
                Console.WriteLine("Enter path");
                path = Console.ReadLine();
                while (!Directory.Exists(path))
                {
                    Console.WriteLine("Invalid path. Enter path");
                }
            } else
            {
                path = args[0];
            }
            Console.WriteLine("Enter file extensions to search, separated by space. Do not include the .");
            string[] fileExtensions = (Console.ReadLine()?.Split(" ", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>()).ToArray();

            //valid directory
            Console.WriteLine("Enter excluded top level folder names, separated by spaces. '.git .vs build node_modules' are excluded automatically");
            string[] ignorePreset = new string[] { ".git", ".vs", "build", "node_modules" };
            string[] ignoreFolders = (Console.ReadLine()?.Split(" ", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>()).Concat(ignorePreset).ToArray();
            string[] TopLevelFolders = Directory.GetDirectories(path).Where(p => !ignoreFolders.Any(f => p.EndsWith(f))).ToArray();
            if (TopLevelFolders.Length == 0)
            {
                Console.WriteLine("No top level folders found, invalid directory");
                Console.ReadKey();
                Environment.Exit(2);
            }

            //has TopLevelFolders
            Console.WriteLine("Found the following assemblies: ");
            foreach (string topLevelFolder in TopLevelFolders)
            {
                Console.WriteLine(Path.GetFileName(topLevelFolder));
            }
            Console.WriteLine("Enter excluded sub folder names, separated by spaces");
            ignoreFolders = Console.ReadLine()?.Split(" ", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("----------------------------------------------");
            (int files, int lines)[] analysisResults = Array.ConvertAll(TopLevelFolders, a => AnalyseAssembly(a, ignoreFolders, fileExtensions));
            int totalFiles = analysisResults.Sum(a => a.files);
            int totalLines = analysisResults.Sum(a => a.lines);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"TOTAL");
            Console.WriteLine($"Files: {totalFiles}");
            Console.WriteLine($"Lines: {totalLines}");

            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        }

        public static (int files, int lines) AnalyseAssembly(string path, string[] ignoreFolders, string[] fileExtensions)
        {
            ignoreFolders = ignoreFolders.Select(f => Path.Combine(path, f)).ToArray();
            
            string[] codePaths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(p => !ignoreFolders.Any(f => p.StartsWith(f))).ToArray();
            codePaths = codePaths.Where(cp => fileExtensions.Any(fe => cp.EndsWith($".{fe}"))).ToArray();
            int fileCount = codePaths.Length;
            int lines = codePaths.Sum(p => File.ReadAllLines(p).Length);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Assembly: {Path.GetFileName(path)}");
            Console.WriteLine($"Files: {fileCount}");
            Console.WriteLine($"Lines: {lines}");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("----------------------------------------------");
            return (fileCount, lines);
        }
    }
}