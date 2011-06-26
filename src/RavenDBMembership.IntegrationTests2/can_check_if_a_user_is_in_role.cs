using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using NJasmine;

namespace RavenDBMembership.IntegrationTests2
{
    public class can_check_if_a_user_is_in_role : GivenWhenThenFixture
    {
        static int index = 0;

        public override void Specify()
        {
            given("a user", delegate
            {
                var roleName = "usernameForRoleTest" + index++;

                var user = arrange(() => Membership.CreateUser(roleName, "password"));
                 
                given("roles foo and bar", delegate
                {
                    arrange(() => Roles.CreateRole("foo"));
                    arrange(() => Roles.CreateRole("bar"));

                    when("user is in group foo", delegate
                    {
                        arrange(() => Roles.AddUserToRole(roleName, "foo"));
                        
                        then("the roles provider indicates user is in role foo", delegate
                        {
                            expect(() => Roles.IsUserInRole(roleName, "foo"));
                        });

                        then("the roles provider indicates user is notin role bar", delegate
                        {
                            expect(() => !Roles.IsUserInRole(roleName, "bar"));
                        });
                    });
                });
            });
        }
    }
}
