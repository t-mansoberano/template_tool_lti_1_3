namespace gec.Server.Startup;

public static class MiddlewareConfiguration
{
    public static void UseCustomMiddleware(this WebApplication app)
    {
        app.UseSentryTracing();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseCors();
        app.UseSession();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapFallbackToFile("/index.html");
    }
}