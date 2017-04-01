using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StoredProcedureCaller;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace TestProject
{
    [TestClass]
    public class TestSignatureSaveLoad
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

        public static void TestInsertEmployee()
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

        public static void InsertEmployee(
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

                string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string dir = System.IO.Path.GetDirectoryName(location);
                string xmlPath = System.IO.Path.Combine(dir, "signature.xml");

                signature.Save(xmlPath);
                SPSignature signature2 = new SPSignature();
                signature2.Load(xmlPath);

                SPCaller caller = new SPCaller(signature2);
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
