using System.Net.Mime;

namespace EventApp.Middlewares
{
    public class ErrorHandlerMiddleWare
    {
        private readonly RequestDelegate _next;
        public ErrorHandlerMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                ILogger logger = context.RequestServices.GetService<ILogger<ErrorHandlerMiddleWare>>()!;

                response.ContentType = MediaTypeNames.Text.Plain;

                context.Response.StatusCode = error switch
                {
                    Exception => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError,
                };

                logger.LogError(error.Message);
                await response.WriteAsync(error.Message);
            }
        }
    }
}
