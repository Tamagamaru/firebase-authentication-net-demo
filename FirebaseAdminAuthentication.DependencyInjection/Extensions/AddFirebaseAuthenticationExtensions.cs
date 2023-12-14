using System;
using FirebaseAdminAuthentication.DependencyInjection.Options;
using FirebaseAdminAuthentication.DependencyInjection.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace FirebaseAdminAuthentication.DependencyInjection.Extensions
{
    public static class AddFirebaseAuthenticationExtensions
    {
        public static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services, Action<FirebaseAuthenticationSchemeOptions> options = null)
        {
            options ??= _ => { };
            
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddScheme<FirebaseAuthenticationSchemeOptions, FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, options);

            services.AddScoped<FirebaseAuthenticationFunctionHandler>();

            return services;
        }
    }
}
