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
    public class TestGeneratingCode
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
            int StaffID = 1;
            DateTime dt = DateTime.Now;
            string code = GetJoinedDateCode(StaffID, ref dt);
            SaveTextFile("GetJoinedDate.cs", code);
        }

        private static string GetJoinedDateCode(int StaffID, ref DateTime dt)
        {
            string code = null;
            try
            {
                string str = "CREATE PROCEDURE [dbo].[sp_GetJoinedDate](@StaffID int,@JoinedDate datetime output)";

                // Parse the stored procedure signature
                SPSignature signature = new SPSignature(str);

                SPCaller caller = new SPCaller(signature);
                caller.ConnectionStr = ConnectionStr;

                Output outputDT = new Output(dt);
                code = caller.GenDataSetProcCode("[dbo].[sp_GetJoinedDate]", StaffID, outputDT);

                dt = outputDT.GetDateTime();
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
            SaveTextFile("GetNum.cs", code);
        }

        private static string GetNumCode()
        {
            string code = null;
            try
            {
                string str = "CREATE PROCEDURE [dbo].[sp_GetNum] ";

                // Parse the stored procedure signature
                SPSignature signature = new SPSignature(str);

                SPCaller caller = new SPCaller(signature);
                caller.ConnectionStr = ConnectionStr;

                code = caller.GenIntProcCode("[dbo].[sp_GetNum]");
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
            int StaffID = 4;
            string code = GetChildrenCode(StaffID);
            SaveTextFile("GetChildren.cs", code);
        }

        private static string GetChildrenCode(int StaffID)
        {
            string code = null;
            try
            {
                string str = "CREATE PROCEDURE [dbo].[sp_GetChildren](@StaffID int )";

                // Parse the stored procedure signature
                SPSignature signature = new SPSignature(str);

                SPCaller caller = new SPCaller(signature);
                caller.ConnectionStr = ConnectionStr;

                code = caller.GenDataSetProcCode("[dbo].[sp_GetChildren]", StaffID);
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
            SaveTextFile("GetAllEmployee.cs", code);
        }

        private static string GetAllEmployeeCode()
        {
            string code = null;
            try
            {
                string str = "CREATE PROCEDURE [dbo].[sp_GetAllEmployee]";

                // Parse the stored procedure signature
                SPSignature signature = new SPSignature(str);

                SPCaller caller = new SPCaller(signature);
                caller.ConnectionStr = ConnectionStr;

                code = caller.GenDataSetProcCode("[dbo].[sp_GetAllEmployee]");
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
            int ID = -1;
            string code = InsertEmployeeCode(
                ref ID,
                "Jack Nillis",
                "Acct Manager",
                "Lilac garden",
                5000.0m,
                DateTime.Now,
                null);

            SaveTextFile("InsertEmployee.cs", code);
        }

        private static string InsertEmployeeCode(
            ref int ID,
            string Name,
            string Title,
            string Address,
            decimal Salary,
            DateTime JoinedDate,
            byte? Children)
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

                SPCaller caller = new SPCaller(signature);
                caller.ConnectionStr = ConnectionStr;

                Output outputID = new Output(ID);
                code = caller.GenVoidProcCode("[dbo].[sp_InsertEmp]",
                    outputID, Name, Title, Address, Salary, JoinedDate, Children);

                ID = outputID.GetInt();
            }
            catch (System.Exception ex)
            {
                Debug.Print(ex.Message);
            }
            return code;
        }
    }
}
