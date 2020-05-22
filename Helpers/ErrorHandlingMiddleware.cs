using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace myMicroservice.Helpers
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        //public ErrorHandlingMiddleware(RequestDelegate next)
        //{
        //    this.next = next;
        //    _logger = null!;
        //}

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            this.next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError;

            if (ex is DbUpdateException) code = HttpStatusCode.Conflict;
            
            var rawCode = (int)code;

            var detailMessage = "";
            #if DEBUG
            detailMessage = ex.Message;
            #endif
            var problem = new ProblemDetails
            {
                Title = code.ToString(),
                Status = rawCode,
                Detail = detailMessage,
                Type = "https://httpstatuses.com/" + rawCode,
                Instance = context.TraceIdentifier
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                IgnoreNullValues = true
            };

            var result = JsonSerializer.Serialize(problem, serializeOptions);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = rawCode;

            _logger.LogError(ex, context.TraceIdentifier);

            return context.Response.WriteAsync(result);
        }
    }
}
