﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace RavenDBMembership.IntegrationTests
{
    public class can_load_database : GivenWhenThenFixture
    {
        public override void Specify()
        {

            var databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                            @"SqlMembershipDatabase\DatabaseFile.mdf");

            var secondPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SqlMembershipDatabase\Lol.mdf");

            arrange(delegate
            {
                if (Directory.Exists(secondPath))
                {
                    Directory.Delete(secondPath, true);
                }
            });

            then("the database can be reached", delegate
            {
                string connectionString = GetConnectionStringForMdfFile("TestDatabase", databasePath);

                var connection = arrange(() => new SqlConnection(connectionString));

                connection.Open();
                cleanup(delegate
                {
                    connection.Close();
                });

                var command = arrange(() => new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", connection));

                command.ExecuteNonQuery();
            });

            then("the database scripts can be loaded", delegate
            {
                string createScript = GetCreateScript();

                Assert.That(createScript, Is.StringContaining("TABLE"));
            });

            then("the database can be generated", delegate
            {
                DetachDatabase("SqlMembership");

                using (var masterConnection = new SqlConnection(GetConnectionStringForMaster()))
                {
                    masterConnection.Open();
                    try
                    {
                        using (var createCommand = new SqlCommand(@"
USE Master
CREATE DATABASE SqlMembership ON (NAME=SqlMembershipFile1, FILENAME= '$filename')
".Replace("$filename", secondPath), masterConnection))
                        {
                            createCommand.ExecuteNonQuery();
                        }
                    }
                    finally 
                    {
                        masterConnection.Close();
                    }
                }
                
                string createScript = GetCreateScript();

                string connectionString = GetConnectionStringFor("SqlMembership"); //GetConnectionStringForMdfFile("SqlMembership", secondPath); 

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    try
                    {
                        using (var command = new SqlCommand(createScript, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    finally
                    {
                        connection.Close();
                    };
                }
            });
        }

        string GetCreateScript()
        {
            return new StreamReader(
                this.GetType().Assembly.GetManifestResourceStream(
                    "RavenDBMembership.IntegrationTests.SqlMembershipDatabase.Create.sql")).ReadToEnd();
        }

        string GetConnectionStringForMaster()
        {
            return GetConnectionStringFor("master");
        }

        string GetConnectionStringFor(string databaseName)
        {
            return @"Database='" + databaseName + @"';Data Source=.\SQLEXPRESS;Integrated Security=True";
        }

        string GetConnectionStringForMdfFile(string databaseName, string databasePath)
        {
            var connectionString =
                @"Database='$name';Data Source=.\SQLEXPRESS;AttachDbFileName='$path';Integrated Security=True;User Instance=True";

            connectionString = connectionString.Replace("$name", databaseName).Replace("$path", databasePath);

            return connectionString;
        }

        public void DetachDatabase(string databaseName)
        {
            using (var connection = new SqlConnection(GetConnectionStringForMaster()))
            {
                connection.Open();

                try
                {
                    string detachScript = @"
USE [master]
IF db_id('$databaseName') IS NOT NULL
BEGIN
    --EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'$databaseName'
    --ALTER DATABASE $databaseName SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    --EXEC sp_detach_db [$databaseName];
    DROP DATABASE [$databaseName]
END
".Replace("$databaseName", databaseName);

                    using (var command = new SqlCommand(detachScript, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
            
        }
    }
}