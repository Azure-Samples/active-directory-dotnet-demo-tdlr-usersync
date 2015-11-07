using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Tdlr.Models
{
    public class User : Microsoft.Azure.ActiveDirectory.GraphClient.User
    {
        public User(Microsoft.Azure.ActiveDirectory.GraphClient.User user)
        {
            AccountEnabled = user.AccountEnabled;
            AppRoleAssignments = user.AppRoleAssignments;
            AssignedLicenses = user.AssignedLicenses;
            AssignedPlans = user.AssignedPlans;
            City = user.City;
            CompanyName = user.CompanyName;
            Country = user.Country;
            CreatedObjects = user.CreatedObjects;
            CreatedOnBehalfOf = user.CreatedOnBehalfOf;
            DeletionTimestamp = user.DeletionTimestamp;
            Department = user.Department;
            DirectReports = user.DirectReports;
            DirSyncEnabled = user.DirSyncEnabled;
            DisplayName = user.DisplayName;
            FacsimileTelephoneNumber = user.FacsimileTelephoneNumber;
            GivenName = user.GivenName;
            ImmutableId = user.ImmutableId;
            JobTitle = user.JobTitle;
            LastDirSyncTime = user.LastDirSyncTime;
            Mail = user.Mail;
            MailNickname = user.MailNickname;
            Manager = user.Manager;
            MemberOf = user.MemberOf;
            Members = user.Members;
            Mobile = user.Mobile;
            Oauth2PermissionGrants = user.Oauth2PermissionGrants;
            ObjectId = user.ObjectId;
            ObjectType = user.ObjectType;
            OnPremisesSecurityIdentifier = user.OnPremisesSecurityIdentifier;
            OtherMails = user.OtherMails;
            OwnedDevices = user.OwnedDevices;
            OwnedObjects = user.OwnedObjects;
            Owners = user.Owners;
            PasswordPolicies = user.PasswordPolicies;
            PasswordProfile = user.PasswordProfile;
            PhysicalDeliveryOfficeName = user.PhysicalDeliveryOfficeName;
            PostalCode = user.PostalCode;
            PreferredLanguage = user.PreferredLanguage;
            ProvisionedPlans = user.ProvisionedPlans;
            ProvisioningErrors = user.ProvisioningErrors;
            ProxyAddresses = user.ProxyAddresses;
            RegisteredDevices = user.RegisteredDevices;
            SipProxyAddress = user.SipProxyAddress;
            State = user.State;
            StreetAddress = user.StreetAddress;
            Surname = user.Surname;
            TelephoneNumber = user.TelephoneNumber;
            UsageLocation = user.UsageLocation;
            UserPrincipalName = user.UserPrincipalName;
            UserType = user.UserType;
        }
        public string assignmentStatus { get; set; }
    }
}