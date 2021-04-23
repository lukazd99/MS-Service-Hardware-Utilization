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
        
        public static void AddData(UtilizationValues utilizationValues, List<HardwareType> hardwareTypes)
        {
            using(IDbConnection connection = new SQLiteConnection())
            {
                connection.ConnectionString = loadConnectionString();

                connection.Open();

                SQLiteCommand cmd = (SQLiteCommand)connection.CreateCommand();
                
                cmd.CommandText = "INSERT INTO UtilizationValues (CPU, DISK, MEMORY)" +
                    $"VALUES ('{utilizationValues.CPU}'," +
                    $"'{utilizationValues.Disk}'," +
                    $"'{utilizationValues.Memory}');";

                int valueId = cmd.ExecuteNonQuery();

                int hardwareTypeId = 1;

                foreach (HardwareType device in hardwareTypes)
                {
                    cmd.CommandText = "INSERT INTO HardwareTypes (Model, AdditionalInfo)" +
                    $"VALUES ('{device.Model}','{device.AdditionalInfo}');";
                    cmd.ExecuteNonQuery();
                }

                if (hardwareTypeId == -1)
                    throw new Exception("Unable to get an Id for the HardwareType object.");

                Record record = new Record(hardwareTypeId, valueId, DateTime.Now);

                cmd.CommandText = "INSERT INTO Records (HardwareType,Value,CreateDate)" +
                    $"VALUES ('{record.HardwareType}','{record.Value}','{record.CreateDate}');";

                cmd.ExecuteNonQuery();
                    
            }
        }

        private static string loadConnectionString(string id = "NCR_ConnString") 
            => ConfigurationManager.ConnectionStrings[id].ConnectionString;
    }
}
