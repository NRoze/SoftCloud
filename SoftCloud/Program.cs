using SoftCloud.Endpoints;
using SoftCloud.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Cloud-Native Best Practice: Structured JSON Logging (good for containers/Azure)
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();

builder.Services.AddProblemDetails(configure =>
{
    configure.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
    };
});
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Cloud-Native Best Practice: Health Checks for k8s Liveness/Readiness probes
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHelloEndpoint();
app.MapHealthChecks("/health");
app.UseExceptionHandler();

app.Run();
