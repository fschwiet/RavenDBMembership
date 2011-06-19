﻿using System.IO;
using System.Linq;
using NJasmine;
using NUnit.Framework;
using RavenDBMembership.IntegrationTests.ProviderFixtures;

namespace RavenDBMembership.IntegrationTests
{
    public class can_load_database : GivenWhenThenFixture
    {
        public override void Specify()
        {
            then("the database scripts can be loaded", delegate
            {
                var createScripts = DatabaseInitialization.GetCreateScript();

                Assert.That(createScripts.Any(s => s.Contains("TABLE")), Is.True);
                Assert.That(createScripts.Count(), Is.GreaterThan(5));
            });

            given("a path for the MDF file", delegate
            {
                var databaseMdfFilepath = arrange(delegate
                {
                    string cTemp = Properties.Settings.Default.AccessibleTempPath;

                    var databaseTempDirectory = Path.Combine(cTemp, "RavenDBMembershipTest");

                    if (!Directory.Exists(databaseTempDirectory))
                        Directory.CreateDirectory(databaseTempDirectory);

                    return Path.Combine(databaseTempDirectory, @"TestSqlMembership.mdf");
                });

                then("the database can be generated", delegate
                {
                    string databaseName = FixtureConstants.SqlMembershipProviderDatabaseName;

                    DatabaseInitialization.RecreateDatabase(databaseName, databaseMdfFilepath);

                    DatabaseInitialization.RunSqlMembershipCreationScript(databaseName);
                });
            });
        }
    }
}
