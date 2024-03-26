using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YamlDotNet.Serialization;

namespace OpenApiGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the path to the .NET Framework project or solution:");
            string projectPath = Console.ReadLine();

            Console.WriteLine("Enter the output path for the OpenAPI specification:");
            string outputPath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(projectPath) || string.IsNullOrWhiteSpace(outputPath))
            {
                Console.WriteLine("Invalid input. Both project path and output path are required.");
                return;
            }

            if (!Directory.Exists(projectPath))
            {
                Console.WriteLine("Project path does not exist.");
                return;
            }

            if (!Directory.Exists(outputPath))
            {
                Console.WriteLine("Output path does not exist.");
                return;
            }

            GenerateOpenApiSpec(projectPath, outputPath);
        }

        static void GenerateOpenApiSpec(string projectPath, string outputPath)
        {
            string assemblyPath;
            
            // Check if the given path is a solution file
            if (Path.GetExtension(projectPath).Equals(".sln", StringComparison.OrdinalIgnoreCase))
            {
                var solutionParser = new Microsoft.Build.Locator.MSBuildLocator();
                solutionParser.RegisterDefaults();
                var solution = Microsoft.Build.Locator.MSBuildWorkspace.Create().OpenSolutionAsync(projectPath).Result;
                var project = solution.Projects.FirstOrDefault();
                if (project == null)
                {
                    Console.WriteLine("No projects found in the solution.");
                    return;
                }
                assemblyPath = project.OutputFilePath;
            }
            // Check if the given path is a DLL
            else if (Path.GetExtension(projectPath).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            {
                assemblyPath = projectPath;
            }
            else
            {
                Console.WriteLine("Invalid project path. Please provide a path to a solution file (.sln) or a DLL.");
                return;
            }

            var startupAssembly = Assembly.LoadFrom(assemblyPath);

            var services = new ServiceCollection();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                c.CustomOperationIds(apiDesc =>
                {
                    return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
                });
            });

            var serviceProvider = services.BuildServiceProvider();

            var generator = serviceProvider.GetRequiredService<ISwaggerProvider>();
            var document = generator.GetSwagger("v1", null, null, "/");

            var filePath = Path.Combine(outputPath, "openapi.yaml");

            using (var writer = new StreamWriter(filePath))
            {
                var serializer = new SerializerBuilder().Build();
                serializer.Serialize(writer, document);
            }

            Console.WriteLine($"OpenAPI spec file generated at: {filePath}");
        }
    }
}
