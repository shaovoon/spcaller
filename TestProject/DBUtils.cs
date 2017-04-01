using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProject
{
    class DBUtils
    {
        public static string ConnectionStr;
        public const string SavedFolder = @"C:\temp";

        static DBUtils()
        {
            ConnectionStr = "Data Source=WONG-HP\\MSSQL2008R2;Initial Catalog=Testing;User Id=sa;Password=password;";
        }
    }
}
