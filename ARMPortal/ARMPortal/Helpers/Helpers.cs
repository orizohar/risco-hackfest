#region Using Statements

using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

#endregion

namespace ARMPortal.Helpers
{
    public static class Helpers
    {
        private static readonly string ClientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static readonly string AppKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static readonly string TenantId = ConfigurationManager.AppSettings["ida:TenantId"];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetAccessTokenAsync()
        {
            var cc = new ClientCredential(ClientId, AppKey);
            var context = new AuthenticationContext($"https://login.windows.net/{TenantId}", TokenCache.DefaultShared);
            var token = context.AcquireToken("https://management.azure.com/", cc);
            if (token == null)
            {
                throw new InvalidOperationException("Could not get the token.");
            }
            return token.AccessToken;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="groupName"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static async Task<ResourceGroup> CreateResourceGroupAsync(TokenCredentials credential, string groupName,
            string subscriptionId, string location)
        {
            try
            {
                using (var resourceManagementClient = new ResourceManagementClient(credential))
                {
                    resourceManagementClient.SubscriptionId = subscriptionId;
                    var resourceGroup = new ResourceGroup { Location = location };
                    return await resourceManagementClient.ResourceGroups.CreateOrUpdateAsync(groupName, resourceGroup);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="groupName"></param>
        /// <param name="deploymentName"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="templateLink"></param>
        /// <param name="templateParametersLink"></param>
        /// <returns></returns>
        public static async Task<DeploymentExtended> CreateTemplateDeploymentAsync(TokenCredentials credential,
            string groupName, string deploymentName, string subscriptionId, string templateLink,
            string templateParametersLink)
        {
            var deployment = new Deployment
            {
                Properties = new DeploymentProperties
                {
                    Mode = DeploymentMode.Incremental,
                    TemplateLink = new TemplateLink(templateLink),
                    ParametersLink = new ParametersLink(templateParametersLink)
                }
            };

            try
            {
                using (var resourceManagementClient = new ResourceManagementClient(credential))
                {
                    resourceManagementClient.SubscriptionId = subscriptionId;
                    return await resourceManagementClient.Deployments.CreateOrUpdateAsync(groupName, deploymentName,
                        deployment);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }
    }
}