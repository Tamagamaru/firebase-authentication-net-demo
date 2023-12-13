﻿using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdminAuthentication.DependencyInjection.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FirebaseAdminAuthentication.DependencyInjection.Services
{
    public class FirebaseAuthenticationFunctionHandler
    {
        private const string BEARER_PREFIX = "Bearer ";

        private readonly FirebaseApp _firebaseApp;

        public FirebaseAuthenticationFunctionHandler(FirebaseApp firebaseApp)
        {
            _firebaseApp = firebaseApp;
        }

        public Task<AuthenticateResult> HandleAuthenticateAsync(HttpRequest request) =>
            HandleAuthenticateAsync(request.HttpContext);

        public async Task<AuthenticateResult> HandleAuthenticateAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.NoResult();
            }

            string bearerToken = context.Request.Headers["Authorization"];

            if (bearerToken == null || !bearerToken.StartsWith(BEARER_PREFIX))
            {
                return AuthenticateResult.Fail("Invalid scheme.");
            }

            string token = bearerToken.Substring(BEARER_PREFIX.Length);

            try
            {
                FirebaseToken firebaseToken = await FirebaseAuth.GetAuth(_firebaseApp).VerifyIdTokenAsync(token, true);

                return AuthenticateResult.Success(CreateAuthenticationTicket(firebaseToken));
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        private AuthenticationTicket CreateAuthenticationTicket(FirebaseToken firebaseToken)
        {
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(ToClaims(firebaseToken.Claims), nameof(ClaimsIdentity))
            });

            return new AuthenticationTicket(claimsPrincipal, JwtBearerDefaults.AuthenticationScheme);
        }

        private IEnumerable<Claim> ToClaims(IReadOnlyDictionary<string, object> claims)
        {
            Dictionary<string, string> standardClaims = new Dictionary<string, string>
            {
                {"user_id", FirebaseUserClaimType.ID},
                {"email", FirebaseUserClaimType.EMAIL},
                {"email_verified", FirebaseUserClaimType.EMAIL_VERIFIED},
                {"name", FirebaseUserClaimType.USERNAME}
            };

            var newClaims = new List<Claim>();
            
            foreach (var (key, value) in claims)
            {
                if (standardClaims.TryGetValue(key, out var claimType))
                {
                    newClaims.Add(new Claim(claimType, value?.ToString() ?? ""));
                }
                // Since Firebase custom claims must have unique keys, use an array to add multiple roles to a user 
                else if (key == ClaimTypes.Role && value is JArray roles)
                {
                    newClaims.AddRange(roles.Select(
                        role => new Claim(ClaimTypes.Role, role.ToString())
                    ));
                }
                // Other custom claims
                else
                {
                    newClaims.Add(new Claim(key, value?.ToString() ?? ""));
                }
            }

            return newClaims;
        }
    }
}
