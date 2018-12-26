using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using SubscriptionMonitoring.Models;

namespace SubscriptionMonitoring
{
    /// <summary>
    /// Azure Function to monitor Storage Accounts and apply HttpsOnly access policy
    /// </summary>
    public static class StorageHttpsPolicyFunction
    {
        [FunctionName("StorageHttpsOnlyPolicy")]
        public static async Task<IActionResult> Run(
             [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
             ILogger log)
        {
            string storageAccountName;
            var outputStatus = string.Empty;
            try
            {
                log.LogInformation("StorageHttpsPolicy: Begin to read activity log");
                outputStatus += $"\n StorageHttpsPolicy: Begin to read activity log";
                var requestData = await new StreamReader(req.Body).ReadToEndAsync();
                var rootData = JsonConvert.DeserializeObject<Rootobject>(requestData);
                if (rootData.data.context.activityLog.resourceType.Contains("storageAccounts"))
                {
                    var subscriptionId = rootData.data.context.activityLog.subscriptionId;
                    log.LogInformation($"StorageHttpsPolicy: Found Subscription Id {subscriptionId}");
                    outputStatus += $"\n StorageHttpsPolicy: Found Subscription Id {subscriptionId}";
                    var resourceId = rootData.data.context.activityLog.resourceId;
                    log.LogInformation($"StorageHttpsPolicy: Found Resource Id {resourceId}");
                    outputStatus += $"\n StorageHttpsPolicy: Found Resource Id {resourceId}";
                    storageAccountName = rootData.data.context.activityLog.resourceId.Substring(resourceId.LastIndexOf("storageAccounts", StringComparison.Ordinal) + 16).Split("/")[0];
                    log.LogInformation($"StorageHttpsPolicy: Found Storage Account: {storageAccountName}");
                    outputStatus += $"\n StorageHttpsPolicy: Found Storage Account: {storageAccountName}";
                    var token = Authenticate().Result;
                    var credentials = new AzureCredentials(new TokenCredentials(token), new TokenCredentials(token), string.Empty, AzureEnvironment.AzureGlobalCloud);
                    var azure = Azure.Configure().WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic).Authenticate(credentials).WithSubscription(subscriptionId);
                    var storageAccounts = azure.StorageAccounts.List();
                    var storageAccount = storageAccounts.FirstOrDefault(x => x.Name == storageAccountName);
                    storageAccount?.Update().WithOnlyHttpsTraffic().Apply();
                    log.LogInformation("StorageHttpsPolicy: Policy application successful");
                    outputStatus += $"\n StorageHttpsPolicy: Policy application successful";
                }
                else
                {
                    outputStatus += $"\n StorageHttpsPolicy: Couldn't find Storage Account";
                }
            }
            catch (Exception x)
            {
                log.LogInformation("StorageHttpsPolicy: Policy application failed");
                outputStatus += $"\n StorageHttpsPolicy: Policy application failed with {x.Message}";
                log.LogInformation(x.Message);
            }


            return (ActionResult)new OkObjectResult(outputStatus);
        }

        public static async Task<string> Authenticate()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://management.azure.com").ConfigureAwait(false);
            return accessToken;
        }
    }
}
