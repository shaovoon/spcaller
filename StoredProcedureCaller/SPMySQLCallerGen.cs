using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace StoredProcedureCaller
{
    public class SPMySQLCallerGen
    {
        public enum ReturnType
        {
            Tables,
            Integer,
            None
        }

        public static string GenCode(ISignature signature, string spname, ReturnType retType,
            string additionalParam, string exceptionCode, bool withConnParam)
        {
            if (retType == ReturnType.Tables)
                return GenDataSetProcCode(signature, spname, additionalParam, exceptionCode, withConnParam);
            else if (retType == ReturnType.Integer)
                return GenIntProcCode(signature, spname, additionalParam, exceptionCode, withConnParam);
            else if (retType == ReturnType.None)
                return GenVoidProcCode(signature, spname, additionalParam, exceptionCode, withConnParam);

            return null;
        }

        public static string GenDataSetProcCode(ISignature signature, string spname,
            string additionalParam, string exceptionCode, bool withConnParam)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(spname))
            {
                msg = "spname is empty!";
                throw new InvalidDataException(msg);
            }

            StringBuilder strBuilder = new StringBuilder(20000);

            // write function declaration.
            strBuilder.Append("public DataSet " + spname + "(");
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.tableType)
                {
                    strBuilder.Append("\n    List<" + col.tableTypeStr + "> " + col.name);
                }
                else if (col.direction != ParameterDirection.Input) // output type.
                {
                    strBuilder.Append("\n    ref " + col.netTypeStr + " " + col.name);
                }
                else
                {
                    if (col.nullableType)
                        strBuilder.Append("\n    " + col.netTypeStr + "? " + col.name);
                    else
                        strBuilder.Append("\n    " + col.netTypeStr + " " + col.name);
                }
                if (i != (signature.Columns.Count - 1))
                    strBuilder.Append(",");
                else
                {
                    if (string.IsNullOrEmpty(additionalParam))
                    {
                        if (withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else
                            strBuilder.Append(")\n");

                    }
                    else
                    {
                        strBuilder.Append(",");

                        if (withConnParam)
                            strBuilder.Append("\n    MySqlConnection connection,\n");

                        additionalParam = additionalParam.Trim(' ', '\r', '\n', '\t');
                        if (additionalParam.Length > 0 && additionalParam[additionalParam.Length - 1] == ',')
                            additionalParam = additionalParam.Substring(0, additionalParam.Length - 1);
                        strBuilder.Append("\n    "+additionalParam + ")\n");
                    }
                }
            }
            if (signature.Columns.Count <= 0)
                strBuilder.Append(")\n");

            strBuilder.Append("{\n");

            strBuilder.Append("    DataSet ds = new DataSet();\n");
            if (withConnParam == false)
                strBuilder.Append("    MySqlConnection connection = new MySqlConnection(ConnectionStr);\n");
            strBuilder.Append("    MySqlCommand command = null;\n");

            strBuilder.Append("    try\n");
            strBuilder.Append("    {\n");
            strBuilder.Append("        connection.Open();\n\n");

            msg = "        command = new MySqlCommand(\"" + signature.Name + "\", connection);\n";
            strBuilder.Append(msg);
            strBuilder.Append("        command.CommandType = CommandType.StoredProcedure;\n\n");

            if (signature.Columns.Count > 0)
                strBuilder.Append("        MySqlParameter parameter = null;\n");

            List<Column> listInputOutput = new List<Column>();
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.direction == ParameterDirection.InputOutput)
                {
                    listInputOutput.Add(col);
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    listInputOutput.Add(col);
                }

                if (col.tableType == false)
                {
                    if (col.length == 0)
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ");\n";
                    }
                    else
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ", " + col.length + ");\n";
                    }
                }
                else
                {
                    msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                        + "\", MySqlDbType.Structured);\n";
                }
                strBuilder.Append(msg);

                if (col.tableType == false)
                {
                    if (col.direction == ParameterDirection.InputOutput)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.InputOutput;\n");
                    }
                    else if (col.direction == ParameterDirection.Output)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.Output;\n");
                    }
                }
                if (col.tableType)
                {
                    strBuilder.Append("        parameter.Value = GetDataTable(" + col.name + ");\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }
                else if ((col.nullableType && col.direction == ParameterDirection.Input) || col.netType == System.Type.GetType("System.Byte[]")
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

            strBuilder.Append("        MySqlDataAdapter adapter = new MySqlDataAdapter(command);\n");
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
            if (string.IsNullOrEmpty(exceptionCode))
                strBuilder.Append("        throw exp;\n");
            else
                strBuilder.Append("        " + exceptionCode + "\n");
            strBuilder.Append("    }\n");
            if (withConnParam == false)
            {
                strBuilder.Append("    finally\n");
                strBuilder.Append("    {\n");
                strBuilder.Append("        if(connection != null)\n");
                strBuilder.Append("            connection.Close();\n");
                strBuilder.Append("        if(command != null)\n");
                strBuilder.Append("            command.Dispose();\n");
                strBuilder.Append("    }\n");
            }
            strBuilder.Append("    return ds;\n");
            strBuilder.Append("}\n");

            return strBuilder.ToString();
        }
        public static string GenIntProcCode(ISignature signature, string spname,
            string additionalParam, string exceptionCode, bool withConnParam)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(spname))
            {
                msg = "spname is empty!";
                throw new InvalidDataException(msg);
            }

            StringBuilder strBuilder = new StringBuilder(20000);

            // write function declaration.
            strBuilder.Append("public int " + spname + "(");
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.tableType)
                {
                    strBuilder.Append("\n    List<" + col.tableTypeStr + "> " + col.name);
                }
                else if (col.direction != ParameterDirection.Input) // output type.
                {
                    strBuilder.Append("\n    ref " + col.netTypeStr + " " + col.name);
                }
                else
                {
                    if (col.nullableType)
                        strBuilder.Append("\n    " + col.netTypeStr + "? " + col.name);
                    else
                        strBuilder.Append("\n    " + col.netTypeStr + " " + col.name);
                }
                if (i != (signature.Columns.Count - 1))
                    strBuilder.Append(",");
                else
                {
                    if (string.IsNullOrEmpty(additionalParam))
                    {
                        if (withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else
                            strBuilder.Append(")\n");

                    }
                    else
                    {
                        strBuilder.Append(",");

                        if (withConnParam)
                            strBuilder.Append("\n    MySqlConnection connection,\n");

                        additionalParam = additionalParam.Trim(' ', '\r', '\n', '\t');
                        if (additionalParam.Length > 0 && additionalParam[additionalParam.Length - 1] == ',')
                            additionalParam = additionalParam.Substring(0, additionalParam.Length - 1);
                        strBuilder.Append("\n    " + additionalParam + ")\n");
                    }
                }
            }
            if (signature.Columns.Count <= 0)
                strBuilder.Append(")\n");

            strBuilder.Append("{\n");

            strBuilder.Append("    int RetValue = -1;\n");
            if (withConnParam==false)
                strBuilder.Append("    MySqlConnection connection = new MySqlConnection(ConnectionStr);\n");
            strBuilder.Append("    MySqlCommand command = null;\n");

            strBuilder.Append("    try\n");
            strBuilder.Append("    {\n");
            strBuilder.Append("        connection.Open();\n\n");

            msg = "        command = new MySqlCommand(\"" + signature.Name + "\", connection);\n";
            strBuilder.Append(msg);
            strBuilder.Append("        command.CommandType = CommandType.StoredProcedure;\n\n");


            // set the return value parameter
            const string RetValueName = "@RetValue254165";
            msg = "        MySqlParameter parameterRet = new MySqlParameter(\"" + RetValueName
                        + "\", MySqlDbType.Int32);\n";
            strBuilder.Append(msg);
            strBuilder.Append("        parameterRet.Direction = System.Data.ParameterDirection.ReturnValue;\n");
            strBuilder.Append("        parameterRet.Value = -1;\n");
            strBuilder.Append("        command.Parameters.Add(parameterRet);\n\n");

            if (signature.Columns.Count > 0)
                strBuilder.Append("        MySqlParameter parameter = null;\n");

            List<Column> listInputOutput = new List<Column>();
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.direction == ParameterDirection.InputOutput)
                {
                    listInputOutput.Add(col);
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    listInputOutput.Add(col);
                }

                if (col.tableType == false)
                {
                    if (col.length == 0)
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ");\n";
                    }
                    else
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ", " + col.length + ");\n";
                    }
                }
                else
                {
                    msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                        + "\", MySqlDbType.Structured);\n";
                }
                strBuilder.Append(msg);

                if (col.tableType == false)
                {
                    if (col.direction == ParameterDirection.InputOutput)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.InputOutput;\n");
                    }
                    else if (col.direction == ParameterDirection.Output)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.Output;\n");
                    }
                }
                if (col.tableType)
                {
                    strBuilder.Append("        parameter.Value = GetDataTable(" + col.name + ");\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }
                else if ((col.nullableType && col.direction == ParameterDirection.Input) || col.netType == System.Type.GetType("System.Byte[]")
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
            if (string.IsNullOrEmpty(exceptionCode))
                strBuilder.Append("        throw exp;\n");
            else
                strBuilder.Append("        " + exceptionCode + "\n");
            strBuilder.Append("    }\n");
            if (withConnParam == false)
            {
                strBuilder.Append("    finally\n");
                strBuilder.Append("    {\n");
                strBuilder.Append("        if(connection != null)\n");
                strBuilder.Append("            connection.Close();\n");
                strBuilder.Append("        if(command != null)\n");
                strBuilder.Append("            command.Dispose();\n");
                strBuilder.Append("    }\n");
            }
            strBuilder.Append("    return RetValue;\n");
            strBuilder.Append("}\n");

            return strBuilder.ToString();
        }
        public static string GenVoidProcCode(ISignature signature, string spname,
            string additionalParam, string exceptionCode, bool withConnParam)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(spname))
            {
                msg = "spname is empty!";
                throw new InvalidDataException(msg);
            }

            StringBuilder strBuilder = new StringBuilder(20000);

            // write function declaration.
            strBuilder.Append("public void " + spname + "(");
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.tableType)
                {
                    strBuilder.Append("\n    List<" + col.tableTypeStr + "> " + col.name);
                }
                else if (col.direction != ParameterDirection.Input) // output type.
                {
                    strBuilder.Append("\n    ref " + col.netTypeStr + " " + col.name);
                }
                else
                {
                    if (col.nullableType)
                        strBuilder.Append("\n    " + col.netTypeStr + "? " + col.name);
                    else
                        strBuilder.Append("\n    " + col.netTypeStr + " " + col.name);
                }
                if (i != (signature.Columns.Count - 1))
                    strBuilder.Append(",");
                else
                {
                    if (string.IsNullOrEmpty(additionalParam))
                    {
                        if (withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else
                            strBuilder.Append(")\n");

                    }
                    else
                    {
                        strBuilder.Append(",");

                        if (withConnParam)
                            strBuilder.Append("\n    MySqlConnection connection,\n");

                        additionalParam = additionalParam.Trim(' ', '\r', '\n', '\t');
                        if (additionalParam.Length > 0 && additionalParam[additionalParam.Length - 1] == ',')
                            additionalParam = additionalParam.Substring(0, additionalParam.Length - 1);
                        strBuilder.Append("\n    " + additionalParam + ")\n");
                    }
                }
            }
            if (signature.Columns.Count <= 0)
                strBuilder.Append(")\n");

            strBuilder.Append("{\n");

            if (withConnParam == false)
                strBuilder.Append("    MySqlConnection connection = new MySqlConnection(ConnectionStr);\n");
            strBuilder.Append("    MySqlCommand command = null;\n");

            strBuilder.Append("    try\n");
            strBuilder.Append("    {\n");
            strBuilder.Append("        connection.Open();\n\n");

            msg = "        command = new MySqlCommand(\"" + signature.Name + "\", connection);\n";
            strBuilder.Append(msg);
            strBuilder.Append("        command.CommandType = CommandType.StoredProcedure;\n\n");

            if (signature.Columns.Count > 0)
                strBuilder.Append("        MySqlParameter parameter = null;\n");

            List<Column> listInputOutput = new List<Column>();
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.direction == ParameterDirection.InputOutput)
                {
                    listInputOutput.Add(col);
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    listInputOutput.Add(col);
                }

                if (col.tableType == false)
                {
                    if (col.length == 0)
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ");\n";
                    }
                    else
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ", " + col.length + ");\n";
                    }
                }
                else
                {
                    msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                        + "\", MySqlDbType.Structured);\n";
                }
                strBuilder.Append(msg);

                if (col.tableType == false)
                {
                    if (col.direction == ParameterDirection.InputOutput)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.InputOutput;\n");
                    }
                    else if (col.direction == ParameterDirection.Output)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.Output;\n");
                    }
                }
                if (col.tableType)
                {
                    strBuilder.Append("        parameter.Value = GetDataTable(" + col.name + ");\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }
                else if ((col.nullableType && col.direction == ParameterDirection.Input) || col.netType == System.Type.GetType("System.Byte[]")
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
            if (string.IsNullOrEmpty(exceptionCode))
                strBuilder.Append("        throw exp;\n");
            else
                strBuilder.Append("        "+exceptionCode+"\n");

            strBuilder.Append("    }\n");
            if (withConnParam == false)
            {
                strBuilder.Append("    finally\n");
                strBuilder.Append("    {\n");
                strBuilder.Append("        if(connection != null)\n");
                strBuilder.Append("            connection.Close();\n");
                strBuilder.Append("        if(command != null)\n");
                strBuilder.Append("            command.Dispose();\n");
                strBuilder.Append("    }\n");
            }
            strBuilder.Append("    return;\n");
            strBuilder.Append("}\n");

            return strBuilder.ToString();
        }
        #region CRUD
        public static string GenSelectCode(ISignature signature, string spname,
            string additionalParam, string exceptionCode, bool withConnParam, bool withTransParam, bool transactional)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(spname))
            {
                msg = "Name is empty!";
                throw new InvalidDataException(msg);
            }

            StringBuilder strBuilder = new StringBuilder(20000);

            // write function declaration.
            strBuilder.Append("public DataSet " + spname + "(");
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.tableType)
                {
                    strBuilder.Append("\n    List<" + col.tableTypeStr + "> " + col.name);
                }
                else if (col.direction != ParameterDirection.Input) // output type.
                {
                    strBuilder.Append("\n    ref " + col.netTypeStr + " " + col.name);
                }
                else
                {
                    if (col.nullableType)
                        strBuilder.Append("\n    " + col.netTypeStr + "? " + col.name);
                    else
                        strBuilder.Append("\n    " + col.netTypeStr + " " + col.name);
                }
                if (i != (signature.Columns.Count - 1))
                    strBuilder.Append(",");
                else
                {
                    if (string.IsNullOrEmpty(additionalParam))
                    {
                        if (withTransParam && !withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    SqlTransaction transaction)\n");
                        }
                        else if (withConnParam && !withTransParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else if (withTransParam && withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    SqlTransaction transaction,");
                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else
                            strBuilder.Append(")\n");

                    }
                    else
                    {
                        strBuilder.Append(",");

                        if (withTransParam)
                            strBuilder.Append("\n    SqlTransaction transaction,");

                        if (withConnParam)
                            strBuilder.Append("\n    MySqlConnection connection,");

                        additionalParam = additionalParam.Trim(' ', '\r', '\n', '\t');
                        if (additionalParam.Length > 0 && additionalParam[additionalParam.Length - 1] == ',')
                            additionalParam = additionalParam.Substring(0, additionalParam.Length - 1);
                        strBuilder.Append("\n    " + additionalParam + ")\n");
                    }
                }
            }
            if (signature.Columns.Count <= 0)
                strBuilder.Append(")\n");

            strBuilder.Append("{\n");

            strBuilder.Append("    DataSet ds = new DataSet();\n");
            if (withConnParam == false)
                strBuilder.Append("    MySqlConnection connection = new MySqlConnection(ConnectionStr);\n");
            if (transactional && withTransParam == false)
                strBuilder.Append("    SqlTransaction transaction = null;\n");
            strBuilder.Append("    MySqlCommand command = null;\n");

            strBuilder.Append("    try\n");
            strBuilder.Append("    {\n");
            if (withConnParam == false)
                strBuilder.Append("        connection.Open();\n\n");

            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        transaction = connection.BeginTransaction();\n\n");
            }

            msg = "        command = new MySqlCommand(\"" + signature.Sql + "\", connection);\n";
            strBuilder.Append(msg);
            strBuilder.Append("        command.CommandType = CommandType.Text;\n\n");
            if (transactional)
                strBuilder.Append("        command.Transaction = transaction;\n\n");

            if (signature.Columns.Count > 0)
                strBuilder.Append("        MySqlParameter parameter = null;\n");

            List<Column> listInputOutput = new List<Column>();
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.direction == ParameterDirection.InputOutput)
                {
                    listInputOutput.Add(col);
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    listInputOutput.Add(col);
                }

                if (col.tableType == false)
                {
                    if (col.length == 0)
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ");\n";
                    }
                    else
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ", " + col.length + ");\n";
                    }
                }
                else
                {
                    msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                        + "\", MySqlDbType.Structured);\n";
                }
                strBuilder.Append(msg);

                if (col.tableType == false)
                {
                    if (col.direction == ParameterDirection.InputOutput)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.InputOutput;\n");
                    }
                    else if (col.direction == ParameterDirection.Output)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.Output;\n");
                    }
                }
                if (col.tableType)
                {
                    strBuilder.Append("        parameter.Value = GetDataTable(" + col.name + ");\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }
                else if ((col.nullableType && col.direction == ParameterDirection.Input) || col.netType == System.Type.GetType("System.Byte[]")
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

            strBuilder.Append("        MySqlDataAdapter adapter = new MySqlDataAdapter(command);\n");
            strBuilder.Append("        adapter.Fill(ds);\n\n");

            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        transaction.Commit();\n");
            }

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
            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        try\n");
                strBuilder.Append("        {\n");
                strBuilder.Append("            transaction.Rollback();\n");
                strBuilder.Append("        }\n");
                strBuilder.Append("        catch(Exception exp2)\n");
                strBuilder.Append("        {\n");
                if (string.IsNullOrEmpty(exceptionCode))
                    strBuilder.Append("            throw exp2;\n");
                else
                    strBuilder.Append("            " + exceptionCode + "\n");

                strBuilder.Append("        }\n");
            }
            if (string.IsNullOrEmpty(exceptionCode))
                strBuilder.Append("        throw exp;\n");
            else
                strBuilder.Append("        " + exceptionCode + "\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    finally\n");
            strBuilder.Append("    {\n");
            if (withConnParam == false)
            {
                strBuilder.Append("        if(connection != null)\n");
                strBuilder.Append("            connection.Close();\n");
            }
            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        if(transaction != null)\n");
                strBuilder.Append("            transaction.Dispose();\n");
            }
            strBuilder.Append("        if(command != null)\n");
            strBuilder.Append("            command.Dispose();\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    return ds;\n");
            strBuilder.Append("}\n");

            return strBuilder.ToString();
        }
        public static string GenSelectReaderCode(ISignature signature, string spname,
            string additionalParam, string exceptionCode, bool withConnParam, bool withTransParam, bool transactional)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(spname))
            {
                msg = "Name is empty!";
                throw new InvalidDataException(msg);
            }

            StringBuilder strBuilder = new StringBuilder(20000);

            // write function declaration.
            strBuilder.Append("public MySqlDataReader " + spname + "(");
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.tableType)
                {
                    strBuilder.Append("\n    List<" + col.tableTypeStr + "> " + col.name);
                }
                else if (col.direction != ParameterDirection.Input) // output type.
                {
                    strBuilder.Append("\n    ref " + col.netTypeStr + " " + col.name);
                }
                else
                {
                    if (col.nullableType)
                        strBuilder.Append("\n    " + col.netTypeStr + "? " + col.name);
                    else
                        strBuilder.Append("\n    " + col.netTypeStr + " " + col.name);
                }
                if (i != (signature.Columns.Count - 1))
                    strBuilder.Append(",");
                else
                {
                    if (string.IsNullOrEmpty(additionalParam))
                    {
                        if (withTransParam && !withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    SqlTransaction transaction)\n");
                        }
                        else if (withConnParam && !withTransParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else if (withTransParam && withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    SqlTransaction transaction,");
                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else
                            strBuilder.Append(")\n");

                    }
                    else
                    {
                        strBuilder.Append(",");

                        if (withTransParam)
                            strBuilder.Append("\n    SqlTransaction transaction,");

                        if (withConnParam)
                            strBuilder.Append("\n    MySqlConnection connection,");

                        additionalParam = additionalParam.Trim(' ', '\r', '\n', '\t');
                        if (additionalParam.Length > 0 && additionalParam[additionalParam.Length - 1] == ',')
                            additionalParam = additionalParam.Substring(0, additionalParam.Length - 1);
                        strBuilder.Append("\n    " + additionalParam + ")\n");
                    }
                }
            }
            if (signature.Columns.Count <= 0)
                strBuilder.Append(")\n");

            strBuilder.Append("{\n");

            strBuilder.Append("    MySqlDataReader reader = null;\n");

            if (withConnParam == false)
                strBuilder.Append("    MySqlConnection connection = new MySqlConnection(ConnectionStr);\n");
            if (transactional && withTransParam == false)
                strBuilder.Append("    SqlTransaction transaction = null;\n");
            strBuilder.Append("    MySqlCommand command = null;\n");

            strBuilder.Append("    try\n");
            strBuilder.Append("    {\n");
            if (withConnParam == false)
                strBuilder.Append("        connection.Open();\n\n");

            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        transaction = connection.BeginTransaction();\n\n");
            }

            msg = "        command = new MySqlCommand(\"" + signature.Sql + "\", connection);\n";
            strBuilder.Append(msg);
            strBuilder.Append("        command.CommandType = CommandType.Text;\n\n");
            if (transactional)
                strBuilder.Append("        command.Transaction = transaction;\n\n");

            if (signature.Columns.Count > 0)
                strBuilder.Append("        MySqlParameter parameter = null;\n");

            List<Column> listInputOutput = new List<Column>();
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.direction == ParameterDirection.InputOutput)
                {
                    listInputOutput.Add(col);
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    listInputOutput.Add(col);
                }

                if (col.tableType == false)
                {
                    if (col.length == 0)
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ");\n";
                    }
                    else
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ", " + col.length + ");\n";
                    }
                }
                else
                {
                    msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                        + "\", MySqlDbType.Structured);\n";
                }
                strBuilder.Append(msg);

                if (col.tableType == false)
                {
                    if (col.direction == ParameterDirection.InputOutput)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.InputOutput;\n");
                    }
                    else if (col.direction == ParameterDirection.Output)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.Output;\n");
                    }
                }
                if (col.tableType)
                {
                    strBuilder.Append("        parameter.Value = GetDataTable(" + col.name + ");\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }
                else if ((col.nullableType && col.direction == ParameterDirection.Input) || col.netType == System.Type.GetType("System.Byte[]")
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

            strBuilder.Append("        reader = command.ExecuteReader(CommandBehavior.CloseConnection);\n\n");

            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        transaction.Commit();\n");
            }

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
            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        try\n");
                strBuilder.Append("        {\n");
                strBuilder.Append("            transaction.Rollback();\n");
                strBuilder.Append("        }\n");
                strBuilder.Append("        catch(Exception exp2)\n");
                strBuilder.Append("        {\n");
                if (string.IsNullOrEmpty(exceptionCode))
                    strBuilder.Append("            throw exp2;\n");
                else
                    strBuilder.Append("            " + exceptionCode + "\n");

                strBuilder.Append("        }\n");
            }
            if (string.IsNullOrEmpty(exceptionCode))
                strBuilder.Append("        throw exp;\n");
            else
                strBuilder.Append("        " + exceptionCode + "\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    finally\n");
            strBuilder.Append("    {\n");
            // MySqlDataReader needs the connection to be opened
            //if (withConnParam == false)
            //{
            //    strBuilder.Append("        if(connection != null)\n");
            //    strBuilder.Append("            connection.Close();\n");
            //}
            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        if(transaction != null)\n");
                strBuilder.Append("            transaction.Dispose();\n");
            }
            strBuilder.Append("        if(command != null)\n");
            strBuilder.Append("            command.Dispose();\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    return reader;\n");
            strBuilder.Append("}\n");

            return strBuilder.ToString();
        }
        public static string GenUpdateCode(ISignature signature, string spname,
            string additionalParam, string exceptionCode, bool withConnParam, bool withTransParam, bool transactional)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(spname))
            {
                msg = "Name is empty!";
                throw new InvalidDataException(msg);
            }

            StringBuilder strBuilder = new StringBuilder(20000);

            // write function declaration.
            strBuilder.Append("public bool " + spname + "(");
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.tableType)
                {
                    strBuilder.Append("\n    List<" + col.tableTypeStr + "> " + col.name);
                }
                else if (col.direction != ParameterDirection.Input) // output type.
                {
                    strBuilder.Append("\n    ref " + col.netTypeStr + " " + col.name);
                }
                else
                {
                    if (col.nullableType)
                        strBuilder.Append("\n    " + col.netTypeStr + "? " + col.name);
                    else
                        strBuilder.Append("\n    " + col.netTypeStr + " " + col.name);
                }
                if (i != (signature.Columns.Count - 1))
                    strBuilder.Append(",");
                else
                {
                    if (string.IsNullOrEmpty(additionalParam))
                    {
                        if (withTransParam && !withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    SqlTransaction transaction)\n");
                        }
                        else if (withConnParam && !withTransParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else if (withTransParam && withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    SqlTransaction transaction,");
                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else
                            strBuilder.Append(")\n");

                    }
                    else
                    {
                        strBuilder.Append(",");

                        if (withTransParam)
                            strBuilder.Append("\n    SqlTransaction transaction,");

                        if (withConnParam)
                            strBuilder.Append("\n    MySqlConnection connection,");

                        additionalParam = additionalParam.Trim(' ', '\r', '\n', '\t');
                        if (additionalParam.Length > 0 && additionalParam[additionalParam.Length - 1] == ',')
                            additionalParam = additionalParam.Substring(0, additionalParam.Length - 1);
                        strBuilder.Append("\n    " + additionalParam + ")\n");
                    }
                }
            }
            if (signature.Columns.Count <= 0)
                strBuilder.Append(")\n");

            strBuilder.Append("{\n");

            strBuilder.Append("    bool RetValue = false;\n");
            if (withConnParam == false)
                strBuilder.Append("    MySqlConnection connection = new MySqlConnection(ConnectionStr);\n");
            if (transactional && withTransParam == false)
                strBuilder.Append("    SqlTransaction transaction = null;\n");
            strBuilder.Append("    MySqlCommand command = null;\n");
            strBuilder.Append("    try\n");
            strBuilder.Append("    {\n");
            if (withConnParam == false)
                strBuilder.Append("        connection.Open();\n\n");

            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        transaction = connection.BeginTransaction();\n\n");
            }
            msg = "        command = new MySqlCommand(\"" + signature.Sql + "\", connection);\n";
            strBuilder.Append(msg);
            strBuilder.Append("        command.CommandType = CommandType.Text;\n\n");
            if (transactional)
                strBuilder.Append("        command.Transaction = transaction;\n\n");


            if (signature.Columns.Count > 0)
                strBuilder.Append("        MySqlParameter parameter = null;\n");

            List<Column> listInputOutput = new List<Column>();
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.direction == ParameterDirection.InputOutput)
                {
                    listInputOutput.Add(col);
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    listInputOutput.Add(col);
                }

                if (col.tableType == false)
                {
                    if (col.length == 0)
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ");\n";
                    }
                    else
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ", " + col.length + ");\n";
                    }
                }
                else
                {
                    msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                        + "\", MySqlDbType.Structured);\n";
                }
                strBuilder.Append(msg);

                if (col.tableType == false)
                {
                    if (col.direction == ParameterDirection.InputOutput)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.InputOutput;\n");
                    }
                    else if (col.direction == ParameterDirection.Output)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.Output;\n");
                    }
                }
                if (col.tableType)
                {
                    strBuilder.Append("        parameter.Value = GetDataTable(" + col.name + ");\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }
                else if ((col.nullableType && col.direction == ParameterDirection.Input) || col.netType == System.Type.GetType("System.Byte[]")
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

            strBuilder.Append("        int affectedRows = command.ExecuteNonQuery();\n\n");

            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        transaction.Commit();\n");
            }

            msg = "        RetValue = affectedRows > 0;\n";
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
            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        try\n");
                strBuilder.Append("        {\n");
                strBuilder.Append("            transaction.Rollback();\n");
                strBuilder.Append("        }\n");
                strBuilder.Append("        catch(Exception exp2)\n");
                strBuilder.Append("        {\n");
                if (string.IsNullOrEmpty(exceptionCode))
                    strBuilder.Append("            throw exp2;\n");
                else
                    strBuilder.Append("            " + exceptionCode + "\n");

                strBuilder.Append("        }\n");
            }
            if (string.IsNullOrEmpty(exceptionCode))
                strBuilder.Append("        throw exp;\n");
            else
                strBuilder.Append("        " + exceptionCode + "\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    finally\n");
            strBuilder.Append("    {\n");
            if (withConnParam == false)
            {
                strBuilder.Append("        if(connection != null)\n");
                strBuilder.Append("            connection.Close();\n");
            }
            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        if(transaction != null)\n");
                strBuilder.Append("            transaction.Dispose();\n");
            }
            strBuilder.Append("        if(command != null)\n");
            strBuilder.Append("            command.Dispose();\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    return RetValue;\n");
            strBuilder.Append("}\n");

            return strBuilder.ToString();
        }

        public static string GenDeleteCode(ISignature signature, string spname,
            string additionalParam, string exceptionCode, bool withConnParam, bool withTransParam, bool transactional)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(spname))
            {
                msg = "Name is empty!";
                throw new InvalidDataException(msg);
            }

            StringBuilder strBuilder = new StringBuilder(20000);

            // write function declaration.
            strBuilder.Append("public bool " + spname + "(");
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.tableType)
                {
                    strBuilder.Append("\n    List<" + col.tableTypeStr + "> " + col.name);
                }
                else if (col.direction != ParameterDirection.Input) // output type.
                {
                    strBuilder.Append("\n    ref " + col.netTypeStr + " " + col.name);
                }
                else
                {
                    if (col.nullableType)
                        strBuilder.Append("\n    " + col.netTypeStr + "? " + col.name);
                    else
                        strBuilder.Append("\n    " + col.netTypeStr + " " + col.name);
                }
                if (i != (signature.Columns.Count - 1))
                    strBuilder.Append(",");
                else
                {
                    if (string.IsNullOrEmpty(additionalParam))
                    {
                        if (withTransParam && !withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    SqlTransaction transaction)\n");
                        }
                        else if (withConnParam && !withTransParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else if (withTransParam && withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    SqlTransaction transaction,");
                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else
                            strBuilder.Append(")\n");

                    }
                    else
                    {
                        strBuilder.Append(",");

                        if (withTransParam)
                            strBuilder.Append("\n    SqlTransaction transaction,");

                        if (withConnParam)
                            strBuilder.Append("\n    MySqlConnection connection,");

                        additionalParam = additionalParam.Trim(' ', '\r', '\n', '\t');
                        if (additionalParam.Length > 0 && additionalParam[additionalParam.Length - 1] == ',')
                            additionalParam = additionalParam.Substring(0, additionalParam.Length - 1);
                        strBuilder.Append("\n    " + additionalParam + ")\n");
                    }
                }
            }
            if (signature.Columns.Count <= 0)
                strBuilder.Append(")\n");

            strBuilder.Append("{\n");

            strBuilder.Append("    bool RetValue = false;\n");
            if (withConnParam == false)
                strBuilder.Append("    MySqlConnection connection = new MySqlConnection(ConnectionStr);\n");
            if (transactional && withTransParam == false)
                strBuilder.Append("    SqlTransaction transaction = null;\n");
            strBuilder.Append("    MySqlCommand command = null;\n");

            strBuilder.Append("    try\n");
            strBuilder.Append("    {\n");
            if (withConnParam == false)
                strBuilder.Append("        connection.Open();\n\n");

            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        transaction = connection.BeginTransaction();\n\n");
            }
            msg = "        command = new MySqlCommand(\"" + signature.Sql + "\", connection);\n";
            strBuilder.Append(msg);
            strBuilder.Append("        command.CommandType = CommandType.Text;\n\n");
            if (transactional)
                strBuilder.Append("        command.Transaction = transaction;\n\n");


            if (signature.Columns.Count > 0)
                strBuilder.Append("        MySqlParameter parameter = null;\n");

            List<Column> listInputOutput = new List<Column>();
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.direction == ParameterDirection.InputOutput)
                {
                    listInputOutput.Add(col);
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    listInputOutput.Add(col);
                }

                if (col.tableType == false)
                {
                    if (col.length == 0)
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ");\n";
                    }
                    else
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ", " + col.length + ");\n";
                    }
                }
                else
                {
                    msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                        + "\", MySqlDbType.Structured);\n";
                }
                strBuilder.Append(msg);

                if (col.tableType == false)
                {
                    if (col.direction == ParameterDirection.InputOutput)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.InputOutput;\n");
                    }
                    else if (col.direction == ParameterDirection.Output)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.Output;\n");
                    }
                }
                if (col.tableType)
                {
                    strBuilder.Append("        parameter.Value = GetDataTable(" + col.name + ");\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }
                else if ((col.nullableType && col.direction == ParameterDirection.Input) || col.netType == System.Type.GetType("System.Byte[]")
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

            strBuilder.Append("        int affectedRows = command.ExecuteNonQuery();\n\n");

            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        transaction.Commit();\n");
            }

            msg = "        RetValue = affectedRows > 0;\n";
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
            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        try\n");
                strBuilder.Append("        {\n");
                strBuilder.Append("            transaction.Rollback();\n");
                strBuilder.Append("        }\n");
                strBuilder.Append("        catch(Exception exp2)\n");
                strBuilder.Append("        {\n");
                if (string.IsNullOrEmpty(exceptionCode))
                    strBuilder.Append("            throw exp2;\n");
                else
                    strBuilder.Append("            " + exceptionCode + "\n");

                strBuilder.Append("        }\n");
            }
            if (string.IsNullOrEmpty(exceptionCode))
                strBuilder.Append("        throw exp;\n");
            else
                strBuilder.Append("        " + exceptionCode + "\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    finally\n");
            strBuilder.Append("    {\n");
            if (withConnParam == false)
            {
                strBuilder.Append("        if(connection != null)\n");
                strBuilder.Append("            connection.Close();\n");
            }
            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        if(transaction != null)\n");
                strBuilder.Append("            transaction.Dispose();\n");
            }
            strBuilder.Append("        if(command != null)\n");
            strBuilder.Append("            command.Dispose();\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    return RetValue;\n");
            strBuilder.Append("}\n");

            return strBuilder.ToString();
        }
        public static string GenInsertCode(ISignature signature, string spname,
            string additionalParam, string exceptionCode, bool withConnParam, bool withTransParam, bool transactional)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(spname))
            {
                msg = "Name is empty!";
                throw new InvalidDataException(msg);
            }

            StringBuilder strBuilder = new StringBuilder(20000);

            // write function declaration.
            strBuilder.Append("public bool " + spname + "(");
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.tableType)
                {
                    strBuilder.Append("\n    List<" + col.tableTypeStr + "> " + col.name);
                }
                else if (col.direction != ParameterDirection.Input) // output type.
                {
                    strBuilder.Append("\n    ref " + col.netTypeStr + " " + col.name);
                }
                else
                {
                    if (col.nullableType)
                        strBuilder.Append("\n    " + col.netTypeStr + "? " + col.name);
                    else
                        strBuilder.Append("\n    " + col.netTypeStr + " " + col.name);
                }
                if (i != (signature.Columns.Count - 1))
                    strBuilder.Append(",");
                else
                {
                    if (string.IsNullOrEmpty(additionalParam))
                    {
                        if (withTransParam && !withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    SqlTransaction transaction)\n");
                        }
                        else if (withConnParam && !withTransParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else if (withTransParam && withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    SqlTransaction transaction,");
                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else
                            strBuilder.Append(")\n");

                    }
                    else
                    {
                        strBuilder.Append(",");

                        if (withTransParam)
                            strBuilder.Append("\n    SqlTransaction transaction,");

                        if (withConnParam)
                            strBuilder.Append("\n    MySqlConnection connection,");

                        additionalParam = additionalParam.Trim(' ', '\r', '\n', '\t');
                        if (additionalParam.Length > 0 && additionalParam[additionalParam.Length - 1] == ',')
                            additionalParam = additionalParam.Substring(0, additionalParam.Length - 1);
                        strBuilder.Append("\n    " + additionalParam + ")\n");
                    }
                }
            }
            if (signature.Columns.Count <= 0)
                strBuilder.Append(")\n");

            strBuilder.Append("{\n");

            strBuilder.Append("    bool RetValue = false;\n");
            if (withConnParam == false)
                strBuilder.Append("    MySqlConnection connection = new MySqlConnection(ConnectionStr);\n");
            if (transactional && withTransParam == false)
                strBuilder.Append("    SqlTransaction transaction = null;\n");
            strBuilder.Append("    MySqlCommand command = null;\n");

            strBuilder.Append("    try\n");
            strBuilder.Append("    {\n");
            if (withConnParam == false)
                strBuilder.Append("        connection.Open();\n\n");

            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        transaction = connection.BeginTransaction();\n\n");
            }
            msg = "        command = new MySqlCommand(\"" + signature.Sql + "\", connection);\n";
            strBuilder.Append(msg);
            strBuilder.Append("        command.CommandType = CommandType.Text;\n\n");
            if (transactional)
                strBuilder.Append("        command.Transaction = transaction;\n\n");


            if (signature.Columns.Count > 0)
                strBuilder.Append("        MySqlParameter parameter = null;\n");

            List<Column> listInputOutput = new List<Column>();
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.direction == ParameterDirection.InputOutput)
                {
                    listInputOutput.Add(col);
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    listInputOutput.Add(col);
                }

                if (col.tableType == false)
                {
                    if (col.length == 0)
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ");\n";
                    }
                    else
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ", " + col.length + ");\n";
                    }
                }
                else
                {
                    msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                        + "\", MySqlDbType.Structured);\n";
                }
                strBuilder.Append(msg);

                if (col.tableType == false)
                {
                    if (col.direction == ParameterDirection.InputOutput)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.InputOutput;\n");
                    }
                    else if (col.direction == ParameterDirection.Output)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.Output;\n");
                    }
                }
                if (col.tableType)
                {
                    strBuilder.Append("        parameter.Value = GetDataTable(" + col.name + ");\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }
                else if ((col.nullableType && col.direction == ParameterDirection.Input) || col.netType == System.Type.GetType("System.Byte[]")
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

            strBuilder.Append("        int affectedRows = command.ExecuteNonQuery();\n\n");

            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        transaction.Commit();\n");
            }

            msg = "        RetValue = affectedRows > 0;\n";
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
            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        try\n");
                strBuilder.Append("        {\n");
                strBuilder.Append("            transaction.Rollback();\n");
                strBuilder.Append("        }\n");
                strBuilder.Append("        catch(Exception exp2)\n");
                strBuilder.Append("        {\n");
                if (string.IsNullOrEmpty(exceptionCode))
                    strBuilder.Append("            throw exp2;\n");
                else
                    strBuilder.Append("            " + exceptionCode + "\n");

                strBuilder.Append("        }\n");
            }
            if (string.IsNullOrEmpty(exceptionCode))
                strBuilder.Append("        throw exp;\n");
            else
                strBuilder.Append("        " + exceptionCode + "\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    finally\n");
            strBuilder.Append("    {\n");
            if (withConnParam == false)
            {
                strBuilder.Append("        if(connection != null)\n");
                strBuilder.Append("            connection.Close();\n");
            }
            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        if(transaction != null)\n");
                strBuilder.Append("            transaction.Dispose();\n");
            }
            strBuilder.Append("        if(command != null)\n");
            strBuilder.Append("            command.Dispose();\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    return RetValue;\n");
            strBuilder.Append("}\n");

            return strBuilder.ToString();
        }
        public static string GenInsertRetPriKeyCode(ISignature signature, string spname,
            string additionalParam, string exceptionCode, bool withConnParam, bool withTransParam, bool transactional)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(spname))
            {
                msg = "Name is empty!";
                throw new InvalidDataException(msg);
            }

            StringBuilder strBuilder = new StringBuilder(20000);

            // write function declaration.
            strBuilder.Append("public int " + spname + "(");
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.tableType)
                {
                    strBuilder.Append("\n    List<" + col.tableTypeStr + "> " + col.name);
                }
                else if (col.direction != ParameterDirection.Input) // output type.
                {
                    strBuilder.Append("\n    ref " + col.netTypeStr + " " + col.name);
                }
                else
                {
                    if (col.nullableType)
                        strBuilder.Append("\n    " + col.netTypeStr + "? " + col.name);
                    else
                        strBuilder.Append("\n    " + col.netTypeStr + " " + col.name);
                }
                if (i != (signature.Columns.Count - 1))
                    strBuilder.Append(",");
                else
                {
                    if (string.IsNullOrEmpty(additionalParam))
                    {
                        if (withTransParam && !withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    SqlTransaction transaction)\n");
                        }
                        else if (withConnParam && !withTransParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else if (withTransParam && withConnParam)
                        {
                            strBuilder.Append(",");

                            strBuilder.Append("\n    SqlTransaction transaction,");
                            strBuilder.Append("\n    MySqlConnection connection)\n");
                        }
                        else
                            strBuilder.Append(")\n");

                    }
                    else
                    {
                        strBuilder.Append(",");

                        if (withTransParam)
                            strBuilder.Append("\n    SqlTransaction transaction,");

                        if (withConnParam)
                            strBuilder.Append("\n    MySqlConnection connection,");

                        additionalParam = additionalParam.Trim(' ', '\r', '\n', '\t');
                        if (additionalParam.Length > 0 && additionalParam[additionalParam.Length - 1] == ',')
                            additionalParam = additionalParam.Substring(0, additionalParam.Length - 1);
                        strBuilder.Append("\n    " + additionalParam + ")\n");
                    }
                }
            }
            if (signature.Columns.Count <= 0)
                strBuilder.Append(")\n");

            strBuilder.Append("{\n");

            strBuilder.Append("    int RetValue = -1;\n");
            if (withConnParam == false)
                strBuilder.Append("    MySqlConnection connection = new MySqlConnection(ConnectionStr);\n");
            if (transactional && withTransParam == false)
                strBuilder.Append("    SqlTransaction transaction = null;\n");
            strBuilder.Append("    MySqlCommand command = null;\n");
            strBuilder.Append("    try\n");
            strBuilder.Append("    {\n");
            if (withConnParam == false)
                strBuilder.Append("        connection.Open();\n\n");

            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        transaction = connection.BeginTransaction();\n\n");
            }
            msg = "        command = new MySqlCommand(\"" + signature.Sql + "\", connection);\n";
            strBuilder.Append(msg);
            strBuilder.Append("        command.CommandType = CommandType.Text;\n\n");
            if (transactional)
                strBuilder.Append("        command.Transaction = transaction;\n\n");


            if (signature.Columns.Count > 0)
                strBuilder.Append("        MySqlParameter parameter = null;\n");

            List<Column> listInputOutput = new List<Column>();
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.direction == ParameterDirection.InputOutput)
                {
                    listInputOutput.Add(col);
                }
                else if (col.direction == ParameterDirection.Output)
                {
                    listInputOutput.Add(col);
                }

                if (col.tableType == false)
                {
                    if (col.length == 0)
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ");\n";
                    }
                    else
                    {
                        msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                            + "\", MySqlDbType." + col.mySQLType + ", " + col.length + ");\n";
                    }
                }
                else
                {
                    msg = "        parameter = new MySqlParameter(\"" + col.parameterName
                        + "\", MySqlDbType.Structured);\n";
                }
                strBuilder.Append(msg);

                if (col.tableType == false)
                {
                    if (col.direction == ParameterDirection.InputOutput)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.InputOutput;\n");
                    }
                    else if (col.direction == ParameterDirection.Output)
                    {
                        strBuilder.Append("        parameter.Direction = System.Data.ParameterDirection.Output;\n");
                    }
                }
                if (col.tableType)
                {
                    strBuilder.Append("        parameter.Value = GetDataTable(" + col.name + ");\n");
                    strBuilder.Append("        command.Parameters.Add(parameter);\n\n");
                }
                else if ((col.nullableType && col.direction == ParameterDirection.Input) || col.netType == System.Type.GetType("System.Byte[]")
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

            strBuilder.Append("        int priKey = (int)command.ExecuteScalar();\n\n");

            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        transaction.Commit();\n");
            }

            msg = "        RetValue = priKey;\n";
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
            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        try\n");
                strBuilder.Append("        {\n");
                strBuilder.Append("            transaction.Rollback();\n");
                strBuilder.Append("        }\n");
                strBuilder.Append("        catch(Exception exp2)\n");
                strBuilder.Append("        {\n");
                if (string.IsNullOrEmpty(exceptionCode))
                    strBuilder.Append("            throw exp2;\n");
                else
                    strBuilder.Append("            " + exceptionCode + "\n");

                strBuilder.Append("        }\n");
            }
            if (string.IsNullOrEmpty(exceptionCode))
                strBuilder.Append("        throw exp;\n");
            else
                strBuilder.Append("        " + exceptionCode + "\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    finally\n");
            strBuilder.Append("    {\n");
            if (withConnParam == false)
            {
                strBuilder.Append("        if(connection != null)\n");
                strBuilder.Append("            connection.Close();\n");
            }
            if (transactional && withTransParam == false)
            {
                strBuilder.Append("        if(transaction != null)\n");
                strBuilder.Append("            transaction.Dispose();\n");
            }
            strBuilder.Append("        if(command != null)\n");
            strBuilder.Append("            command.Dispose();\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    return RetValue;\n");
            strBuilder.Append("}\n");

            return strBuilder.ToString();
        }
        #endregion // CRUD
    }
}
