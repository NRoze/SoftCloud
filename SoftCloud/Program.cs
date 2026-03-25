using Azure.Monitor.OpenTelemetry.AspNetCore;
using SoftCloud.Endpoints;
using SoftCloud.Middlewares;

var builder = WebApplication.CreateBuilder(args);

//#region TBD
//// Define the credential once
//var credential = new DefaultAzureCredential();

//// Example: Using it with Key Vault
//var keyVaultUri = builder.Configuration["KeyVault:Uri"];

//if (!string.IsNullOrEmpty(keyVaultUri))
//{
//    // DefaultAzureCredential handles the auth
//    // AddAzureKeyVault handles the configuration mapping
//    builder.Configuration.AddAzureKeyVault(
//        new Uri(keyVaultUri),
//        new DefaultAzureCredential());
//}
//// Example: Using it with a specific client (e.g., Blob Storage)
//// builder.Services.AddSingleton(new BlobServiceClient(uri, credential));

//#endregion

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();

builder.Services.AddOpenTelemetry().UseAzureMonitor(options =>
{ 
    options.ConnectionString = builder.Configuration["ConnectionStrings:ApplicationInsights"];
});
builder.Services.AddProblemDetails(configure =>
{
    configure.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
    };
});
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHelloEndpoint();
app.MapHealthChecks("/health");
app.UseExceptionHandler();

app.Run();
