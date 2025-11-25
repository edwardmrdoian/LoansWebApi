using Loans.Domain.Errors;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Loans.Api.Swagger
{
    public class SwaggerErrorResponsesOperationFilter : IOperationFilter
    {
        private static OpenApiSchema ErrorResponseSchema => new()
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["errorCode"] = new OpenApiSchema { Type = "string" },
                ["message"] = new OpenApiSchema { Type = "string" },
                ["statusCode"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                ["details"] = new OpenApiSchema { Type = "array", Items = new OpenApiSchema { Type = "string" } },
                ["timestamp"] = new OpenApiSchema { Type = "string", Format = "date-time" }
            }
        };

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var schemaKey = nameof(ErrorResponse);
            if (!context.SchemaRepository.Schemas.ContainsKey(schemaKey))
            {
                context.SchemaRepository.Schemas.Add(schemaKey, ErrorResponseSchema);
            }

            AddResponse(operation, "400", "Bad Request");
            AddResponse(operation, "401", "Unauthorized");
            AddResponse(operation, "403", "Forbidden");
            AddResponse(operation, "404", "Not Found");
            AddResponse(operation, "500", "Internal Server Error");
        }
        private static void AddResponse(OpenApiOperation operation, string statusCode, string description)
        {
            if (operation.Responses.ContainsKey(statusCode)) return;

            operation.Responses[statusCode] = new OpenApiResponse
            {
                Description = description,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema { Reference = new OpenApiReference { Id = nameof(ErrorResponse), Type = ReferenceType.Schema } }
                    }
                }
            };
        }
    }
}
