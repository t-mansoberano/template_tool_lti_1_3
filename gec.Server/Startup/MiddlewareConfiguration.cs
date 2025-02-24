using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Authentication;

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
        //app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

//#if !DEBUG
//        app.Use(async (context, next) =>
//        {
//            if (!context.User.Identity.IsAuthenticated)
//            {
//                var path = context.Request.Path.Value.ToLower();
//                //if (path.Contains("vendor") ||
//                //    path.Contains("polyfills") ||
//                //    path.Contains("styles") ||
//                //    path.Contains(".css") ||
//                //    path.Contains("primeicons") ||
//                //    path.Contains("main") ||
//                //    path.Contains("runtime") ||
//                //    path.Contains("api") ||
//                //    path.Contains(".js"))
//                //{
//                    await next();
//                //}
//                //else
//                //    await context.ChallengeAsync(Saml2Constants.AuthenticationScheme);
//            }
//            else
//            {
//                await next();
//            }
//        });
//#endif
        app.MapFallbackToFile("/index.html");
    }
}