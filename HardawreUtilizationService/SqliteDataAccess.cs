using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace HardwareUtilizationService
{
    public static class SqliteDataAccess
    {
        /// <summary>
        /// Static function used for inserting and updating data into the SQLite local database.
        /// </summary>
        /// <param name="utilizationValues">CPU, Memory and DISK utilization values.</param>
        /// <param name="hardwareTypes">Generic list of hardware devices found.</param>
        public static void AddData(UtilizationValues utilizationValues, List<HardwareType> hardwareTypes)
        {
            // Declaring a secure connection using the using statement.
            // Which guarantees that the connection will be disposed.
            using(IDbConnection connection = new SQLiteConnection())
            {
                try
                {
                    Console.WriteLine("Connecting to database...");
                    connection.ConnectionString = loadConnectionString();

                    connection.Open();

                    if (connection.State == ConnectionState.Open)
                    {
                        Console.WriteLine("Connection established!");
                    }
                    else
                    {
                        throw new SQLiteException("Unable to establish connection to database.");
                    }

                    SQLiteCommand cmd = (SQLiteCommand)connection.CreateCommand();

                    // Inserting the utilizationValues object values into the database
                    cmd.CommandText = "INSERT INTO UtilizationValues (CPU, DISK, MEMORY)" +
                        $"VALUES ('{utilizationValues.CPU}'," +
                        $"'{utilizationValues.Disk}'," +
                        $"'{utilizationValues.Memory}');";

                    cmd.ExecuteNonQuery();

                    // Retrieveing the id of the row just inserted for referencing later.
                    cmd.CommandText = "select last_insert_rowid()";
                    Int64 ID64 = (Int64)cmd.ExecuteScalar();
                    int UtilizationValueId = (int)ID64;

                    foreach (HardwareType device in hardwareTypes)
                    {
                        // If unable to change the default value of this ID, the application will throw an exception.
                        int hardwareTypeId = -1;

                        // Checking (and retrieving) the hardwareId from the database.
                        // If there is no match, the device is inserted as a new device to the database.
                        cmd.CommandText = "SELECT Id FROM HardwareTypes WHERE AdditionalInfo = '" + device.AdditionalInfo + "'";
                        var Scalar = cmd.ExecuteScalar();

                        if (Scalar != null)
                        {
                            // HardwareId found, updating the row.

                            Console.WriteLine("Updating data to database... (Please wait)");
                            ID64 = (Int64)cmd.ExecuteScalar();
                            hardwareTypeId = (int)ID64;

                            cmd.CommandText = $"UPDATE HardwareTypes SET Model='{device.Model}', AdditionalInfo= '{device.AdditionalInfo}' " +
                            $" WHERE Id={hardwareTypeId}";
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // HardwareId not found, inserting row as new device.

                            Console.WriteLine("Adding data to database... (Please wait)");
                           
                            cmd.CommandText = "INSERT INTO HardwareTypes (Model, AdditionalInfo)" +
                            $"VALUES ('{device.Model}','{device.AdditionalInfo}');";
                            cmd.ExecuteNonQuery();

                            // Retrieving the hardwareId of the newly added device.
                            cmd.CommandText = "select last_insert_rowid()";
                            ID64 = (Int64)cmd.ExecuteScalar();
                            hardwareTypeId = (int)ID64;
                        }

                        if (hardwareTypeId == -1)
                            throw new Exception("Unable to get an Id for the HardwareType object.");


                        // Adding new record row
                        Record record = new Record(hardwareTypeId, UtilizationValueId, DateTime.Now);
                        
                        cmd.CommandText = "INSERT INTO Records (HardwareType,Value,CreateDate)" +
                            $"VALUES ('{record.HardwareType}','{record.Value}','{record.CreateDate}');";

                        cmd.ExecuteNonQuery();

                    }
                    connection.Close();

                    Console.WriteLine("Data entered successfully!\n");
                }
                catch (SQLiteException ex)
                {
                    
                    Console.WriteLine(ex.StackTrace);

                }
            }
        }

        // Method to retrieve the connection string from the configuration file.
        private static string loadConnectionString(string id = "NCR_ConnString") 
            => ConfigurationManager.ConnectionStrings[id].ConnectionString;
    }
}
