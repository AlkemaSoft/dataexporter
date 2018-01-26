using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace TablesExporter
{
    class Program
    {
        private static DataSaver _saver = new DataSaver(AppDomain.CurrentDomain.BaseDirectory);
        static void Main(string[] args)
        {
            var connectionStr = System.Configuration.ConfigurationManager.ConnectionStrings["ExportingBase"].ConnectionString;
            using (var con = new SqlConnection(connectionStr))
            {
                con.Open();

                var reader = new DataReader(con,_saver);

                var tableNames = System.Configuration.ConfigurationManager.AppSettings["ExportingTables"];
                List<string> tableNamesList = null;

                if (!string.IsNullOrWhiteSpace(tableNames))
                {
                    tableNamesList = tableNames.Split(',').Select(x => x.Trim()).ToList();
                }
                
                ExecuteTables(reader, tableNamesList);
            }
        }

        static void ExecuteTables(DataReader reader, List<string> tableNames)
        {
            List<string> executingTableNames    = tableNames;
            var dbTableNames                    = reader.GetTableNames();
            if (tableNames == null || tableNames.Count == 0)
            {
                executingTableNames = dbTableNames;
            }
            else
            {
                var failedTableNames    = executingTableNames.Where(x => dbTableNames.All(y => y.ToUpper() != x.ToUpper()));
                executingTableNames     = executingTableNames.Where(x => failedTableNames.All(y => y.ToUpper() != x.ToUpper())).ToList();

                foreach (var failedTable in failedTableNames)
                {
                    Console.WriteLine("Incorrect table name '{0}'. Failed to find it on database.", failedTable);
                }
            }
            foreach (var tableName in executingTableNames)
            {
                reader.WriteTable(tableName);
                Console.WriteLine("Table '{0}' exported.", tableName);
            }
        }
    }
}
