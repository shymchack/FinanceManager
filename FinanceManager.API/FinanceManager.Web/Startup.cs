﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinanceManager.Web.Startup))]
namespace FinanceManager.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
