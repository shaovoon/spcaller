using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace TestProject
{
    [TestClass]
    public class TestGeneratedCode
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

        static DataSet GetJoinedDate(int StaffID, ref DateTime JoinedDate)
        {
            DataSet ds = new DataSet();
            SqlConnection connection = new SqlConnection(ConnectionStr);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand("[dbo].[sp_GetJoinedDate]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter parameter = null;
                parameter = new SqlParameter("@StaffID", SqlDbType.Int);
                parameter.Value = StaffID;
                command.Parameters.Add(parameter);

                parameter = new SqlParameter("@JoinedDate", SqlDbType.DateTime);
                parameter.Direction = System.Data.ParameterDirection.Output;
                parameter.Value = JoinedDate;
                command.Parameters.Add(parameter);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);

                JoinedDate = Convert.ToDateTime(command.Parameters["@JoinedDate"].Value);

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                connection.Close();
            }
            return ds;

        }

        [TestMethod]
        public void TestGetNum()
        {
            int n = GetNum();
            Debug.Print("Number returned is {0}", n);
        }

        static int GetNum()
        {
            int RetValue = -1;
            SqlConnection connection = new SqlConnection(ConnectionStr);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand("[dbo].[sp_GetNum]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter parameterRet = new SqlParameter("@RetValue254165", SqlDbType.Int);
                parameterRet.Direction = System.Data.ParameterDirection.ReturnValue;
                parameterRet.Value = -1;
                command.Parameters.Add(parameterRet);

                command.ExecuteNonQuery();

                RetValue = Convert.ToInt32(command.Parameters["@RetValue254165"].Value);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                connection.Close();
            }
            return RetValue;

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

        static DataSet GetChildren(int StaffID)
        {
            DataSet ds = new DataSet();
            SqlConnection connection = new SqlConnection(ConnectionStr);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand("[dbo].[sp_GetChildren]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter parameter = null;
                parameter = new SqlParameter("@StaffID", SqlDbType.Int);
                parameter.Value = StaffID;
                command.Parameters.Add(parameter);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                connection.Close();
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

        static DataSet GetAllEmployee()
        {
            DataSet ds = new DataSet();
            SqlConnection connection = new SqlConnection(ConnectionStr);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand("[dbo].[sp_GetAllEmployee]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                connection.Close();
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

        static void InsertEmployee(
            ref int ID,
            string Name,
            string Title,
            string Address,
            decimal Salary,
            DateTime JoinedDate,
            byte? Children)
        {
            SqlConnection connection = new SqlConnection(ConnectionStr);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand("[dbo].[sp_InsertEmp]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter parameter = null;
                parameter = new SqlParameter("@ID", SqlDbType.Int);
                parameter.Direction = System.Data.ParameterDirection.Output;
                parameter.Value = ID;
                command.Parameters.Add(parameter);

                parameter = new SqlParameter("@Name", SqlDbType.NVarChar, 30);
                if (Name == null)
                    parameter.Value = DBNull.Value;
                else
                    parameter.Value = Name;
                command.Parameters.Add(parameter);

                parameter = new SqlParameter("@Title", SqlDbType.VarChar, 20);
                if (Title == null)
                    parameter.Value = DBNull.Value;
                else
                    parameter.Value = Title;
                command.Parameters.Add(parameter);

                parameter = new SqlParameter("@Address", SqlDbType.VarChar, 30);
                if (Address == null)
                    parameter.Value = DBNull.Value;
                else
                    parameter.Value = Address;
                command.Parameters.Add(parameter);

                parameter = new SqlParameter("@Salary", SqlDbType.Money);
                parameter.Value = Salary;
                command.Parameters.Add(parameter);

                parameter = new SqlParameter("@JoinedDate", SqlDbType.DateTime);
                parameter.Value = JoinedDate;
                command.Parameters.Add(parameter);

                parameter = new SqlParameter("@Children", SqlDbType.TinyInt);
                if (Children == null)
                    parameter.Value = DBNull.Value;
                else
                    parameter.Value = Children;
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();

                ID = Convert.ToInt32(command.Parameters["@ID"].Value);

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                connection.Close();
            }
            return;
        }

    }
}
