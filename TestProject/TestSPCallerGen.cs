using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StoredProcedureCaller;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;

namespace TestProject
{
    [TestClass]
    public class TestSPCallerGen
    {
        public static string ConnectionStr;

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ConnectionStr = DBUtils.ConnectionStr;
        }

        private static void SaveTextFile(string file, string text)
        {
            string path = Path.Combine(DBUtils.SavedFolder, file);
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                FileStream stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
                StreamWriter sw = new StreamWriter(stream);

                //Write a line of text
                sw.WriteLine(text);

                //Close the file
                sw.Close();
            }
            catch (Exception e)
            {
                Debug.Print("Exception: " + e.Message);
            }
            finally
            {
                Debug.Print("Executing finally block.");
            }
        }

        [TestMethod]
        public void TestGetJoinedDate()
        {
            string code = GetJoinedDateCode();
            SaveTextFile("NewGetJoinedDate.cs", code);
        }

        private static string GetJoinedDateCode()
        {
            string code = null;
            try
            {
                string str = "CREATE PROCEDURE [dbo].[sp_GetJoinedDate](@StaffID int,@JoinedDate datetime output)";

                // Parse the stored procedure signature
                SPSignature signature = new SPSignature(str);

                code = SPCallerGen.GenCode(signature, "GetJoinedDate", SPCallerGen.ReturnType.Tables, null, null, false);
            }
            catch (System.Exception ex)
            {
                Debug.Print(ex.Message);
            }
            return code;
        }

        [TestMethod]
        public void TestGetNum()
        {
            string code = GetNumCode();
            SaveTextFile("NewGetNum.cs", code);
        }

        private static string GetNumCode()
        {
            string code = null;
            try
            {
                string str = "CREATE PROCEDURE [dbo].[sp_GetNum] ";

                // Parse the stored procedure signature
                SPSignature signature = new SPSignature(str);

                code = SPCallerGen.GenCode(signature, "GetNum", SPCallerGen.ReturnType.Integer, null, null, false);
            }
            catch (System.Exception ex)
            {
                Debug.Print(ex.Message);
            }
            return code;
        }

        [TestMethod]
        public void TestGetChildren()
        {
            string code = GetChildrenCode();
            SaveTextFile("NewGetChildren.cs", code);
        }

        private static string GetChildrenCode()
        {
            string code = null;
            try
            {
                string str = "CREATE PROCEDURE [dbo].[sp_GetChildren](@StaffID int )";

                // Parse the stored procedure signature
                SPSignature signature = new SPSignature(str);

                code = SPCallerGen.GenCode(signature, "GetChildren", SPCallerGen.ReturnType.Tables, null, null, false);
            }
            catch (System.Exception ex)
            {
                Debug.Print(ex.Message);
            }
            return code;
        }

        [TestMethod]
        public void TestGetAllEmployee()
        {
            string code = GetAllEmployeeCode();
            SaveTextFile("NewGetAllEmployee.cs", code);
        }

        private static string GetAllEmployeeCode()
        {
            string code = null;
            try
            {
                string str = "CREATE PROCEDURE [dbo].[sp_GetAllEmployee]";

                // Parse the stored procedure signature
                SPSignature signature = new SPSignature(str);

                code = SPCallerGen.GenCode(signature, "GetAllEmployee", SPCallerGen.ReturnType.Tables, null, null, false);
            }
            catch (System.Exception ex)
            {
                Debug.Print(ex.Message);
            }
            return code;
        }

        [TestMethod]
        public void TestInsertEmployee()
        {
            string code = InsertEmployeeCode();

            SaveTextFile("NewInsertEmployee.cs", code);
        }

        private static string InsertEmployeeCode()
        {
            string code = null;
            try
            {
                string str = "CREATE PROCEDURE [dbo].[sp_InsertEmp](";
                str += "@ID int OUTPUT," + "@Name nvarchar(30),";
                str += "@Title varchar(20)," + "@Address varchar(30),";
                str += "@Salary money," + "@JoinedDate datetime,";
                str += "@Children tinyint)";

                // Parse the stored procedure signature
                SPSignature signature = new SPSignature(str);

                code = SPCallerGen.GenCode(signature, "InsertEmployee", SPCallerGen.ReturnType.None, null, null, false);
            }
            catch (System.Exception ex)
            {
                Debug.Print(ex.Message);
            }
            return code;
        }
    }
}
