using System;
using Microsoft.AspNetCore.Owin;
using Owin;

[assembly: Microsoft.Owin.OwinStartupAttribute(typeof(eShopLegacyMVC.Startup))]
namespace eShopLegacyMVC
{
    public partial class Startup
    {
        public void Configuration(Owin.IAppBuilder app)
        {
            // Comment out the ConfigureAuth call until authentication is properly configured
            // ConfigureAuth(app);
        }

        // Simple placeholder for ConfigureAuth method
        private void ConfigureAuth(IAppBuilder app)
        {
            // Authentication configuration will be implemented here
            // This is just a placeholder to fix the compile error
        }
    }
}