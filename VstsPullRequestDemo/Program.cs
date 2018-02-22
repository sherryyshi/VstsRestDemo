using System;

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.WebApi;

// WIT
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Collections.Generic;

// Identity
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.Identity;

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
                /*
                 * Authentication
                 */

                // Specify "true" in VssClientCredentials constructor to use default windows credentials.
                VssConnection connection = new VssConnection(new Uri(collectionUrl), new VssClientCredentials(true));

                /*
                 * Add a work item to a pull request
                 */

                // Get a GitHttpClient to use the Git API
                GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

                // Get a WorkItemTrackingHttpClient to use the WIT API
                WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

                // TODO: Update the pullRequestId and workItemId
                int pullRequestId = 0000000;
                int workItemId = 000000;

                GitPullRequest pullRequest = gitClient.GetPullRequestAsync(projectName, repoName, pullRequestId).Result;

                string pullrequesturl = "vstfs:///Git/PullRequestId/" + pullRequest.Repository.ProjectReference.Id + "%2F" + pullRequest.Repository.Id + "%2F" + pullRequest.PullRequestId;

                JsonPatchOperation operation = new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new WorkItemRelation()
                    {
                        Attributes = new Dictionary<string, object>()
                        {
                            { "name", "Pull Request" }
                        } ,
                        Rel = "ArtifactLink",
                        Url = pullrequesturl
                    }
                };

                JsonPatchDocument document = new JsonPatchDocument();
                document.Add(operation);

                WorkItem addWorkItemResult = witClient.UpdateWorkItemAsync(document, workItemId).Result;

                /*
                 * Add a reviewer to the pull request
                 */

                // Get a IdentityHttpClient to use the Identity API
                IdentityHttpClient identityClient = connection.GetClient<IdentityHttpClient>();

                // TODO: Update the reviewer
                string reviewer = "janedoe@fakedomain.com";

                // Get a list of identities that match the filter (which will hopefully only contain 1 member)
                IdentitiesCollection identities = identityClient.ReadIdentitiesAsync(IdentitySearchFilter.General, filterValue: reviewer).Result;

                // Worth doing some validation here before accessing the list in production code.
                Identity identity = identities[0];

                var identityRefWithVote = new IdentityRefWithVote();
                identityRefWithVote.Id = identity.Id.ToString();
                identityRefWithVote.Vote = 10; // Approved. See https://docs.microsoft.com/en-us/rest/api/vsts/git/Pull%20Request%20Reviewers/Create%20Pull%20Request%20Reviewer

                IdentityRefWithVote addReviewerResult = gitClient.CreatePullRequestReviewerAsync(identityRefWithVote, project: projectName, repositoryId: repoName, pullRequestId: pullRequestId, reviewerId:identity.Id.ToString()).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1}", ex.GetType(), ex.Message);
            }
        }
    }
}
