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
            
            if (!Directory.GetFiles(path).Where(p => p.EndsWith(".sln")).Any())
            {
                Console.WriteLine("No .sln file found, invalid directory");
                Console.ReadKey();
                Environment.Exit(1);
            }

            //valid solution directory
            string[] assemblies = Directory.GetDirectories(path).Where(p => !(p.EndsWith(".git") || p.EndsWith(".vs"))).ToArray();
            Console.WriteLine(assemblies.Length);
            if (assemblies.Length == 0)
            {
                Console.WriteLine("No assemblies found, invalid directory");
                Console.ReadKey();
                Environment.Exit(2);
            }

            //has assemblies
            Console.WriteLine("Found the following assemblies: ");
            foreach (string assembly in assemblies)
            {
                Console.WriteLine(Path.GetFileName(assembly));
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("----------------------------------------------");
            (int files, int lines)[] analysisResults = Array.ConvertAll(assemblies, a => AnalyseAssembly(a));
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

        public static (int files, int lines) AnalyseAssembly(string path)
        {
            string debugFolder = Path.Combine(path, "debug");
            string objFolder = Path.Combine(path, "obj");
            string[] codePaths = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories).Where(p => !(p.StartsWith(debugFolder) || p.StartsWith(objFolder))).ToArray();
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