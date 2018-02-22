using System;

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace VstsRestDemo
{
    class Program
    {
        // TODO: Update the following constants
        private const string collectionUrl = "https://yourdomain.visualstudio.com";
        private const string projectName = "yourProjectName";
        private const string repoName = "yourRepoName";

        public static void Main(string[] args)
        {
            try
            {
                // Specify "true" in VssClientCredentials constructor to use default windows credentials.
                VssConnection connection = new VssConnection(new Uri(collectionUrl), new VssClientCredentials(true));
                
                // Get a GitHttpClient to use the Git API
                GitHttpClient gitHttpClient = connection.GetClient<GitHttpClient>();

                // TODO: Update yourBranchName
                var branch = gitHttpClient.GetBranchAsync(projectName, repoName, "yourBranchName").Result;
                Console.WriteLine(branch.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1}", ex.GetType(), ex.Message);
            }
        }
    }
}
