using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace RavenDBMembership
{
    public class DataAccess
    {
        public static string CreateUserInRaven(IDocumentStore store, string applicationName, string username, string password, string email, DateTime dateCreated, string[] roleIDs = null)
        {
            string userId;
            using (var session = store.OpenSession())
            {
                session.Advanced.UseOptimisticConcurrency = true;

                var user = new User();
                user.Username = username;
                password = password.Trim();
                user.PasswordSalt = PasswordUtil.CreateRandomSalt();
                user.PasswordHash = PasswordUtil.HashPassword(password, user.PasswordSalt);
                user.Email = email;
                user.ApplicationName = applicationName;
                user.DateCreated = dateCreated;
        
                if (roleIDs != null)
                {
                    foreach(var role in roleIDs)
                    {
                        user.Roles.Add(role);
                    }
                }

                session.Store(user);
                session.Store(new ReservationForUniqueFieldValue() { Id = "username/" + user.Username });
                session.Store(new ReservationForUniqueFieldValue() { Id = "email/" + user.Email });

                session.SaveChanges();

                userId = user.Id;
            }
            return userId;
        }

        public static string[] GetUsersInRole(IDocumentStore store, string roleName, string applicationName)
        {
            using (var session = store.OpenSession())
            {
                var role = (from r in session.Query<Role>().Customize(q => q.WaitForNonStaleResultsAsOfNow())
                            where r.Name == roleName && r.ApplicationName == applicationName
                            select r).SingleOrDefault();
                if (role != null)
                {
                    var usernames = from u in session.Query<User>().Customize(q => q.WaitForNonStaleResultsAsOfNow())
                                    where u.Roles.Any(a => a == role.Id)
                                    select u.Username;
                    return usernames.ToArray();
                }
                return null;
            }
        }

        public static string CreateRole(IDocumentStore store, string roleName, string applicationName)
        {
            using (var session = store.OpenSession())
            {
                var role = new Role(roleName, null);
                role.ApplicationName = applicationName;

                session.Store(role);
                session.SaveChanges();

                return role.Id;
            }
        }

        public static string GetUserNameByEmail(IDocumentStore store, string email, string applicatName)
        {
            using (var session = store.OpenSession())
            {
                var q = from u in session.Query<User>()
                        where u.Email == email && u.ApplicationName == applicatName
                        select u.Username;
                return q.SingleOrDefault();
            }
        }

        public static bool IsUserInRole(IDocumentStore store, string username, string roleName, string applicationName)
        {
            using (var session = store.OpenSession())
            {
                var user = session.Query<User>()
                    .Where(u => u.Username == username && u.ApplicationName == applicationName)
                    .SingleOrDefault();
                if (user != null)
                {
                    var role = (from r in session.Query<Role>()
                                where r.Name == roleName && r.ApplicationName == applicationName
                                select r).SingleOrDefault();
                    if (role != null)
                    {
                        return user.Roles.Contains(role.Id);
                    }
                }
                return false;
            }
        }
    }
}
