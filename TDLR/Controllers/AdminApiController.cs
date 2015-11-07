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
using Tdlr.Utils;

namespace Tdlr.Controllers
{
    public class AdminApiController : ApiController
    {
        private static Dictionary<string, List<Models.User>> _assignedUsersByTenant = new Dictionary<string, List<Models.User>>();

        [HttpGet]
        [HostAuthentication("AADBearer")]
        [Authorize]
        public async Task<List<Models.User>> GetAssignedUsers()
        {
            string tenantId = ClaimsPrincipal.Current.FindFirst(Globals.TenantIdClaimType).Value;
            ActiveDirectoryClient graphClient = new ActiveDirectoryClient(new Uri(Globals.GraphApiUrl, tenantId), async () => await GraphHelper.AcquireToken());

            List<Models.User> assignedUsersHistory;
            if (!_assignedUsersByTenant.TryGetValue(tenantId, out assignedUsersHistory))
                assignedUsersHistory = new List<Models.User>();
            List<Models.User> assignedUsersWithStatus = new List<Models.User>(assignedUsersHistory);
            List<Models.User> updatedUsersHistory = new List<Models.User>(assignedUsersHistory);

            ServicePrincipal sp = (ServicePrincipal)graphClient.ServicePrincipals.Where(servicePrincpial => servicePrincpial.AppId.Equals(ConfigHelper.ClientId)).ExecuteAsync().Result.CurrentPage.FirstOrDefault();
            IServicePrincipalFetcher spFetcher = sp;
            List<IAppRoleAssignment> assignments = spFetcher.AppRoleAssignedTo.ExecuteAsync().Result.CurrentPage.ToList(); // TODO: Paging

            // TODO: Error Handling
            foreach (IAppRoleAssignment assignment in assignments)
            {
                if (assignment.PrincipalType == "User")
                {
                    User user = (User) await graphClient.Users.GetByObjectId(assignment.PrincipalId.ToString()).ExecuteAsync();
                    int existingUserIndex = assignedUsersHistory.FindIndex(u => u.ObjectId == user.ObjectId);
                    if (existingUserIndex == -1)
                    {
                        assignedUsersWithStatus.Add(new Models.User(user) { assignmentStatus = "New" });
                        updatedUsersHistory.Add(new Models.User(user));
                    }
                    else
                    {
                        assignedUsersWithStatus[assignedUsersWithStatus.FindIndex(u => u.ObjectId == user.ObjectId)] = new Models.User(user) { assignmentStatus = "Enabled" };
                        updatedUsersHistory[existingUserIndex] = new Models.User(user);
                    }
                }
                else if (assignment.PrincipalType == "Group")
                {
                    IGroupFetcher gFetcher = graphClient.Groups.GetByObjectId(assignment.PrincipalId.ToString());
                    List<IDirectoryObject> members = gFetcher.Members.ExecuteAsync().Result.CurrentPage.ToList(); // TODO: Paging
                    foreach (IDirectoryObject member in members)
                    {
                        if (member is User)
                        {   // TODO: Nested Groups
                            User user = (User)member;
                            int existingUserIndex = assignedUsersHistory.FindIndex(u => u.ObjectId == user.ObjectId);
                            if (existingUserIndex == -1 && assignedUsersWithStatus.FindIndex(u => u.ObjectId == user.ObjectId) == -1)
                            {
                                assignedUsersWithStatus.Add(new Models.User(user) { assignmentStatus = "New" });
                                updatedUsersHistory.Add(new Models.User(user));
                            }
                            else if (existingUserIndex >= 0)
                            {
                                assignedUsersWithStatus[assignedUsersWithStatus.FindIndex(u => u.ObjectId == user.ObjectId)] = new Models.User(user) { assignmentStatus = "Enabled" };
                                updatedUsersHistory[existingUserIndex] = new Models.User(user);
                            }
                        }
                    }
                }
            }

            _assignedUsersByTenant[tenantId] = updatedUsersHistory;
            return assignedUsersWithStatus;
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