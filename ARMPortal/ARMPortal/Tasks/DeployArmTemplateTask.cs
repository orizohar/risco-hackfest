#region Using Statements

using System;
using System.Threading;
using System.Threading.Tasks;
using ARMPortal.Models;
using Microsoft.Rest;

#endregion

namespace ARMPortal.Tasks
{
    // Static can be removed
    public static class DeployArmTemplateTask
    {
        public static async Task<TaskExecutionResult> DeployTemplateAsync(string environmentName,
            string selectedSubscription, string selectedRegion)
        {
            var accessToken = Helpers.Helpers.GetAccessTokenAsync();
            var accessCredentials = new TokenCredentials(accessToken);

            var groupName = $"{environmentName}-RG";
            var deploymentName = $"{environmentName}-RG";
            var subscriptionId = "[DEV Subscription ID]";
            var region = selectedRegion;

            switch (selectedSubscription)
            {
                case "Production":
                    subscriptionId = "[Prod Subscription ID]";
                    break;
                case "QA":
                    subscriptionId = "[QA Subscription ID]";
                    break;
                case "Dev":
                    subscriptionId = "[DEV Subscription ID]";
                    break;
                default:
                    subscriptionId = "[DEV Subscription ID]";
                    break;
            }


            var resourceGroupResult =
                await Helpers.Helpers.CreateResourceGroupAsync(accessCredentials, groupName, subscriptionId,
                    region);

            if (resourceGroupResult.Properties.ProvisioningState != "Succeeded")
            {
                return new TaskExecutionResult()
                {
                    TaskExecutionMessage = $"Creating resource group {groupName} failed.",
                    TaskExecutionStatus = Enums.TaskStatus.Failed,
                };
            }

            var deploymentResult =
                Helpers.Helpers.CreateTemplateDeploymentAsync(accessCredentials, groupName, deploymentName,
                    subscriptionId, "[ARM JSON Template File URL with extension]",
                    "[ARM JSON Teplate Parameters File URL with extension]");

            while (deploymentResult.Status == TaskStatus.Running ||
                   deploymentResult.Status == TaskStatus.WaitingForActivation)
            {
                await Task.Delay(500);
            }

            if (resourceGroupResult.Properties.ProvisioningState == "Succeeded")
            {
                return new TaskExecutionResult()
                {
                    TaskExecutionMessage = $"Deployment {deploymentName} succeeded.",
                    TaskExecutionStatus = Enums.TaskStatus.Succeeded,
                };
            }
            else
            {
                return new TaskExecutionResult()
                {
                    TaskExecutionMessage = $"Deployment {deploymentName} failed.",
                    TaskExecutionStatus = Enums.TaskStatus.Failed,
                };
            }
        }
    }
}