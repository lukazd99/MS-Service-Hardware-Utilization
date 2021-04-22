using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardawreUtilizationService
{
    public static class SqliteDataAccess
    {
        public static void AddData(HardwareType hardwareType, Record record)
        {
            using(IDbConnection connection = new SQLiteConnection())
            {
                connection.ConnectionString = loadConnectionString();

            }
        }

        private static string loadConnectionString(string id = "NCR") 
            => ConfigurationManager.ConnectionStrings[id].ConnectionString;
    }
}
