using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using gec.Server.Identidad;
using InterfazUnica.Server.Controllers;

namespace decisiones_estudiantiles.Server.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [ExcludeFromCodeCoverage]
    //[EnableCors("CorsPolicy")]
    public class AuthController : ControllerBase
    {
        const string relayStateReturnUrl = "ReturnUrl";
        private readonly Saml2Configuration Saml2Config;
        private readonly IConfiguration Configuration;
        private readonly string SingleLogoutDestination;
        private readonly string SingleLogoutDestinationReturn;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IOptions<Saml2Configuration> saml2Config, IConfiguration configuration, ILogger<AuthController> logger)
        {
            Saml2Config = saml2Config.Value;
            Configuration = configuration;
            SingleLogoutDestination = configuration.GetSection("Saml2:SingleLogoutDestination").Get<string>();
            SingleLogoutDestinationReturn = configuration.GetSection("Saml2:SingleLogoutDestinationReturn").Get<string>();
            _logger = logger;
        }

        [Route("Login")]

        public IActionResult Login(string returnUrl = "")
        {
            _logger.LogInformation("2.-Login");
            var binding = new Saml2RedirectBinding();
            binding.SetRelayStateQuery(new Dictionary<string, string> { { relayStateReturnUrl, returnUrl ?? Url.Content("~/redirect") } });

            return binding.Bind(new Saml2AuthnRequest(Saml2Config)).ToActionResult();
        }

        [Route("AssertionConsumerService")]
        public async Task<IActionResult> AssertionConsumerService()
        {
            try
            {
                _logger.LogInformation("3.-AssertionConsumerService");

                var binding = new Saml2PostBinding();
                var saml2AuthnResponse = new Saml2AuthnResponse(Saml2Config);

                binding.ReadSamlResponse(Request.ToGenericHttpRequest(), saml2AuthnResponse);

                if (saml2AuthnResponse.Status != Saml2StatusCodes.Success)
                    throw new AuthenticationException($"SAML Response status: {saml2AuthnResponse.Status}");

                binding.Unbind(Request.ToGenericHttpRequest(), saml2AuthnResponse);
                await saml2AuthnResponse.CreateSession(HttpContext, lifetime: new TimeSpan(1, 0, 0, 0), claimsTransform: (claimsPrincipal) => ClaimsTransform.Transform(claimsPrincipal));

                var relayStateQuery = binding.GetRelayStateQuery();
                var returnUrl = relayStateQuery.ContainsKey(relayStateReturnUrl) ? relayStateQuery[relayStateReturnUrl] : Url.Content("~/redirect");
                //var returnUrl = relayStateQuery.ContainsKey(relayStateReturnUrl) ? relayStateQuery[relayStateReturnUrl] : Url.Content("~/");



                if (saml2AuthnResponse.Status == Saml2StatusCodes.Success)
                {
                    _logger.LogInformation("#Url  Redirect: redirect #", returnUrl);
                    //GetUsuarioSesion();
                }


                _logger.LogInformation("4.-redirect");
                return Redirect(returnUrl);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception Auth/redirect");
                throw new Exception("Ocurrió un error en AssertionConsumerService", e);
            }
        }
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {

            var binding = new Saml2PostBinding();
            var saml2LogoutRequest = await new Saml2LogoutRequest(Saml2Config, User).DeleteSession(HttpContext);
            return binding.Bind(saml2LogoutRequest).ToActionResult();


        }
        [Route("Logout2")]
        public async Task<IActionResult> Logout2()
        {

#if DEBUG
            return Redirect(Url.Content("http://localhost:4200/"));
#endif
            //if (!User.Identity.IsAuthenticated)
            //{
            //    return Redirect(Url.Content("~/"));
            //}

            try
            {

                var cookies = Request.Cookies;

                if (cookies != null)
                {
                    var fedAuthCookies = cookies.Keys
                   .Where(x => x.IndexOf("FedAuth") != -1)
                   .Select(x => x)
                   .ToList();

                    var FedAuth1 = cookies.Keys
                        .Where(x => x.IndexOf("FedAuth1") != -1)
                        .Select(x => x)
                        .ToList();

                    // Recorre todas las cookies encontradas y les cambia la fecha de espiracion
                    // por ayer, (les quita un dia) despues las vuelce a agregar y se actualizan.
                    if (fedAuthCookies.Count != 0)
                    {
                        foreach (var cookie in fedAuthCookies)
                        {
                            Response.Cookies.Append(cookie, "", new CookieOptions()
                            {
                                Expires = DateTime.Now.AddDays(-1),
                                Secure = true,
                                HttpOnly = true,
                            });
                        }
                    }

                    if (FedAuth1.Count != 0)
                    {
                        foreach (var cookie in FedAuth1)
                        {
                            Response.Cookies.Append(cookie, "", new CookieOptions()
                            {
                                Expires = DateTime.Now.AddDays(-1),
                                Secure = true,
                                HttpOnly = true,
                            });
                        }
                    }
                }

                HttpContext.Session.SetObjectAsJson("usuarioId", null);

#if DEBUG
                return Redirect(Url.Content("~/login"));
#else
 
                var binding = new Saml2PostBinding();
                var saml2LogoutRequest = await new Saml2LogoutRequest(Saml2Config, User).DeleteSession(HttpContext);
                return binding.Bind(saml2LogoutRequest).ToActionResult();
#endif

            }
            catch (Exception e)
            {
                // Exception baseException = _utileriasService.GetFirstException(e);
                throw new Exception(e.Message);
            }

            return Ok();
        }
        [Route("LoggedOut")]
        public IActionResult LoggedOut()
        {
            var binding = new Saml2PostBinding();
            binding.Unbind(Request.ToGenericHttpRequest(), new Saml2LogoutResponse(Saml2Config));
            return Redirect(Url.Content("~/"));
        }

        [Route("SingleLogout2")]
        public async Task<IActionResult> SingleLogout2()
        {
            Saml2StatusCodes status;
            var requestBinding = new Saml2PostBinding();
            var logoutRequest = new Saml2LogoutRequest(Saml2Config, User);
            try
            {
                requestBinding.Unbind(Request.ToGenericHttpRequest(), logoutRequest);
                status = Saml2StatusCodes.Success;
                await logoutRequest.DeleteSession(HttpContext);
            }
            catch (Exception exc)
            {
                // log exception
                Debug.WriteLine("SingleLogout error: " + exc.ToString());
                status = Saml2StatusCodes.RequestDenied;
            }

            var responsebinding = new Saml2PostBinding();
            responsebinding.RelayState = requestBinding.RelayState;
            var saml2LogoutResponse = new Saml2LogoutResponse(Saml2Config)
            {
                InResponseToAsString = logoutRequest.IdAsString,
                Status = status,
            };
            return responsebinding.Bind(saml2LogoutResponse).ToActionResult();
        }


        [Route("SingleLogout")]
        public async Task<IActionResult> SingleLogout()
        {
            var genericHttpRequest = Request.ToGenericHttpRequest();

            var requestBinding = new Saml2PostBinding();
            var logoutRequest = new Saml2LogoutRequest(Saml2Config, User);

            if (new Saml2PostBinding().IsResponse(genericHttpRequest) || new Saml2RedirectBinding().IsResponse(genericHttpRequest))
            {
                try
                {
                    // Flujo del Single Logout desde el Service Provider (SP). Lo dispara esta misma aplicación.
                    var saml2ConfigSP = GetNewSaml2Configuration();
                    // En este escenario es necesario que el Single Logout Destination sea: https://amfsdevl.tec.mx/nidp/saml2/slo
                    saml2ConfigSP.SingleLogoutDestination = new Uri(SingleLogoutDestination);

                    //_logger.LogInformation($"Ingreso a Auth/SingleLogout, dentro del If | Logout desde el Service Provider (SP). " +
                    //                $"El usuario esta autenticado: {User.Identity.IsAuthenticated} | " +
                    //                $"SingleLogoutDestination. {saml2ConfigSP.SingleLogoutDestination.AbsoluteUri}.");

                    var logoutRequestSP = new Saml2LogoutRequest(saml2ConfigSP, User);
                    var requestBindingSP = new Saml2PostBinding();
                    requestBindingSP.Unbind(Request.ToGenericHttpRequest(), logoutRequestSP);
                    await logoutRequestSP.DeleteSession(HttpContext);

                    var responsebindingSP = new Saml2PostBinding
                    {
                        RelayState = requestBindingSP.RelayState
                    };
                    var saml2LogoutResponse = new Saml2LogoutResponse(saml2ConfigSP)
                    {
                        InResponseToAsString = logoutRequestSP.IdAsString,
                        Status = Saml2StatusCodes.Success
                    };

                    return requestBindingSP.Bind(saml2LogoutResponse).ToActionResult();

                }
                catch (Exception)
                {

                    return BadRequest();
                }
            }
            else
            {
                try
                {
                    // Flujo del Single Logout desde el Identity Provider (IdP). Lo dispara NAM posterior al Logout desde otra aplicación.
                    var saml2ConfigIdP = GetNewSaml2Configuration();
                    // En este escenario es necesario que el Single Logout Destination sea: https://amfsdevl.tec.mx/nidp/saml2/slo_return
                    saml2ConfigIdP.SingleLogoutDestination = new Uri(SingleLogoutDestinationReturn);

                    //_logger.LogInformation($"Ingreso a Auth/SingleLogout, dentro del Else | Logout desde el Identity Provider (IdP). " +
                    //                $"El usuario esta autenticado: {User.Identity.IsAuthenticated} | " +
                    //                $"SingleLogoutDestination. {saml2ConfigIdP.SingleLogoutDestination.AbsoluteUri}.");

                    var logoutRequestIdP = new Saml2LogoutRequest(saml2ConfigIdP, User);
                    var requestBindingIdP = new Saml2PostBinding();
                    requestBindingIdP.Unbind(Request.ToGenericHttpRequest(), logoutRequestIdP);
                    await logoutRequestIdP.DeleteSession(HttpContext);

                    var responsebindingSP = new Saml2PostBinding
                    {
                        RelayState = requestBindingIdP.RelayState
                    };
                    var saml2LogoutResponseIdP = new Saml2LogoutResponse(saml2ConfigIdP)
                    {
                        InResponseToAsString = logoutRequestIdP.IdAsString,
                        Status = Saml2StatusCodes.Success
                    };

                    return requestBindingIdP.Bind(saml2LogoutResponseIdP).ToActionResult();
                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }
        }

        private Saml2Configuration GetNewSaml2Configuration()
        {
            try
            {
                var saml2Configuration = new Saml2Configuration
                {
                    Issuer = Configuration["Saml2:Issuer"],
                    SingleSignOnDestination = new Uri(Configuration["Saml2:SingleSignOnDestination"]),
                    SingleLogoutDestination = new Uri(Configuration["Saml2:SingleLogoutDestination"]),
                    SignatureAlgorithm = Configuration["Saml2:SignatureAlgorithm"],
                    SignAuthnRequest = Convert.ToBoolean(Configuration["Saml2:SignAuthnRequest"]),
                    SigningCertificate = CertificateUtil.Load(Configuration["Saml2:SigningCertificateFile"], Configuration["Saml2:SigningCertificatePassword"], X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet),
                    CertificateValidationMode = (X509CertificateValidationMode)Enum.Parse(typeof(X509CertificateValidationMode), Configuration["Saml2:CertificateValidationMode"]),
                    RevocationMode = (X509RevocationMode)Enum.Parse(typeof(X509RevocationMode), Configuration["Saml2:RevocationMode"])
                };
                saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);
                var entityDescriptor = new EntityDescriptor();
                entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(Configuration["Saml2:IdPMetadata"]));

                if (entityDescriptor.IdPSsoDescriptor != null)
                {
                    saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
                    saml2Configuration.SingleLogoutDestination = entityDescriptor.IdPSsoDescriptor.SingleLogoutServices.First().Location;
                    saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
                }
                else
                { }
                //Log.Error("No se cargó el IdPSsoDescriptor del metadata");

                return saml2Configuration;
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception Auth/GetNewSaml2Configuration");
                throw;
            }
        }

        [HttpGet]
        public IActionResult Profile()
        {
            try
            {
                var isAuthenticated = User.Identity.IsAuthenticated;

#if DEBUG
                isAuthenticated = true;
#endif
                if (isAuthenticated)
                {
                    try
                    {
                        return StatusCode(200);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(403, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {

                return StatusCode(400, ex.Message);
            }

            return StatusCode(401);
        }

        [Route("/")]
        public IActionResult Index()
        {
           return Redirect(Url.Content("~/"));
        }

//         [Route("/")]
//         public IActionResult Index()
//         {
//
// #if DEBUG
//             return Redirect(Url.Content("~/"));
// #else
//
//             if (User.Identity.IsAuthenticated)
//             {
//               
//                 return Redirect(Url.Content("~/redirect"));
//
//             }
//             else
//             {
//                 
//                 return Redirect(Url.Content("~/Auth/Login?ReturnUrl=%2Fredirect"));
//
//
//             }
// #endif
//         }




    }
}