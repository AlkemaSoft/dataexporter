using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TablesExporter
{
    public class DataReader
    {
        private SqlConnection _connection;
        private string _lineSeparator = Environment.NewLine;
        public DataReader(SqlConnection connection)
        {
            _connection = connection;
        }

        public string GetTableStr(string tableName)
        {
            var resultStr = new StringBuilder(1200);

            var columns         = GetTableColumns(tableName);
            var columnNames     = columns.Select(x => x.Item1).ToList();
            var columnNamesStr  = string.Join("|", columnNames);
            resultStr.Append(columnNamesStr);
            resultStr.Append(_lineSeparator);

            var columnDataTypes     = columns.Select(x => x.Item2).ToList();
            var columnDataTypesStr  = string.Join("|", columnDataTypes);
            resultStr.Append(columnDataTypesStr);
            resultStr.Append(_lineSeparator);

            var tableRowsStr    = GetTableDataStr(tableName, columns);
            resultStr.Append(tableRowsStr);

            return resultStr.ToString();
        }

        public List<string> GetTableNames()
        {
            string sqlExpression    = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ";
            SqlCommand command      = new SqlCommand(sqlExpression, _connection);
            SqlDataReader reader    = command.ExecuteReader();

            var names = new List<string>(5);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string name = reader.GetValue(0).ToString();

                    names.Add(name);
                }
            }

            reader.Close();

            return names;
        }

        private List<Tuple<string, string>> GetTableColumns(string tableName)
        {
            string sqlExpression = "select COLUMN_NAME, DATA_TYPE FROM information_schema.columns WHERE TABLE_NAME='" + tableName + "'";
            SqlCommand command = new SqlCommand(sqlExpression, _connection);
            SqlDataReader reader = command.ExecuteReader();

            var columns = new List<Tuple<string, string>>(5);
            if (reader.HasRows) 
            {
                while (reader.Read())
                {
                    string name     = reader.GetValue(0).ToString();
                    string dataType = reader.GetValue(1).ToString();

                    columns.Add(new Tuple<string, string>(name, dataType));
                }
            }

            reader.Close();

            return columns;
        }

        private string GetTableDataStr(string tableName, List<Tuple<string, string>> columns)
        {
            var columnNames         = columns.Select(x => x.Item1).ToList();
            var columnNamesStr      = string.Join(",", columnNames);
            string sqlExpression    = string.Format("SELECT {0} FROM {1}", columnNamesStr, tableName);
            SqlCommand command      = new SqlCommand(sqlExpression, _connection);
            SqlDataReader reader    = command.ExecuteReader();

            var resultStr = new StringBuilder(120);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    for (int i = 0; i < columns.Count; i++)
                    {
                        var valueStr = reader.GetValue(i).ToString();
                        resultStr.Append(valueStr);
                        if (i < columns.Count - 1)
                        {
                            resultStr.Append('|');
                        }

                    }
                    resultStr.Append(_lineSeparator);
                }
            }

            reader.Close();

            return resultStr.ToString();
        }
    }
}
