using System;
using System.IO;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

namespace OpenApiGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: dotnet run <project-path> <output-path>");
                return;
            }

            string projectPath = args[0];
            string outputPath = args[1];

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
            var startupAssembly = Assembly.LoadFrom(Path.Combine(projectPath, "bin", "Debug", $"{Path.GetFileNameWithoutExtension(projectPath)}.dll"));

            var services = new ServiceCollection();
            var applicationLifetime = new ApplicationLifetime();
            services.AddSingleton<IHostApplicationLifetime>(applicationLifetime);
            services.AddMvcCore().AddApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                c.CustomOperationIds(apiDesc =>
                {
                    return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
                });
            });

            var serviceProvider = services.BuildServiceProvider();

            var generatorOptions = serviceProvider.GetRequiredService<IOptions<SwaggerGenOptions>>().Value;
            generatorOptions.OperationFilter<RemoveVersionFromParameter>();
            generatorOptions.DocumentFilter<ReplaceVersionWithExactValueInPath>();

            var generator = serviceProvider.GetRequiredService<ISwaggerProvider>();
            var document = generator.GetSwagger("v1");

            var filePath = Path.Combine(outputPath, "openapi.yaml");

            using (var streamWriter = new StreamWriter(filePath))
            {
                document.SerializeAsYaml(streamWriter.BaseStream);
            }

            Console.WriteLine($"OpenAPI spec file generated at: {filePath}");
        }
    }

    public class RemoveVersionFromParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters.Single(p => p.Name == "version");
            operation.Parameters.Remove(versionParameter);
        }
    }

    public class ReplaceVersionWithExactValueInPath : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var path in swaggerDoc.Paths)
            {
                var updatedPath = path.Key.Replace("v{version}", "v1");
                swaggerDoc.Paths[updatedPath] = path.Value;
                if (updatedPath != path.Key)
                {
                    swaggerDoc.Paths.Remove(path.Key);
                }
            }
        }
    }
}
