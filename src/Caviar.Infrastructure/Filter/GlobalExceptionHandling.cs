// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Caviar.Core.Services;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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
            catch (ResultException ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, ResultException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            var json = JsonSerializer.Serialize(ex.ResultMsg);
            return context.Response.WriteAsync(json);
        }


        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            SysLog sysLog;
            using (var provider = Configure.ServiceProvider.CreateScope())
            {
                var logServices = provider.ServiceProvider.GetRequiredService<LogServices<GlobalExceptionHandling>>();
                if (ex.InnerException != null)
                {
                    sysLog = logServices.Error(ex.InnerException.Message);
                }
                else
                {
                    sysLog = logServices.Error(ex.Message);
                }

            }

            //todo
            ResultMsg resultMsg = new ResultMsg()
            {
                TraceId = sysLog.TraceId,
                Status = System.Net.HttpStatusCode.InternalServerError,
                Title = CurrencyConstant.InternalServerError,
                Detail = new Dictionary<string, string>()
                {
                    { "Id", sysLog?.Id.ToString() },
                    { "Message",ex.Message },
                }
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

