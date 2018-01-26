using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TablesExporter
{
    public class DataSaver
    {
        private string _dir;
        public DataSaver(string savingDirectory)
        {
            _dir = savingDirectory.TrimEnd('\\');
        }

        public void Save(string tableName, string tableStr)
        {
            var filePath = $"{_dir}\\{tableName}.dat";
            try
            {
                File.AppendAllText(filePath, tableStr);
            }
            catch (IOException ex)
            {
                Console.WriteLine("Failed to save file '{0}'. {1}", filePath, ex.Message);
            }
            catch (Exception ex0)
            {
                Console.WriteLine("Failed to save file '{0}'. {1}", filePath, ex0.Message);
            }
        }
    }
}
