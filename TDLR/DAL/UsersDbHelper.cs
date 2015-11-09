using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Tdlr.Models;
using System.Data.Entity;

namespace Tdlr.DAL
{
    public class UsersDbHelper
    {
        public static List<User> GetUsersForTenant(string tenantId)
        {
            TdlrContext db = new TdlrContext();
            List<User> users = db.Users.Where(u => u.TenantId == tenantId).AsNoTracking().ToList();
            return users ?? new List<User>();
        }

        public static void SaveUsersForTenant(string tenantId, List<User> users)
        {
            TdlrContext db = new TdlrContext();
            List<User> existingUsers = db.Users.Where(u => u.TenantId == tenantId).AsNoTracking().ToList();
            foreach (User user in users)
            {
                User existingUser = existingUsers.Where(u => u.ObjectId == user.ObjectId).FirstOrDefault();
                if (existingUser != null)
                {
                    db.Users.Attach(user);
                    db.Entry(user).State = EntityState.Modified;
                }
                else
                {
                    db.Users.Add(user);
                }
            }
            db.SaveChanges();
        }

        public static void ClearUsersForTenant(string tenantId)
        {
            TdlrContext db = new TdlrContext();
            List<User> existingUsers = db.Users.Where(u => u.TenantId == tenantId).ToList();
            foreach (User user in existingUsers)
            {
                db.Users.Remove(user);
            }
            db.SaveChanges();
        }
    }
}