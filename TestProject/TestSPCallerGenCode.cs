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
    public class TestSPCallerGenCode
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

        public DataSet GetJoinedDate(
            int? StaffID,
            ref DateTime JoinedDate)
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
                if (StaffID == null)
                    parameter.Value = DBNull.Value;
                else
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

        public int GetNum()
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

        public DataSet GetChildren(
            int? StaffID)
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
                if (StaffID == null)
                    parameter.Value = DBNull.Value;
                else
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

        public DataSet GetAllEmployee()
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

        public void InsertEmployee(
            ref int ID,
            string Name,
            string Title,
            string Address,
            decimal? Salary,
            DateTime? JoinedDate,
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
                if (Salary == null)
                    parameter.Value = DBNull.Value;
                else
                    parameter.Value = Salary;
                command.Parameters.Add(parameter);

                parameter = new SqlParameter("@JoinedDate", SqlDbType.DateTime);
                if (JoinedDate == null)
                    parameter.Value = DBNull.Value;
                else
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

        [TestMethod]
        public void TestInsertManyEmployees()
        {
            List<EmpType> list = new List<EmpType>();

            EmpType emp = null;

            emp = new EmpType();
            emp.Address = "Canada";
            emp.Children = null;
            emp.JoinedDate = DateTime.Now;
            emp.Name = "Mary";
            emp.Salary = 3600.0m;
            emp.Title = "Secretary";
            list.Add(emp);

            emp = new EmpType();
            emp.Address = "USA";
            emp.Children = 3;
            emp.JoinedDate = DateTime.Now;
            emp.Name = "John";
            emp.Salary = 2500.0m;
            emp.Title = "Senior Technician";
            list.Add(emp);

            int RowsInserted = -1;
            InsertManyEmployees(
                ref RowsInserted,
                list);

            Debug.Print("RowsInserted:{0}", RowsInserted);
        }

        public class EmpType
        {
            // Default Constructor
            public EmpType()
            {
                Name = null;
                Title = null;
                Address = null;
                Salary = 0m;
                JoinedDate = new DateTime();
                Children = null;
            }

            // Constructor
            public EmpType(
                string NameTemp,
                string TitleTemp,
                string AddressTemp,
                decimal SalaryTemp,
                DateTime JoinedDateTemp,
                byte? ChildrenTemp)
            {
                Name = NameTemp;
                Title = TitleTemp;
                Address = AddressTemp;
                Salary = SalaryTemp;
                JoinedDate = JoinedDateTemp;
                Children = ChildrenTemp;
            }

            public string Name;
            public string Title;
            public string Address;
            public decimal Salary;
            public DateTime JoinedDate;
            public byte? Children;
        }

        DataTable GetDataTable(List<EmpType> list)
        {
            if (list == null)
                return null;
            if (list.Count <= 0)
                return null;

            //Create DataTable and add Columns
            DataTable tbl = new DataTable();
            tbl.Columns.Add("Name", System.Type.GetType("System.String"));
            tbl.Columns.Add("Title", System.Type.GetType("System.String"));
            tbl.Columns.Add("Address", System.Type.GetType("System.String"));
            tbl.Columns.Add("Salary", System.Type.GetType("System.Decimal"));
            tbl.Columns.Add("JoinedDate", System.Type.GetType("System.DateTime"));
            tbl.Columns.Add("Children", System.Type.GetType("System.Byte"));

            foreach (EmpType obj in list)
            {
                DataRow newRow = tbl.NewRow();
                if (obj.Name == null)
                    newRow["Name"] = DBNull.Value;
                else
                    newRow["Name"] = obj.Name;

                if (obj.Title == null)
                    newRow["Title"] = DBNull.Value;
                else
                    newRow["Title"] = obj.Title;

                if (obj.Address == null)
                    newRow["Address"] = DBNull.Value;
                else
                    newRow["Address"] = obj.Address;

                newRow["Salary"] = obj.Salary;

                newRow["JoinedDate"] = obj.JoinedDate;

                if (obj.Children == null)
                    newRow["Children"] = DBNull.Value;
                else
                    newRow["Children"] = obj.Children;

                tbl.Rows.Add(newRow);
            }
            return tbl;
        }

        public void InsertManyEmployees(
            ref int RowsInserted,
            List<EmpType> Employees)
        {
            SqlConnection connection = new SqlConnection(ConnectionStr);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand("[dbo].[sp_InsertManyEmp]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter parameter = null;
                parameter = new SqlParameter("@RowsInserted", SqlDbType.Int);
                parameter.Direction = System.Data.ParameterDirection.Output;
                parameter.Value = RowsInserted;
                command.Parameters.Add(parameter);

                parameter = new SqlParameter("@Employees", SqlDbType.Structured);
                parameter.Value = GetDataTable(Employees);
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();

                RowsInserted = Convert.ToInt32(command.Parameters["@RowsInserted"].Value);

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
