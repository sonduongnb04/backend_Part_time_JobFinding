using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SwaggerFileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParameters = context.ApiDescription.ParameterDescriptions
            .Where(p => p.Type == typeof(IFormFile))
            .ToList();

        if (!fileParameters.Any())
            return;

        // Keep route parameters (like {id}) but remove form file parameters
        var parametersToKeep = operation.Parameters
            .Where(p => context.ApiDescription.ParameterDescriptions
                .Any(pd => pd.Name == p.Name && pd.Source.Id == "Path"))
            .ToList();

        operation.Parameters.Clear();
        foreach (var param in parametersToKeep)
        {
            operation.Parameters.Add(param);
        }

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = fileParameters.ToDictionary(
                            p => p.Name,
                            p => new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            }
                        ),
                        Required = new HashSet<string>(fileParameters.Select(p => p.Name))
                    }
                }
            }
        };
    }
}
