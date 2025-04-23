using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Attributes;
public class CacheAttribute(int DurationInSec) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheService = context.HttpContext.RequestServices.GetRequiredService<IServiceManager>().CacheService;

        var cacheKey = GenerateCacheKey(context.HttpContext.Request);

        var result = await cacheService.GetCacheValueAsync(cacheKey);
        if(!string.IsNullOrEmpty(result))
        {
            //Return Response
            context.Result = new ContentResult()
            {
                ContentType = "application/json",
                StatusCode = StatusCodes.Status200OK,
                Content = result

            };
            return;
        }

        //Execute the endpoint

        var contextResult = await next.Invoke();

        if(contextResult.Result is OkObjectResult okObject && okObject.Value != null)
        {
            var serializedValue = System.Text.Json.JsonSerializer.Serialize(okObject.Value); // comment

            await cacheService.SetCacheValueAsync(cacheKey , serializedValue, TimeSpan.FromSeconds(DurationInSec));
        }

    }


    private string GenerateCacheKey(HttpRequest request)
    {
        var key = new StringBuilder();
        key.Append(request.Path);
        foreach( var item in request.Query.OrderBy(q => q.Key) )
        {
            key.Append($"|{item.Key} - {item.Value}");
        }

        return key.ToString();
    }


}
