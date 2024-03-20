public static class Utils
{
    public static void AddCustomApplicationServices(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IIdentityService, MockIdentityService>();
    }

    public static void AddCustomSwagger(this WebApplicationBuilder builder) =>
        builder.Services.AddSwaggerGen();
}

