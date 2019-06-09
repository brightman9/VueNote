using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using VueNote.WebApi.ViewModels;

namespace VueNote.WebApi.Common
{
    public class ExceptionHandler : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.ToString());

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var result = new { Error = "API服务器发生异常：" + ex.Message };
                await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }
        }
    }
}
