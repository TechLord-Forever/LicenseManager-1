﻿using System;
using System.Linq;
using LicenseManager.Authentication;
using LicenseManager.Authentication.Providers;
using LicenseManager.Authentication.Providers.LocalUser;
using LicenseManager.Services;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace LicenseManager
{
	public partial class Startup
	{
        public void ConfigureAuth(ConfigurationService config, AuthenticationService auth, IAppBuilder app)
        {
            // Get the local user auth provider, if present and attach it first
            Authenticator localUserAuther;
            if (auth.Authenticators.TryGetValue(Authenticator.GetName(typeof(LocalUserAuthenticator)), out localUserAuther))
            {
                // Configure cookie auth now
                localUserAuther.Startup(config, app);
            }

            // Attach external sign-in cookie middleware
            app.SetDefaultSignInAsAuthenticationType(AuthenticationTypes.External);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = AuthenticationTypes.External,
                AuthenticationMode = AuthenticationMode.Passive,
                CookieName = ".AspNet.ILM." + AuthenticationTypes.External,
                ExpireTimeSpan = TimeSpan.FromMinutes(5)
            });

            // Attach non-cookie auth providers
            var nonCookieAuthers = auth
                .Authenticators
                .Where(p => !String.Equals(
                    p.Key,
                    Authenticator.GetName(typeof(LocalUserAuthenticator)),
                    StringComparison.OrdinalIgnoreCase))
                .Select(p => p.Value);

            foreach (var auther in nonCookieAuthers)
            {
                auther.Startup(config, app);
            }
        }
    }
}