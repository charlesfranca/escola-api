using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using EscolaDeVoce.Services.Message;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;
using System.Net;

namespace SimpleTokenProvider
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;
        EscolaDeVoce.Services.Interfaces.IUserService _userService;

        public TokenProviderMiddleware(
            RequestDelegate next,
            IOptions<TokenProviderOptions> options,
            EscolaDeVoce.Services.Interfaces.IUserService userService)
        {
            _next = next;
            _options = options.Value;
            _userService = userService;
        }

        public Task Invoke(HttpContext context)
        {
            // If the request path doesn't match, skip
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                return _next(context);
            }

            // Request must be POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST")
               || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            return GenerateToken(context);
        }

        private Task<ClaimsIdentity> GetIdentity(string username, string password, bool isFacebook)
        {
            var req = new UserLoginRequest();
            req.password = password;
            req.username = username;
            req.isFacebook = isFacebook;

            var response = _userService.UserLogin(req);
            // DON'T do this in production, obviously!
            if (response.user != null)
            {
                var now = DateTime.UtcNow;
                var claims = new Claim[]
                    {
                        new Claim("name", username),
                        new Claim("id", response.user.Id.ToString()),
                    };
                return Task.FromResult(
                    new ClaimsIdentity(new System.Security.Principal.GenericIdentity(response.user.Id.ToString(), "id"), claims));
            }

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }

        private async Task GenerateToken(HttpContext context)
        {
            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];
            bool isFacebook = bool.Parse(context.Request.Form["isFacebook"]);

            var identity = await GetIdentity(username, password, isFacebook);
            var response = new object();
            if (identity == null)
            {
                // context.Response.StatusCode = 400;
                // await context.Response.WriteAsync("Invalid username or password.");
                // return;

                response = new
                {
                    access_token = "",
                    expires_in = 0,
                    StatusCode = HttpStatusCode.NotFound
                };
            }
            else
            {
                var now = DateTime.UtcNow;
                var userid = identity.Claims.Where(c => c.Type == "id").FirstOrDefault().Value.ToString();
                // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
                // You can add other claims here, if you want:
                var claims = new Claim[]
                {
                new Claim("name", username),
                new Claim("id", userid),
                    // new Claim(JwtRegisteredClaimNames.Sub, username),
                    // new Claim(JwtRegisteredClaimNames.Jti, userid),
                    // new Claim(JwtRegisteredClaimNames.Iat, now.AddDays(5).Subtract(now).TotalSeconds.ToString(), ClaimValueTypes.Integer64)
                };

                // Create the JWT and write it to a string
                var jwt = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    notBefore: now,
                    expires: now.Add(_options.Expiration),
                    signingCredentials: _options.SigningCredentials);
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                response = new
                {
                    access_token = encodedJwt,
                    expires_in = (int)_options.Expiration.TotalSeconds,
                    StatusCode = HttpStatusCode.Created
                };
            }

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }
    }
}