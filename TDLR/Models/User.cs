using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using Tdlr.Utils;

namespace Tdlr.Models
{
    // A projection of the Graph User object
    public class User
    {
        public User() { }

        public User(Microsoft.Azure.ActiveDirectory.GraphClient.User user)
        {
            City = user.City;
            CompanyName = user.CompanyName;
            Country = user.Country;
            DisplayName = user.DisplayName;
            GivenName = user.GivenName;
            JobTitle = user.JobTitle;
            Mail = user.Mail;
            MailNickname = user.MailNickname;
            ObjectId = user.ObjectId;
            ObjectType = user.ObjectType;
            Surname = user.Surname;
            TelephoneNumber = user.TelephoneNumber;
            UsageLocation = user.UsageLocation;
            UserPrincipalName = user.UserPrincipalName;
            TenantId = ClaimsPrincipal.Current.FindFirst(Globals.TenantIdClaimType).Value;
        }
        public string assignmentStatus { get; set; }
        [Key]
        public string ObjectId { get; set; }
        public string City { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string Country { get; set; }
        public string DisplayName { get; set; }
        public string State { get; set; }
        public string GivenName { get; set; }
        public string Mail { get; set; }
        public string MailNickname { get; set; }
        public string ObjectType { get; set; }
        public string TelephoneNumber { get; set; }
        public string Surname { get; set; }
        public string UsageLocation { get; set; }
        public string UserPrincipalName { get; set; }
        public string TenantId { get; set; }
    }
}