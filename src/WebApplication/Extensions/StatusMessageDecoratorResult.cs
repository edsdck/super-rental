using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication.Extensions
{
    public class StatusMessageDecoratorResult : IActionResult
    {
        public IActionResult Result { get; }

        public string Type { get; }

        public string Title { get; }

        public string Body { get; }

        public StatusMessageDecoratorResult(IActionResult result, string type, string title, string body)
        {
            Result = result;
            Type = type;
            Title = title;
            Body = body;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var factory = context.HttpContext.RequestServices.GetService<ITempDataDictionaryFactory>();
            if (factory is null || string.IsNullOrEmpty(Body))
            {
                return;
            }

            var tempData = factory.GetTempData(context.HttpContext);
            tempData["_status.type"] = Type;
            tempData["_status.title"] = Title;
            tempData["_status.body"] = Body;

            await Result.ExecuteResultAsync(context);
        }
    }
}