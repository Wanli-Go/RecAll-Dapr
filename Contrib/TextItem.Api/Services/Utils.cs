using Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RecAll.Infrastructure.Infrastructure.Api;
using Serilog;
using TheSalLab.GeneralReturnValues;

public static class Utils
{
    public static void AddCustomApplicationServices(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IIdentityService, MockIdentityService>();
    }

    public static void AddCustomSwagger(this WebApplicationBuilder builder) =>
        builder.Services.AddSwaggerGen();


    public static readonly string AppName = typeof(Utils).Namespace;
    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        var seqServerUrl = builder.Configuration["Serilog:SeqServerUrl"];

        Log.Logger = new LoggerConfiguration().ReadFrom
            .Configuration(builder.Configuration).WriteTo.Console().WriteTo
            .Seq(seqServerUrl).Enrich.WithProperty("ApplicationName", AppName)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void AddInvalidModelStateResponseFactory(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions().PostConfigure<ApiBehaviorOptions>(options => {
            options.InvalidModelStateResponseFactory = context =>
                new OkObjectResult(ServiceResult
                    .CreateInvalidParameterResult(
                        new ValidationProblemDetails(context.ModelState).Errors
                            .Select(p =>
                                $"{p.Key}: {string.Join(" / ", p.Value)}"))
                    .ToServiceResultViewModel());
        });
    }

    public static void
        AddCustomHealthChecks(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDapr()
            .AddSqlServer(builder.Configuration["ConnectionStrings:TextItemContext"]!,
                name: "TextListDb-check", tags: new[] { "TextListDb" });

}

