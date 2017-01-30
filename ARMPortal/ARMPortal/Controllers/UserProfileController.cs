#region Using Statements

using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ARMPortal.Models;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;

#endregion

namespace ARMPortal.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly string _aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private readonly string _appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private readonly string _clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private readonly string _graphResourceId = "https://graph.windows.net";
        private ApplicationDbContext _db = new ApplicationDbContext();

        // GET: UserProfile
        public async Task<ActionResult> Index()
        {
            var tenantId =
                ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            var userObjectId =
                ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            try
            {
                var servicePointUri = new Uri(_graphResourceId);
                var serviceRoot = new Uri(servicePointUri, tenantId);
                var activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
                    async () => await GetTokenForApplication());

                // use the token for querying the graph to get the user details

                var result = await activeDirectoryClient.Users
                    .Where(u => u.ObjectId.Equals(userObjectId))
                    .ExecuteAsync();
                var user = result.CurrentPage.ToList().First();

                return View(user);
            }
            catch (AdalException)
            {
                // Return to error page.
                return View("Error");
            }
                // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
            catch (Exception)
            {
                return View("Relogin");
            }
        }

        public void RefreshSession()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties {RedirectUri = "/UserProfile"},
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        public async Task<string> GetTokenForApplication()
        {
            var signedInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            var tenantId =
                ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            var userObjectId =
                ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
            var clientcred = new ClientCredential(_clientId, _appKey);
            // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's database
            var authenticationContext = new AuthenticationContext(_aadInstance + tenantId,
                new AdalTokenCache(signedInUserId));
            var authenticationResult =
                await
                    authenticationContext.AcquireTokenSilentAsync(_graphResourceId, clientcred,
                        new UserIdentifier(userObjectId, UserIdentifierType.UniqueId));
            return authenticationResult.AccessToken;
        }
    }
}