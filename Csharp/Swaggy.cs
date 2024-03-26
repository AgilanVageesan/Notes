using System;
using System.IO;
using System.Reflection;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;

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

            var generatorOptions = serviceProvider.GetRequiredService<IOptions<SwaggerGenOptions>>().Value;
            generatorOptions.OperationFilter<RemoveVersionFromParameter>();
            generatorOptions.DocumentFilter<ReplaceVersionWithExactValueInPath>();

            var app = new ApplicationBuilder(serviceProvider);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var document = generatorOptions.GenerateAsync("v1", app.Services).Result;

            var filePath = Path.Combine(outputPath, "openapi.yaml");

            using (var writer = new StreamWriter(filePath))
            {
                var serializer = new SerializerBuilder().Build();
                serializer.Serialize(writer, document);
            }

            Console.WriteLine($"OpenAPI spec file generated at: {filePath}");
        }
    }

    public class RemoveVersionFromParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters.SingleOrDefault(p => p.Name == "version");
            if (versionParameter != null)
                operation.Parameters.Remove(versionParameter);
        }
    }

    public class ReplaceVersionWithExactValueInPath : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var (path, pathItem) in swaggerDoc.Paths.ToList())
            {
                var updatedPath = path.Replace("{version}", "v1");
                if (updatedPath != path)
                {
                    swaggerDoc.Paths.Remove(path);
                    swaggerDoc.Paths.Add(updatedPath, pathItem);
                }
            }
        }
    }
}
