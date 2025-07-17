using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

[ApiController]
[Route(".well-known")]
public class WellKnownController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public WellKnownController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("openid_configuration")]
    public IActionResult GetDiscoveryDocument()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var issuer = _configuration.GetValue<string>("IdentityServer:IssuerUri") ?? baseUrl;

        var discoveryDocument = new
        {
            issuer = issuer,
            authorization_endpoint = $"{baseUrl}/connect/authorize",
            token_endpoint = $"{baseUrl}/connect/token",
            userinfo_endpoint = $"{baseUrl}/connect/userinfo",
            end_session_endpoint = $"{baseUrl}/connect/endsession",
            check_session_iframe = $"{baseUrl}/connect/checksession",
            revocation_endpoint = $"{baseUrl}/connect/revocation",
            introspection_endpoint = $"{baseUrl}/connect/introspect",
            device_authorization_endpoint = $"{baseUrl}/connect/deviceauthorization",
            frontchannel_logout_supported = true,
            frontchannel_logout_session_supported = true,
            backchannel_logout_supported = true,
            backchannel_logout_session_supported = true,
            scopes_supported = new[]
            {
                "openid",
                "profile",
                "email",
                "role",
                "microservice.api",
                "user.api",
                "order.api",
                "payment.api",
                "notification.api",
                "full_access"
            },
            claims_supported = new[]
            {
                "sub",
                "name",
                "email",
                "email_verified",
                "phone_number",
                "phone_number_verified",
                "role"
            },
            grant_types_supported = new[]
            {
                "authorization_code",
                "client_credentials",
                "refresh_token",
                "implicit",
                "password",
                "urn:ietf:params:oauth:grant-type:device_code"
            },
            response_types_supported = new[]
            {
                "code",
                "token",
                "id_token",
                "id_token token",
                "code id_token",
                "code token",
                "code id_token token"
            },
            response_modes_supported = new[]
            {
                "form_post",
                "query",
                "fragment"
            },
            token_endpoint_auth_methods_supported = new[]
            {
                "client_secret_basic",
                "client_secret_post"
            },
            id_token_signing_alg_values_supported = new[]
            {
                "RS256"
            },
            subject_types_supported = new[]
            {
                "public"
            },
            code_challenge_methods_supported = new[]
            {
                "plain",
                "S256"
            },
            request_parameter_supported = true
        };

        return Ok(discoveryDocument);
    }
}
