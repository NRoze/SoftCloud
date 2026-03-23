using System.ComponentModel.DataAnnotations;

namespace SoftCloud.Endpoints
{
    static public class HelloEndpoint
    {
        extension (WebApplication app)
        {
            public RouteHandlerBuilder MapHelloEndpoint()
            {
                return app.MapGet("", () =>
                {
                    //throw new ValidationException();
                    return "Hello from GitHub Actions!";
                });
            }
        }
    }
}
