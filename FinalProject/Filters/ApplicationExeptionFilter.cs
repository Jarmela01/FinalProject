using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace FinalProject.Filters
{
    public class ApplicationExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ApplicationExceptionFilter> _logger;

        public ApplicationExceptionFilter(ILogger<ApplicationExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            var baseException = context.Exception.GetBaseException();

            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new JsonResult(new
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorMessage = baseException.Message
            });

            _logger.LogError(baseException, baseException.Message);

            base.OnException(context);
        }
    }
}