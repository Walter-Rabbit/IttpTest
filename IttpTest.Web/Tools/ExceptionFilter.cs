using IttpTest.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IttpTest.Web.Tools;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = new JsonResult(context.Exception.Message)
        {
            StatusCode = context.Exception switch
            {
                LoginAlreadyExistsException => StatusCodes.Status400BadRequest,
                IncorrectPasswordException => StatusCodes.Status401Unauthorized,
                RevokedException => StatusCodes.Status403Forbidden,
                ForbiddenException => StatusCodes.Status403Forbidden,
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            }
        };

        context.ExceptionHandled = true;
    }
}