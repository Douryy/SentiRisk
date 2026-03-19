
        using System.Net;
using System.Text.Json;
using SentiRisk.Exceptions;

namespace SentiRisk.Middleware
    {
        /// <summary>
        /// Intercepte toutes les exceptions et retourne une réponse JSON uniforme.
        /// À enregistrer en PREMIER dans le pipeline (Program.cs).
        /// </summary>
        public class ErrorHandlingMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<ErrorHandlingMiddleware> _logger;

            public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
            {
                _next = next;
                _logger = logger;
            }

            public async Task Invoke(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (AppException ex)
                {
                    // Erreurs métier connues (404, 401, 422…)
                    _logger.LogWarning("[{Code}] {Message} - {Path}",
                        ex.StatusCode, ex.Message, context.Request.Path);

                    await WriteErrorResponse(context, ex.StatusCode, ex.Message);
                }
                catch (Exception ex)
                {
                    // Erreurs inattendues → 500
                    _logger.LogError(ex, "Erreur inattendue sur {Path}", context.Request.Path);

                    await WriteErrorResponse(context,
                        (int)HttpStatusCode.InternalServerError,
                        "Une erreur interne est survenue.");
                }
            }

            private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                var response = new
                {
                    success = false,
                    code = statusCode,
                    error = message,
                    path = context.Request.Path.Value,
                    timestamp = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(json);
            }
        }
    }

