using DomainLayer.Exceptions;
using Shared.ErrorModels;

namespace Her_Journey.CustomMiddleWares
{
    public class CustomExpceptionHandlerMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExpceptionHandlerMiddleWare> _logger;

        public CustomExpceptionHandlerMiddleWare(RequestDelegate Next, ILogger<CustomExpceptionHandlerMiddleWare> logger)
        {
            _next = Next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
                await HandleNotFoundEndPointAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something Went Wrong");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var Response = new ErrorToReturn
            {
                StatusCode = context.Response.StatusCode,
                ErrorMessage = ex.Message
            };
            // set status code for response
            //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.StatusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                BadRequestException badRequestException => GetBadRequestErrors(badRequestException, Response),
                _ => StatusCodes.Status500InternalServerError
            };


            // set content type for response
            //context.Response.ContentType = "application/json"; malhash lazma

            //Respone Object
            //Return Object as json

            await context.Response.WriteAsJsonAsync(Response);
        }

        private static int GetBadRequestErrors(BadRequestException badRequestException, ErrorToReturn response)
        {
            response.Errors = badRequestException.Errors;
            return StatusCodes.Status400BadRequest;
        }

        private static async Task HandleNotFoundEndPointAsync(HttpContext context)
        {
            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                var Response = new ErrorToReturn
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorMessage = $"End Point {context.Request.Path} is not found"
                };
                await context.Response.WriteAsJsonAsync(Response);
            }
        }
    }
}
