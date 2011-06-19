using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Security;
using NUnit.Framework;

namespace RavenDBMembership.IntegrationTests.ProviderFixtures
{
    public class OverrideForSqlMembershipProvider : MembershipProviderOverride
    {
        public override MembershipProvider GetProvider()
        {
            string tempPath = Properties.Settings.Default.AccessibleTempPath;
            string databaseMdfPath = Path.Combine(tempPath, @"SqlMembershipProviderTestDatabase\DatabaseFile.mdf");

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            DatabaseInitialization.DetachDatabase(FixtureConstants.SqlMembershipProviderDatabaseName);
            DatabaseInitialization.RecreateDatabase(FixtureConstants.SqlMembershipProviderDatabaseName, databaseMdfPath);
            DatabaseInitialization.RunSqlMembershipCreationScript(FixtureConstants.SqlMembershipProviderDatabaseName);

            var result = new SqlMembershipProvider();

            return result;
        }

        public override bool IsSqlMembershipProvider()
        {
            return true;
        }

        public override void PostInitializeUpdate(MembershipProvider provider)
        {
            var connectionStringProperty = typeof(SqlMembershipProvider).GetField("_sqlConnectionString",
                                                                                   BindingFlags.NonPublic |
                                                                                   BindingFlags.Instance);

            Assert.That(connectionStringProperty, Is.Not.Null);

            connectionStringProperty.SetValue(provider, DatabaseInitialization.GetConnectionStringFor(FixtureConstants.SqlMembershipProviderDatabaseName));
        }
    }
}