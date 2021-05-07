using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Extensions
{
    public static class StatusMessageExtensions
    {
        private const string BootstrapSuccessClass = "success";
        private const string BootstrapInfoClass = "info";
        private const string BootstrapWarningClass = "warning";
        private const string BootstrapDangerClass = "danger";

        public static IActionResult WithSuccess(this IActionResult result, string title, string body)
        {
            return Alert(result, BootstrapSuccessClass, title, body);
        }

        public static IActionResult WithInfo(this IActionResult result, string title, string body)
        {
            return Alert(result, BootstrapInfoClass, title, body);
        }

        public static IActionResult WithWarning(this IActionResult result, string title, string body)
        {
            return Alert(result, BootstrapWarningClass, title, body);
        }

        public static IActionResult WithDanger(this IActionResult result, string title, string body)
        {
            return Alert(result, BootstrapDangerClass, title, body);
        }

        private static IActionResult Alert(IActionResult result, string type, string title, string body)
        {
            return new StatusMessageDecoratorResult(result, type, title, body);
        }
    }
}