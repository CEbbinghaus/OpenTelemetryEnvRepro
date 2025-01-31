using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string ServiceName = "test-service";
const string ServiceVersion = "0.0.1";

Environment.SetEnvironmentVariable("OTEL_DOTNET_EXPERIMENTAL_ASPNETCORE_DISABLE_URL_QUERY_REDACTION", "True"); // This does not work

Debug.Assert(Environment.GetEnvironmentVariable(
    "OTEL_DOTNET_EXPERIMENTAL_ASPNETCORE_DISABLE_URL_QUERY_REDACTION") == "True"); // This is true


var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(ServiceName, serviceVersion: ServiceVersion)
    .AddAttributes(new Dictionary<string, object>
    {
        { "attribute", "value"}
    });

var traceProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(ServiceName)
    .SetResourceBuilder(resourceBuilder)
    .AddHttpClientInstrumentation()
    .AddConsoleExporter()
    .Build();

using (var activitySource = new ActivitySource(ServiceName))
{
    HttpClient client = new HttpClient();
    var result = await client.GetAsync("https://postman-echo.com/get?key=SuperImportantValueThatWeNeedToKnow&another=ValueThatDoesntNeedToBeRedacted");
}
