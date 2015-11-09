using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Tdlr.DAL;
using Tdlr.Utils;

namespace Tdlr.Controllers
{
    public class AdminApiController : ApiController
    {
        [HttpGet]
        [HostAuthentication("AADBearer")]
        [Authorize]
        public async Task<List<Models.User>> GetAssignedUsers()
        {
            try
            {
                // Create the Graph Client
                string tenantId = ClaimsPrincipal.Current.FindFirst(Globals.TenantIdClaimType).Value;
                ActiveDirectoryClient graphClient = new ActiveDirectoryClient(new Uri(Globals.GraphApiUrl, tenantId), async () => await GraphHelper.AcquireTokenAsApp());

                // Read users from db for evaluating in memory
                List<Models.User> userHistory = UsersDbHelper.GetUsersForTenant(tenantId);
                List<Models.User> usersWithStatus = new List<Models.User>(userHistory);
                List<Models.User> updatedUserHistory = new List<Models.User>(userHistory);

                // Get the assignments for the application
                ServicePrincipal sp = (ServicePrincipal)graphClient.ServicePrincipals.Where(servicePrincpial => servicePrincpial.AppId.Equals(ConfigHelper.ClientId)).ExecuteAsync().Result.CurrentPage.FirstOrDefault();
                IServicePrincipalFetcher spFetcher = sp;
                List<IAppRoleAssignment> assignments = spFetcher.AppRoleAssignedTo.ExecuteAsync().Result.CurrentPage.ToList(); // TODO: Paging

                // TODO: Better Error Handling
                // TODO: Retry Logic
                // TODO: Nested Groups
                // TODO: Paged Results on Assignments
                // TODO: Paged Results on Group Membership
                // TODO: Performance & Batch Queries

                // Get the groups assigned to the app first
                foreach (IAppRoleAssignment assignment in assignments)
                {
                    if (assignment.PrincipalType == "Group")
                    {
                        // Get the group members
                        IGroupFetcher gFetcher = graphClient.Groups.GetByObjectId(assignment.PrincipalId.ToString());
                        List<IDirectoryObject> members = gFetcher.Members.ExecuteAsync().Result.CurrentPage.ToList();

                        foreach (IDirectoryObject member in members)
                        {
                            if (member is User)
                            {   
                                User user = (User)member;
                                int existingUserIndex = userHistory.FindIndex(u => u.ObjectId == user.ObjectId);

                                // If the user did not exist in the db before
                                if (existingUserIndex == -1)
                                {
                                    // The user is new
                                    usersWithStatus.Add(new Models.User(user) { assignmentStatus = "New" });
                                    updatedUserHistory.Add(new Models.User(user));
                                }
                                else
                                {
                                    // The user is active, but not new
                                    usersWithStatus[usersWithStatus.FindIndex(u => u.ObjectId == user.ObjectId)] = new Models.User(user) { assignmentStatus = "Enabled" };
                                    updatedUserHistory[existingUserIndex] = new Models.User(user);
                                }
                            }
                        }
                    }
                }

                // Get the users assigned to the app second
                foreach (IAppRoleAssignment assignment in assignments)
                {
                    if (assignment.PrincipalType == "User")
                    {
                        int existingUserIndex = userHistory.FindIndex(u => u.ObjectId == assignment.PrincipalId.ToString());
                        int assignedUserIndex = usersWithStatus.FindIndex(u => u.ObjectId == assignment.PrincipalId.ToString());

                        // If we haven't seen the user before, add it
                        if (existingUserIndex == -1 &&  assignedUserIndex == -1)
                        {
                            User user = (User)await graphClient.Users.GetByObjectId(assignment.PrincipalId.ToString()).ExecuteAsync();
                            usersWithStatus.Add(new Models.User(user) { assignmentStatus = "New" });
                            updatedUserHistory.Add(new Models.User(user));
                        }

                        // If we have seen the user before but didn't already update his data as part of group assignment, update the user data.
                        else if (existingUserIndex >= 0 && string.IsNullOrEmpty(usersWithStatus[assignedUserIndex].assignmentStatus))
                        {
                            User user = (User)await graphClient.Users.GetByObjectId(assignment.PrincipalId.ToString()).ExecuteAsync();
                            usersWithStatus[usersWithStatus.FindIndex(u => u.ObjectId == user.ObjectId)] = new Models.User(user) { assignmentStatus = "Enabled" };
                            updatedUserHistory[existingUserIndex] = new Models.User(user);
                        }
                    }
                }

                UsersDbHelper.SaveUsersForTenant(tenantId, updatedUserHistory);
                return usersWithStatus;
            }
            catch (AdalException ex)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }

        [HttpGet]
        [HostAuthentication("AADBearer")]
        [Authorize]
        public void ClearUserTable()
        {
            string tenantId = ClaimsPrincipal.Current.FindFirst(Globals.TenantIdClaimType).Value;
            UsersDbHelper.ClearUsersForTenant(tenantId);
        }

    }
}

//int batchCount = 0;
//List<Task<IBatchElementResult[]>> requests = new List<Task<IBatchElementResult[]>>();
//List<IReadOnlyQueryableSetBase<IDirectoryObject>> batch = new List<IReadOnlyQueryableSetBase<IDirectoryObject>>();
//IEnumerator<IAppRoleAssignment> assignmentIndex = assignments.GetEnumerator();
//IEnumerator<IAppRoleAssignment> nextAssignment = assignments.GetEnumerator();
//nextAssignment.MoveNext();
//while (assignmentIndex.MoveNext())
//{
//    IAppRoleAssignment assignment = assignmentIndex.Current; // for delegate capture
//    if (assignment.PrincipalType == "Group")
//    {
//        batch.Add(graphClient.DirectoryObjects.Where(o => o.ObjectId.Equals(assignment.PrincipalId.ToString())));
//        batchCount++;
//    }
//    else if (assignment.PrincipalType == "User")
//    {
//        IGroupFetcher gFetcher = new Group { ObjectId = assignment.PrincipalId.ToString() };
//        batch.Add(gFetcher.Members);
//        batchCount++;
//    }
//    if (!nextAssignment.MoveNext() || batchCount == 5)
//    {
//        requests.Add(graphClient.Context.ExecuteBatchAsync(batch.ToArray()));
//        batchCount = 0;
//        batch.Clear();
//    }
//}

//IBatchElementResult[][] responses = await Task.WhenAll<IBatchElementResult[]>(requests);
//foreach (IBatchElementResult[] batchResult in responses)
//{
//    foreach (IBatchElementResult query in batchResult)
//    {
//        if (query.SuccessResult != null && query.FailureResult == null)
//        {
//            if (query.SuccessResult.CurrentPage.First() is User)
//            {
//                User user = query.SuccessResult.CurrentPage.First() as User;
//                assignedUsers.Add(user);
//            }
//        }
//        else
//        {
//            throw new Exception("Error calling Graph API for users.");
//        }
//    }
//}