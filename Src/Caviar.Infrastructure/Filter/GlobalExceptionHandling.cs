using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Caviar.Infrastructure
{
    public class GlobalExceptionHandling
    {
        private readonly RequestDelegate _next;
        public GlobalExceptionHandling(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (NotificationException ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, NotificationException ex)
        {
            //todo
            ResultMsg resultMsg = ex.ResultMsg;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            var json = JsonSerializer.Serialize(resultMsg);
            return context.Response.WriteAsync(json);
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            //todo
            ResultMsg resultMsg = new ResultMsg()
            {
                Status = System.Net.HttpStatusCode.InternalServerError,
                Title = CurrencyConstant.InternalServerError,
                Detail = "发生未处理异常，请联系管理员在日志中查看，异常id："
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            var json = JsonSerializer.Serialize(resultMsg);
            return context.Response.WriteAsync(json);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CustomExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandling>();
        }
    }
}

