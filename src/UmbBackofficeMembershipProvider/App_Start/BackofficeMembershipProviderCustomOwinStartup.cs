using Microsoft.Owin;
using Owin;
using Umbraco.Core;
using Umbraco.Core.Security;
using Umbraco.Web.Security.Identity;
using UmbBackofficeMembershipProvider;
using Umbraco.Core.Models.Identity;
using Umbraco.Web;
using Umbraco.Core.Configuration;

//To use this startup class, change the appSetting value in the web.config called 
// "owin:appStartup" to be "BackofficeMembershipProviderCustomOwinStartup"

[assembly: OwinStartup("BackofficeMembershipProviderCustomOwinStartup", typeof(BackofficeMembershipProviderCustomOwinStartup))]

namespace UmbBackofficeMembershipProvider
{
    /// <summary>
    /// A custom way to configure OWIN for Umbraco
    /// </summary>
    /// <remarks>
    /// The startup type is specified in appSettings under owin:appStartup - change it to "BackofficeMembershipProviderCustomOwinStartup" to use this class
    /// 
    /// This startup class would allow you to customize the Identity IUserStore and/or IUserManager for the Umbraco Backoffice
    /// </remarks>
    public class BackofficeMembershipProviderCustomOwinStartup : UmbracoDefaultOwinStartup
    {
        /// <summary>
        /// Configure user manager for use with Active Directory
        /// </summary>
        /// <param name="app"></param>
        protected override void ConfigureUmbracoUserManager(IAppBuilder app)
        {
            app.ConfigureUserManagerForUmbracoBackOffice<BackOfficeUserManager, BackOfficeIdentityUser>(
                ApplicationContext,
                (options, context) =>
                {
                    var membershipProvider = MembershipProviderExtensions.GetUsersMembershipProvider().AsUmbracoMembershipProvider();
                    var userManager = BackOfficeUserManager.Create(options,
                        ApplicationContext.Services.UserService,
                        ApplicationContext.Services.EntityService,
                        ApplicationContext.Services.ExternalLoginService,
                        membershipProvider,
                        UmbracoConfig.For.UmbracoSettings().Content);

                    // Configure custom password checker.
                    userManager.BackOfficeUserPasswordChecker = new BackofficeMembershipProviderPasswordChecker();

                    return userManager;
                });
        }
    }
}
