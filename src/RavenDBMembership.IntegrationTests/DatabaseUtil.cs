using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RavenDBMembership.IntegrationTests.Properties;
using RavenDBMembership.IntegrationTests.ProviderFixtures;

namespace RavenDBMembership.IntegrationTests
{
    class DatabaseUtil
    {
        public static void CreateTestDatabase(string databaseName)
        {
            string tempPath = Settings.Default.AccessibleTempPath;
            string databaseMdfPath = Path.Combine(tempPath, databaseName + @"\DatabaseFile.mdf");

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            DatabaseInitialization.DetachDatabase(databaseName);
            DatabaseInitialization.RecreateDatabase(databaseName, databaseMdfPath);
            DatabaseInitialization.RunSqlMembershipCreationScript(databaseName);
        }
    }
}
