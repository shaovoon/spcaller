using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace StoredProcedureCaller
{
    public class SPCaller
    {
        public ISignature Signature { get; set; }
        public string ConnectionStr { get; set; }

        public SPCaller()
        {
            Signature = null;
            ConnectionStr = null;
        }
        public SPCaller(ISignature signature)
        {
            Signature = signature;
            ConnectionStr = null;
        }

        public DataSet CallDataSetProc(string spname, params object[] param)
        {
            if (string.IsNullOrEmpty(spname))
            {
                string msg = "spname is empty!";
                throw new InvalidDataException(msg);
            }
            if (Signature.Columns.Count != param.Length)
            {
                string msg = string.Format("Signature.Columns.Count({0}) is not equal to param.Length({1})",
                    Signature.Columns.Count, param.Length);
                throw new InvalidDataException(msg);
            }
            if (Signature.Name.ToLower() != spname.ToLower())
            {
                string msg = "Signature.Name is not equal to spname";
                throw new InvalidDataException(msg);
            }
            DataSet ds = new DataSet();
            SqlConnection connection = new SqlConnection(ConnectionStr);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand(spname, connection);
                command.CommandType = CommandType.StoredProcedure;

                List<Column> listInputOutput = new List<Column>();
                for (int i = 0; i < Signature.Columns.Count; ++i)
                {
                    object obj = param[i];
                    Column col = Signature.Columns[i];
                    if (col.direction == ParameterDirection.Input && obj != null && obj.GetType() != col.netType)
                    {
                        string msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})", 
                            obj.GetType().ToString(), col.netType.ToString());
                        throw new InvalidDataException(msg);
                    }
                    else if (col.direction == ParameterDirection.InputOutput)
                    {
                        InputOutput inputoutput = new InputOutput();
                        if(obj.GetType() != inputoutput.GetType())
                        {
                            string msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                                obj.GetType().ToString(), inputoutput.GetType().ToString());
                            throw new InvalidDataException(msg);
                        }
                        col.inputOuput = (InputOutput)(obj);
                        listInputOutput.Add(col);
                    }
                    else if (col.direction == ParameterDirection.Output)
                    {
                        Output output = new Output();
                        if (obj.GetType() != output.GetType())
                        {
                            string msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                                obj.GetType().ToString(), output.GetType().ToString());
                            throw new InvalidDataException(msg);
                        }
                        col.inputOuput = (InputOutput)(obj);
                        listInputOutput.Add(col);
                    }

                    SqlParameter parameter = null;
                    if(col.length==0)
                        parameter = new SqlParameter(col.parameterName, col.sqlType);
                    else
                        parameter = new SqlParameter(col.parameterName, col.sqlType, (int)(col.length));

                    if (obj == null)
                        parameter.Value = DBNull.Value;
                    else
                    {
                        if (col.direction == ParameterDirection.Input)
                            parameter.Value = obj;
                        else
                        {
                            parameter.Direction = col.direction;
                            parameter.Value = col.inputOuput.Value;
                        }
                    }
                    command.Parameters.Add(parameter);

                }

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);

                // assign back all the InputOutput values
                foreach (Column col in listInputOutput)
                {
                    col.inputOuput.Value = command.Parameters[col.parameterName].Value;
                }
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
        public int CallIntProc(string spname, params object[] param)
        {
            if (string.IsNullOrEmpty(spname))
            {
                string msg = "spname is empty!";
                throw new InvalidDataException(msg);
            }
            if (Signature.Columns.Count != param.Length)
            {
                string msg = string.Format("Signature.Columns.Count({0}) is not equal to param.Length({1})",
                    Signature.Columns.Count, param.Length);
                throw new InvalidDataException(msg);
            }
            if (Signature.Name.ToLower() != spname.ToLower())
            {
                string msg = "Signature.Name is not equal to spname";
                throw new InvalidDataException(msg);
            }
            int RetValue = 0;
            const string RetValueName = "@ReturnValue569824";
            SqlConnection connection = new SqlConnection(ConnectionStr);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand(spname, connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter parameterRet = new SqlParameter(RetValueName, SqlDbType.Int);
                parameterRet.Direction = System.Data.ParameterDirection.ReturnValue;
                parameterRet.Value = RetValue;
                command.Parameters.Add(parameterRet);

                List<Column> listInputOutput = new List<Column>();
                for (int i = 0; i < Signature.Columns.Count; ++i)
                {
                    object obj = param[i];
                    Column col = Signature.Columns[i];
                    if (col.direction == ParameterDirection.Input && obj != null && obj.GetType() != col.netType)
                    {
                        string msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                            obj.GetType().ToString(), col.netType.ToString());
                        throw new InvalidDataException(msg);
                    }
                    else if (col.direction == ParameterDirection.InputOutput)
                    {
                        InputOutput inputoutput = new InputOutput();
                        if (obj.GetType() != inputoutput.GetType())
                        {
                            string msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                                obj.GetType().ToString(), inputoutput.GetType().ToString());
                            throw new InvalidDataException(msg);
                        }
                        col.inputOuput = (InputOutput)(obj);
                        listInputOutput.Add(col);
                    }
                    else if (col.direction == ParameterDirection.Output)
                    {
                        Output output = new Output();
                        if (obj.GetType() != output.GetType())
                        {
                            string msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                                obj.GetType().ToString(), output.GetType().ToString());
                            throw new InvalidDataException(msg);
                        }
                        col.inputOuput = (InputOutput)(obj);
                        listInputOutput.Add(col);
                    }

                    SqlParameter parameter = null;
                    if (col.length == 0)
                        parameter = new SqlParameter(col.parameterName, col.sqlType);
                    else
                        parameter = new SqlParameter(col.parameterName, col.sqlType, (int)(col.length));

                    if (obj == null)
                        parameter.Value = DBNull.Value;
                    else
                    {
                        if (col.direction == ParameterDirection.Input)
                            parameter.Value = obj;
                        else
                        {
                            parameter.Direction = col.direction;
                            parameter.Value = col.inputOuput.Value;
                        }
                    }
                    command.Parameters.Add(parameter);
                }

                command.ExecuteNonQuery();

                RetValue = Convert.ToInt32(command.Parameters[RetValueName].Value);

                // assign back all the InputOutput values
                foreach (Column col in listInputOutput)
                {
                    col.inputOuput.Value = command.Parameters[col.parameterName].Value;
                }
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
        public void CallVoidProc(string spname, params object[] param)
        {
            if(string.IsNullOrEmpty(spname))
            {
                string msg = "spname is empty!";
                throw new InvalidDataException(msg);
            }
            if (Signature.Columns.Count != param.Length)
            {
                string msg = string.Format("Signature.Columns.Count({0}) is not equal to param.Length({1})",
                    Signature.Columns.Count, param.Length);
                throw new InvalidDataException(msg);
            }
            if (Signature.Name.ToLower() != spname.ToLower())
            {
                string msg = "Signature.Name is not equal to spname";
                throw new InvalidDataException(msg);
            }

            SqlConnection connection = new SqlConnection(ConnectionStr);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand(spname, connection);
                command.CommandType = CommandType.StoredProcedure;

                List<Column> listInputOutput = new List<Column>();
                for (int i = 0; i < Signature.Columns.Count; ++i)
                {
                    object obj = param[i];
                    Column col = Signature.Columns[i];
                    if (col.direction == ParameterDirection.Input && obj!=null && obj.GetType() != col.netType)
                    {
                        string msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                            obj.GetType().ToString(), col.netType.ToString());
                        throw new InvalidDataException(msg);
                    }
                    else if (col.direction == ParameterDirection.InputOutput)
                    {
                        InputOutput inputoutput = new InputOutput();
                        if (obj.GetType() != inputoutput.GetType())
                        {
                            string msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                                obj.GetType().ToString(), inputoutput.GetType().ToString());
                            throw new InvalidDataException(msg);
                        }
                        col.inputOuput = (InputOutput)(obj);
                        listInputOutput.Add(col);
                    }
                    else if (col.direction == ParameterDirection.Output)
                    {
                        Output output = new Output();
                        if (obj.GetType() != output.GetType())
                        {
                            string msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                                obj.GetType().ToString(), output.GetType().ToString());
                            throw new InvalidDataException(msg);
                        }
                        col.inputOuput = (InputOutput)(obj);
                        listInputOutput.Add(col);
                    }

                    SqlParameter parameter = null;
                    if (col.length == 0)
                        parameter = new SqlParameter(col.parameterName, col.sqlType);
                    else
                        parameter = new SqlParameter(col.parameterName, col.sqlType, (int)(col.length));

                    if (obj == null)
                        parameter.Value = DBNull.Value;
                    else
                    {
                        if (col.direction == ParameterDirection.Input)
                            parameter.Value = obj;
                        else
                        {
                            parameter.Direction = col.direction;
                            parameter.Value = col.inputOuput.Value;
                        }
                    }
                    command.Parameters.Add(parameter);
                }

                command.ExecuteNonQuery();

                // assign back all the InputOutput values
                foreach (Column col in listInputOutput)
                {
                    col.inputOuput.Value = command.Parameters[col.parameterName].Value;
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                connection.Close();
            }
        }
        public string GenDataSetProcCode(string spname, params object[] param)
        {
            string msg = string.Empty;
            if(string.IsNullOrEmpty(spname))
            {
                msg = "spname is empty!";
                throw new InvalidDataException(msg);
            }
            if (Signature.Columns.Count != param.Length)
            {
                msg = string.Format("Signature.Columns.Count({0}) is not equal to param.Length({1})",
                    Signature.Columns.Count, param.Length);
                throw new InvalidDataException(msg);
            }
            if (Signature.Name.ToLower() != spname.ToLower())
            {
                msg = "Signature.Name is not equal to spname";
                throw new InvalidDataException(msg);
            }

            StringBuilder strBuilder = new StringBuilder(20000);

            strBuilder.Append("    DataSet ds = new DataSet();\n");
	        strBuilder.Append("    SqlConnection connection = new SqlConnection(ConnectionStr);\n");
	        strBuilder.Append("    try\n");
            strBuilder.Append("    {\n");
            strBuilder.Append("        connection.Open();\n\n");

		    msg = "        SqlCommand command = new SqlCommand(\""+Signature.Name+ "\", connection);\n";
            strBuilder.Append(msg);
		    strBuilder.Append("        command.CommandType = CommandType.StoredProcedure;\n\n");

            if (Signature.Columns.Count > 0)
                strBuilder.Append("        SqlParameter parameter = null;\n");

            List<Column> listInputOutput = new List<Column>();
            for(int i=0; i<Signature.Columns.Count;++i)
            {
                object obj = param[i];
                Column col = Signature.Columns[i];
                if (col.direction == ParameterDirection.Input && obj != null && obj.GetType() != col.netType)
                {
                    msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                        obj.GetType().ToString(), col.netType.ToString());
                    throw new InvalidDataException(msg);
                }
                else if (col.direction == ParameterDirection.InputOutput)
                {
                    InputOutput inputoutput = new InputOutput();
                    if (obj.GetType() != inputoutput.GetType())
                    {
                        msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                            obj.GetType().ToString(), inputoutput.GetType().ToString());
                        throw new InvalidDataException(msg);
                    }
                    col.inputOuput = (InputOutput)(obj);
                    listInputOutput.Add(col);
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    Output output = new Output();
                    if (obj.GetType() != output.GetType())
                    {
                        msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                            obj.GetType().ToString(), output.GetType().ToString());
                        throw new InvalidDataException(msg);
                    }
                    col.inputOuput = (InputOutput)(obj);
                    listInputOutput.Add(col);
                }

                if(col.length==0)
                {
                    msg = "        parameter = new SqlParameter(\""+col.parameterName
                        +"\", SqlDbType."+col.sqlType.ToString()+");\n";
                }
                else
                {
                    msg = "        parameter = new SqlParameter(\""+col.parameterName
                        +"\", SqlDbType."+col.sqlType.ToString()+", "+col.length+");\n";
                }
                strBuilder.Append(msg);

                if(col.direction==ParameterDirection.InputOutput)
                {
                    strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.InputOutput;\n");
                }
                else if(col.direction==ParameterDirection.Output)
                {
                    strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.Output;\n");
                }

                if(obj==null||col.netType==System.Type.GetType("System.Byte[]")
                    ||col.netType==System.Type.GetType("System.String"))
                {
                    strBuilder.Append("        if ("+col.name+" == null)\n");
			        strBuilder.Append("            parameter.Value = DBNull.Value;\n");
		            strBuilder.Append("        else\n");
			        strBuilder.Append("            parameter.Value = "+col.name+";\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }
                else
                {
			        strBuilder.Append("        parameter.Value = "+col.name+";\n");
		            strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }

            }

	        strBuilder.Append("        SqlDataAdapter adapter = new SqlDataAdapter(command);\n");
	        strBuilder.Append("        adapter.Fill(ds);\n\n");

            // assign back all the InputOutput values
            foreach (Column col in listInputOutput)
            {
                if (col.netType.ToString().IndexOf("Byte[]") == -1) // not a byte array type
                {
                    string sType = col.netType.ToString().Substring(7);
                    msg = "        " + col.name + " = Convert.To" + sType
                        + "(command.Parameters[\"" + col.parameterName + "\"].Value);\n\n";
                }
                else // cast to byte array
                {
                    msg = "        " + col.name + " = (byte[])"
                        + "(command.Parameters[\"" + col.parameterName + "\"].Value);\n\n";
                }
                strBuilder.Append(msg);
            }


	        strBuilder.Append("    }\n");
	        strBuilder.Append("    catch (Exception exp)\n");
		    strBuilder.Append("    {\n");
			strBuilder.Append("        throw exp;\n");
		    strBuilder.Append("    }\n");
		    strBuilder.Append("    finally\n");
		    strBuilder.Append("    {\n");
            strBuilder.Append("        if(connection != null)\n");
            strBuilder.Append("            connection.Close();\n");
            strBuilder.Append("    }\n");
		    strBuilder.Append("    return ds;\n");

            return strBuilder.ToString();
        }
        public string GenIntProcCode(string spname, params object[] param)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(spname))
            {
                msg = "spname is empty!";
                throw new InvalidDataException(msg);
            }
            if (Signature.Columns.Count != param.Length)
            {
                msg = string.Format("Signature.Columns.Count({0}) is not equal to param.Length({1})",
                    Signature.Columns.Count, param.Length);
                throw new InvalidDataException(msg);
            }
            if (Signature.Name.ToLower() != spname.ToLower())
            {
                msg = "Signature.Name is not equal to spname";
                throw new InvalidDataException(msg);
            }

            StringBuilder strBuilder = new StringBuilder(20000);

            strBuilder.Append("    int RetValue = -1;\n");
            strBuilder.Append("    SqlConnection connection = new SqlConnection(ConnectionStr);\n");
            strBuilder.Append("    try\n");
            strBuilder.Append("    {\n");
            strBuilder.Append("        connection.Open();\n\n");

            msg = "        SqlCommand command = new SqlCommand(\"" + Signature.Name + "\", connection);\n";
            strBuilder.Append(msg);
            strBuilder.Append("        command.CommandType = CommandType.StoredProcedure;\n\n");


            // set the return value parameter
            const string RetValueName = "@RetValue254165";
            msg = "        SqlParameter parameterRet = new SqlParameter(\"" + RetValueName
                        + "\", SqlDbType.Int);\n";
            strBuilder.Append(msg);
            strBuilder.Append("        parameterRet.Direction = System.Data.ParameterDirection.ReturnValue;\n");
            strBuilder.Append("        parameterRet.Value = -1;\n");
            strBuilder.Append("        command.Parameters.Add(parameterRet);\n\n");

            if (Signature.Columns.Count > 0)
                strBuilder.Append("        SqlParameter parameter = null;\n");

            List<Column> listInputOutput = new List<Column>();
            for (int i = 0; i < Signature.Columns.Count; ++i)
            {
                object obj = param[i];
                Column col = Signature.Columns[i];
                if (col.direction == ParameterDirection.Input && obj != null && obj.GetType() != col.netType)
                {
                    msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                        obj.GetType().ToString(), col.netType.ToString());
                    throw new InvalidDataException(msg);
                }
                else if (col.direction == ParameterDirection.InputOutput)
                {
                    InputOutput inputoutput = new InputOutput();
                    if (obj.GetType() != inputoutput.GetType())
                    {
                        msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                            obj.GetType().ToString(), inputoutput.GetType().ToString());
                        throw new InvalidDataException(msg);
                    }
                    col.inputOuput = (InputOutput)(obj);
                    listInputOutput.Add(col);
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    Output output = new Output();
                    if (obj.GetType() != output.GetType())
                    {
                        msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                            obj.GetType().ToString(), output.GetType().ToString());
                        throw new InvalidDataException(msg);
                    }
                    col.inputOuput = (InputOutput)(obj);
                    listInputOutput.Add(col);
                }

                if (col.length == 0)
                {
                    msg = "        parameter = new SqlParameter(\"" + col.parameterName
                        + "\", SqlDbType." + col.sqlType.ToString() + ");\n";
                }
                else
                {
                    msg = "        parameter = new SqlParameter(\"" + col.parameterName
                        + "\", SqlDbType." + col.sqlType.ToString() + ", " + col.length + ");\n";
                }
                strBuilder.Append(msg);

                if (col.direction == ParameterDirection.InputOutput)
                {
                    strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.InputOutput;\n");
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.Output;\n");
                }

                if (obj == null || col.netType == System.Type.GetType("System.Byte[]")
                    || col.netType == System.Type.GetType("System.String"))
                {
                    strBuilder.Append("        if (" + col.name + " == null)\n");
                    strBuilder.Append("            parameter.Value = DBNull.Value;\n");
                    strBuilder.Append("        else\n");
                    strBuilder.Append("            parameter.Value = " + col.name + ";\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }
                else
                {
                    strBuilder.Append("        parameter.Value = " + col.name + ";\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }

            }

            strBuilder.Append("        command.ExecuteNonQuery();\n\n");

            msg = "        RetValue = Convert.ToInt32(command.Parameters[\"" + RetValueName + "\"].Value);\n";
            strBuilder.Append(msg);

            // assign back all the InputOutput values
            foreach (Column col in listInputOutput)
            {
                if (col.netType.ToString().IndexOf("Byte[]") == -1) // not a byte array type
                {
                    string sType = col.netType.ToString().Substring(7);
                    msg = "        " + col.name + " = Convert.To" + sType
                        + "(command.Parameters[\"" + col.parameterName + "\"].Value);\n\n";
                }
                else // cast to byte array
                {
                    msg = "        " + col.name + " = (byte[])"
                        + "(command.Parameters[\"" + col.parameterName + "\"].Value);\n\n";
                }
                strBuilder.Append(msg);
            }


            strBuilder.Append("    }\n");
            strBuilder.Append("    catch (Exception exp)\n");
            strBuilder.Append("    {\n");
            strBuilder.Append("        throw exp;\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    finally\n");
            strBuilder.Append("    {\n");
            strBuilder.Append("        if(connection != null)\n");
            strBuilder.Append("            connection.Close();\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    return RetValue;\n");

            return strBuilder.ToString();
        }
        public string GenVoidProcCode(string spname, params object[] param)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(spname))
            {
                msg = "spname is empty!";
                throw new InvalidDataException(msg);
            }
            if (Signature.Columns.Count != param.Length)
            {
                msg = string.Format("Signature.Columns.Count({0}) is not equal to param.Length({1})",
                    Signature.Columns.Count, param.Length);
                throw new InvalidDataException(msg);
            }
            if (Signature.Name.ToLower() != spname.ToLower())
            {
                msg = "Signature.Name is not equal to spname";
                throw new InvalidDataException(msg);
            }

            StringBuilder strBuilder = new StringBuilder(20000);

            strBuilder.Append("    SqlConnection connection = new SqlConnection(ConnectionStr);\n");
            strBuilder.Append("    try\n");
            strBuilder.Append("    {\n");
            strBuilder.Append("        connection.Open();\n\n");

            msg = "        SqlCommand command = new SqlCommand(\"" + Signature.Name + "\", connection);\n";
            strBuilder.Append(msg);
            strBuilder.Append("        command.CommandType = CommandType.StoredProcedure;\n\n");

            if (Signature.Columns.Count>0)
                strBuilder.Append("        SqlParameter parameter = null;\n");

            List<Column> listInputOutput = new List<Column>();
            for (int i = 0; i < Signature.Columns.Count; ++i)
            {
                object obj = param[i];
                Column col = Signature.Columns[i];
                if (col.direction == ParameterDirection.Input && obj != null && obj.GetType() != col.netType)
                {
                    msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                        obj.GetType().ToString(), col.netType.ToString());
                    throw new InvalidDataException(msg);
                }
                else if (col.direction == ParameterDirection.InputOutput)
                {
                    InputOutput inputoutput = new InputOutput();
                    if (obj.GetType() != inputoutput.GetType())
                    {
                        msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                            obj.GetType().ToString(), inputoutput.GetType().ToString());
                        throw new InvalidDataException(msg);
                    }
                    col.inputOuput = (InputOutput)(obj);
                    listInputOutput.Add(col);
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    Output output = new Output();
                    if (obj.GetType() != output.GetType())
                    {
                        msg = string.Format("Type mismatch: Inputted type({0}) do not match with internal type({1})",
                            obj.GetType().ToString(), output.GetType().ToString());
                        throw new InvalidDataException(msg);
                    }
                    col.inputOuput = (InputOutput)(obj);
                    listInputOutput.Add(col);
                }

                if (col.length == 0)
                {
                    msg = "        parameter = new SqlParameter(\"" + col.parameterName
                        + "\", SqlDbType." + col.sqlType.ToString() + ");\n";
                }
                else
                {
                    msg = "        parameter = new SqlParameter(\"" + col.parameterName
                        + "\", SqlDbType." + col.sqlType.ToString() + ", " + col.length + ");\n";
                }
                strBuilder.Append(msg);

                if (col.direction == ParameterDirection.InputOutput)
                {
                    strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.InputOutput;\n");
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.Output;\n");
                }

                if (obj == null || col.netType == System.Type.GetType("System.Byte[]")
                    || col.netType == System.Type.GetType("System.String"))
                {
                    strBuilder.Append("        if (" + col.name + " == null)\n");
                    strBuilder.Append("            parameter.Value = DBNull.Value;\n");
                    strBuilder.Append("        else\n");
                    strBuilder.Append("            parameter.Value = " + col.name + ";\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }
                else
                {
                    strBuilder.Append("        parameter.Value = " + col.name + ";\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }

            }

            strBuilder.Append("        command.ExecuteNonQuery();\n\n");

            // assign back all the InputOutput values
            foreach (Column col in listInputOutput)
            {
                if (col.netType.ToString().IndexOf("Byte[]") == -1) // not a byte array type
                {
                    string sType = col.netType.ToString().Substring(7);
                    msg = "        " + col.name + " = Convert.To" + sType
                        + "(command.Parameters[\"" + col.parameterName + "\"].Value);\n\n";
                }
                else // cast to byte array
                {
                    msg = "        " + col.name + " = (byte[])"
                        + "(command.Parameters[\"" + col.parameterName + "\"].Value);\n\n";
                }
                strBuilder.Append(msg);
            }


            strBuilder.Append("    }\n");
            strBuilder.Append("    catch (Exception exp)\n");
            strBuilder.Append("    {\n");
            strBuilder.Append("        throw exp;\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    finally\n");
            strBuilder.Append("    {\n");
            strBuilder.Append("        if(connection != null)\n");
            strBuilder.Append("            connection.Close();\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    return;\n");

            return strBuilder.ToString();
        }

    }
}
