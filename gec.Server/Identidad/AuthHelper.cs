using System.Diagnostics.CodeAnalysis;
using Serilog;
using gec.Models.Common;

namespace gec.ServerIdentidad
{
    [ExcludeFromCodeCoverage]
    public class AuthHelper
    {
        private readonly IConfiguration Configuration;
        private readonly IWebHostEnvironment Environment;
        private readonly IHttpContextAccessor HttpContextAccessor;

        public AuthHelper(IConfiguration configuration, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            Environment = environment;
            HttpContextAccessor = httpContextAccessor;
        }

        public UserClaims GetClaims()
            {
            UserClaims userClaims = new();

            try
            {
                if (Environment.IsDevelopment()) // Esta sección es para que funcione localmente sin redirigir a la federación.
                    Configuration.GetSection("UserImpersonation").Bind(userClaims);
                else
                {
                    if (HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    {
                        var claims = HttpContextAccessor.HttpContext.User.Identities.First().Claims.ToList();

                        /* Con el siguiente código se recorre cada uno de los claims y se escriben en el Log */
                        // StringBuilder message = new();
                        // claims.ForEach(claim => { message.AppendFormat($"[ {claim.Type} - {claim.Value} ]", "\t"); });
                        // Log.Information($"Claims | {message}");

                        userClaims.PersonID = claims?.FirstOrDefault(x => x.Type.ToLower().Contains("fullName".ToLower()))?.Value;
                        userClaims.UserType = claims?.FirstOrDefault(x => x.Type.ToLower().Contains("NAM_SAMAccountName".ToLower()))?.Value;
                        userClaims.PayrollID = claims?.FirstOrDefault(x => x.Type.ToLower().Contains("NAM_cn".ToLower()))?.Value;
                        userClaims.Email = claims?.FirstOrDefault(x => x.Type.ToLower().Contains("NAM_upn".ToLower()))?.Value;
                       
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Ocurrió un error en el método GetUserClaims() en la clase Authentication | {e.Message}");
            }

            /* Registra en el log, los atributos del usuario que inicio sesión. */
                 Log.Information($"Usuario Inicio Sesión: [ Nómina: {userClaims.PayrollID} ] [ Email: {userClaims.Email} ] [ ID Persona: {userClaims.PersonID} ] [ Tipo Usuario: {userClaims.UserType} ] ");

            return userClaims;
        }

    }
}
