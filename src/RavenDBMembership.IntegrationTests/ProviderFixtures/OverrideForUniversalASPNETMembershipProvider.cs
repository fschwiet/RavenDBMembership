using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Providers;
using System.Web.Security;

namespace RavenDBMembership.IntegrationTests.ProviderFixtures
{
    class OverrideForUniversalASPNETMembershipProvider : MembershipProviderOverride
    {
        public override MembershipProvider GetProvider()
        {
            DatabaseUtil.CreateTestDatabase(FixtureConstants.UniversalMembershipProviderDatabaseName);
            return new DefaultMembershipProvider();
        }

        public override void PostInitializeUpdate(MembershipProvider provider)
        {
            var connectionStringProperty = GetConnectionStringProperty();

            var connectionStringSettings = new ConnectionStringSettings()
            {
                ConnectionString = DatabaseInitialization.GetConnectionStringFor(FixtureConstants.UniversalMembershipProviderDatabaseName)
            };

            connectionStringProperty.SetValue(provider, connectionStringSettings, null);
            
            base.PostInitializeUpdate(provider);
        }

        public static PropertyInfo GetConnectionStringProperty()
        {
            return typeof(System.Web.Providers.DefaultMembershipProvider)
                .GetProperty("ConnectionString", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}
