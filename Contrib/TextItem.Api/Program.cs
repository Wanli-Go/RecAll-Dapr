using Dapr.Client;
using Dapr.Extensions.Configuration;
using HealthChecks.UI.Client;
using Microsoft.EntityFrameworkCore;
using Polly;
using RecAll.Contrib.TextItem.Api.Services;
using RecAll.Infrastructure.Infrastructure.Api;

static Policy CreateRetryPolicy()
{
    return Policy.Handle<Exception>().WaitAndRetryForever(
        sleepDurationProvider: i => TimeSpan.FromSeconds(2^i),
        onRetry: (exception, retry, _) => {
            Console.WriteLine(
                "Exception {0} with message {1} detected during database migration (retry attempt {2})",
                exception.GetType().Name, exception.Message, retry);
        });
}

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddDaprSecretStore("recall-secretstore", new DaprClientBuilder().Build());

builder.Services.AddDbContext<TextItemContext>(p => p.UseSqlServer(builder.Configuration["ConnectionStrings:TextItemContext"]));

builder.Services.AddControllers();

builder.Services.AddHealthChecks();

builder.Services.AddDaprClient();


Utils.AddCustomSerilog(builder);

Utils.AddCustomApplicationServices(builder);

Utils.AddCustomSwagger(builder);

Utils.AddInvalidModelStateResponseFactory(builder);

builder.AddCustomHealthChecks();

var app = builder.Build();
Console.Write(builder.Configuration["ConnectionStrings:TextItemContext"]);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
app.MapControllers();

// Apply Database Migration
using var scope = app.Services.CreateScope();

var retryPolicy = CreateRetryPolicy();
var context =
    scope.ServiceProvider.GetRequiredService<TextItemContext>();

retryPolicy.Execute(context.Database.Migrate);

app.MapCustomHealthChecks(
    responseWriter: UIResponseWriter.WriteHealthCheckUIResponse);


app.Run();


