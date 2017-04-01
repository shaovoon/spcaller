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
    public class TestNormalCode
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

        [TestMethod]
        public void TestGetJoinedDate()
        {
            int StaffID = 1;
            DateTime dt = DateTime.Now;
            DataSet ds = GetJoinedDate(StaffID, ref dt);
            Debug.Print("Employee with StaffID({0}) joined on {1}", StaffID, dt);
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Debug.Print("Employee with StaffID({0}) joined on {1}", StaffID, dt);
                }
            }
        }

        private static DataSet GetJoinedDate(int StaffID, ref DateTime dt)
        {
            DataSet ds = null;
            try
            {
                string str = "CREATE PROCEDURE [dbo].[sp_GetJoinedDate](@StaffID int,@JoinedDate datetime output)";

                // Parse the stored procedure signature
                SPSignature signature = new SPSignature(str);

                SPCaller caller = new SPCaller(signature);
                caller.ConnectionStr = ConnectionStr;

                Output outputDT = new Output(dt);
                ds = caller.CallDataSetProc("[dbo].[sp_GetJoinedDate]", StaffID, outputDT);

                dt = outputDT.GetDateTime();
            }
            catch (System.Exception ex)
            {
                Debug.Print(ex.Message);
            }
            return ds;
        }

        [TestMethod]
        public void TestGetNum()
        {
            int n = GetNum();
            Debug.Print("Number returned is {0}", n);
        }

        private static int GetNum()
        {
            int n = -1;
            try
            {
                string str = "CREATE PROCEDURE [dbo].[sp_GetNum] ";

                // Parse the stored procedure signature
                SPSignature signature = new SPSignature(str);

                SPCaller caller = new SPCaller(signature);
                caller.ConnectionStr = ConnectionStr;

                n = caller.CallIntProc("[dbo].[sp_GetNum]");
            }
            catch (System.Exception ex)
            {
                Debug.Print(ex.Message);
            }
            return n;
        }

        [TestMethod]
        public void TestGetChildren()
        {
            int StaffID = 4;
            DataSet ds = GetChildren(StaffID);
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Debug.Print("Employee with StaffID({0}) has {1} children", StaffID, row[0].ToString());
                }
            }
        }

        private static DataSet GetChildren(int StaffID)
        {
            DataSet ds = null;
            try
            {
                string str = "CREATE PROCEDURE [dbo].[sp_GetChildren](@StaffID int )";

                // Parse the stored procedure signature
                SPSignature signature = new SPSignature(str);

                SPCaller caller = new SPCaller(signature);
                caller.ConnectionStr = ConnectionStr;

                ds = caller.CallDataSetProc("[dbo].[sp_GetChildren]", StaffID);
            }
            catch (System.Exception ex)
            {
                Debug.Print(ex.Message);
            }
            return ds;
        }


        [TestMethod]
        public void TestGetAllEmployee()
        {
            DataSet ds = GetAllEmployee();
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Debug.Print(row[0].ToString() + ", ");
                    Debug.Print(row[1].ToString() + ", ");
                    Debug.Print(row[2].ToString() + ", ");
                    Debug.Print(row[3].ToString() + ", ");
                    Debug.Print(row[4].ToString() + ", ");
                    Debug.Print(row[5].ToString() + ", ");
                    Debug.Print(row[6].ToString() + "\n");
                }
            }
        }

        private static DataSet GetAllEmployee()
        {
            DataSet ds = null;
            try
            {
                string str = "CREATE PROCEDURE [dbo].[sp_GetAllEmployee]";

                // Parse the stored procedure signature
                SPSignature signature = new SPSignature(str);

                SPCaller caller = new SPCaller(signature);
                caller.ConnectionStr = ConnectionStr;

                ds = caller.CallDataSetProc("[dbo].[sp_GetAllEmployee]");
            }
            catch (System.Exception ex)
            {
                Debug.Print(ex.Message);
            }
            return ds;
        }

        [TestMethod]
        public void TestInsertEmployee()
        {
            int ID = -1;
            InsertEmployee(
                ref ID,
                "Jack Nillis",
                "Acct Manager",
                "Lilac garden",
                5000.0m,
                DateTime.Now,
                null);

            Debug.Print("New Employee ID:{0}", ID);
        }

        private static void InsertEmployee(
            ref int ID,
            string Name,
            string Title,
            string Address,
            decimal Salary,
            DateTime JoinedDate,
            byte? Children)
        {
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
                caller.CallVoidProc("[dbo].[sp_InsertEmp]",
                    outputID, Name, Title, Address, Salary, JoinedDate, Children);

                ID = outputID.GetInt();
            }
            catch (System.Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }
    }
}
