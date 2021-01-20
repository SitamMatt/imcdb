using McMaster.Extensions.CommandLineUtils;
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace slnf
{
    class Program
    {
        [Option("-s|--solution", Description = "Path to solution file")]
        public string SolutionPath { get; set; }
        [Option("-o|--output", Description = "Path of the output solution filter")]
        public string OutputPath { get; set; }
        [Option("-i|--include")]
        public List<string> IncludedProjects { get; set; }

        static void Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        private void OnExecute()
        {
            var fullSolutionPath = Path.GetFullPath(SolutionPath);
            var fullOutputPath = Path.GetFullPath(OutputPath);
            var solution = SolutionFile.Parse(fullSolutionPath);
            var projects = solution.ProjectsInOrder;

            var filterPath = fullOutputPath;

            var filter = new Solution()
            {
                Path = GetRelativePath(OutputPath, SolutionPath),
                Projects = projects.Where(p => IncludedProjects
                .Any(x => p.ProjectName.Contains(x)))
                .Select(p => p.RelativePath).ToList()
            };
            var root = new Root(filter);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            using var fs = File.Create(filterPath);

            JsonSerializer.SerializeAsync(fs, root, options).Wait();
        }

        public record Solution
        {
            [JsonPropertyName("path")]
            public string Path { get; set; }
            [JsonPropertyName("projects")]
            public List<string> Projects { get; set; }
        }

        public record Root(Solution solution);

        public static string GetRelativePath(string path1, string path2)
        {
            var slnPath = Path.GetFullPath(path1);
            var outPath = Path.GetFullPath(path2);
            var slnDirPath = Path.GetDirectoryName(slnPath);
            var outDirPath = Path.GetDirectoryName(outPath);
            var relative = Path.GetRelativePath(outDirPath, slnDirPath);
            var finalPath = Path.Combine(relative, path2);
            return finalPath;
        }
    }
}
