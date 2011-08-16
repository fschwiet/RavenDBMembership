using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using Raven.Client.Embedded;

namespace RavenDBMembership.IntegrationTests
{
    public class can_create_administration_bypassing_provider_bullocks : GivenWhenThenFixture
    {
        public override void Specify()
        {
            var applicationName = "testApp";
            var roleName = "Administrators";
            var store = arrange(() => new EmbeddableDocumentStore() { RunInMemory = true });
            arrange(() => store.Initialize());

            it("has no Administrator initially", delegate
            {
                var result = DataAccess.GetUsersInRole(store, roleName, applicationName);
                expect(() => result == null || result.Count() == 0);
            });

            when("an administrator is created in that role", delegate {
                arrange(delegate
                {
                    var roleId = DataAccess.CreateRole(store, roleName, applicationName);
                    DataAccess.CreateUserInRaven(store, applicationName, "admin", "password", "email@server.com", DateTime.Now, new[] { roleId });
                });

                then("the administrator is in the administrators role", delegate
                {
                    var result = DataAccess.GetUsersInRole(store, roleName, applicationName);
                    expect(() => result.Single() == "admin");
                });

                then("the administrator has that email", delegate
                {
                    var result = DataAccess.GetUserNameByEmail(store, "email@server.com", applicationName);

                    expect(() => result == "admin");
                });
            }); 
        }
    }
}
