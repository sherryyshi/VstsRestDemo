using System;
using System.Collections.Generic;
using System.Net.Http;

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

                VstsHttpClient vstsHttpClient = connection.GetClient<VstsHttpClient>();

                string branchName = "yourBranchName";

                var payload = new
                {
                    itemVersion = new { version = branchName, versionType = "branch" },
                    historyMode = "FirstParent",
                };

                string requestPath = $"{projectName}/_apis/git/repositories/{repoName}/commitsBatch?api-version=3.1-preview";

                var result = vstsHttpClient.CallApi<List<GitCommitRef>>(requestPath, HttpMethod.Post, payload);

                foreach (var commit in result)
                {
                    Console.WriteLine(commit.CommitId);
                    Console.WriteLine(commit.Comment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1}", ex.GetType(), ex.Message);
            }
        }
    }
}
