using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace StoredProcedureCaller
{
    class TypeSetter
    {
        private static Dictionary<string, Column> dict;
        static TypeSetter()
        {
            dict = new Dictionary<string, Column>();
            InitDict();
        }
        public static bool FillOutputType(string outputType, Column col)
        {
            if (string.IsNullOrEmpty(outputType))
            {
                col.direction = ParameterDirection.Input;
                return true;
            }

            string lower = outputType.ToLower();
            lower = lower.Trim();
            if (lower.IndexOf("inputoutput") != -1)
                col.direction = ParameterDirection.InputOutput;
            else if (lower.IndexOf("input") != -1)
                col.direction = ParameterDirection.Input;
            else if (lower.IndexOf("output") != -1)
                col.direction = ParameterDirection.Output;
            else if (lower.IndexOf("readonly") != -1)
                col.tableType = true;
            else
                return false;

            return true;
        }
        public static bool FillMySQLOutputType(string outputType, Column col)
        {
            if (string.IsNullOrEmpty(outputType))
            {
                col.direction = ParameterDirection.Input;
                return true;
            }

            string lower = outputType.ToLower();
            if (lower.IndexOf("inout") != -1)
                col.direction = ParameterDirection.InputOutput;
            else if (lower.IndexOf("in") != -1)
                col.direction = ParameterDirection.Input;
            else if (lower.IndexOf("out") != -1)
                col.direction = ParameterDirection.Output;
            else
                return false;

            return true;
        }
        public static bool FillNullType(string nullType, Column col)
        {
            if (string.IsNullOrEmpty(nullType))
            {
                col.canBeNull = true;
                return true;
            }

            string lower = nullType.ToLower();
            if (lower.IndexOf("not") != -1)
                col.canBeNull = false;
            else if (lower.IndexOf("null") != -1)
                col.canBeNull = true;
            else
            {
                col.canBeNull = true;
                return false;
            }

            return true;
        }
        public static bool FillType(string sqlType, Column col, bool noNullableTypes)
        {
            if (col.tableType)
            {
                col.tableTypeStr = sqlType.Trim();
                return false;
            }

            lock (dict)
            {
                string lower = sqlType.ToLower();
                lower = lower.Trim('(', ')');
                if (dict.ContainsKey(lower))
                {
                    Column found = dict[lower];
                    col.netType = found.netType;
                    col.sqlType = found.sqlType;
                    if (noNullableTypes)
                        col.nullableType = false;
                    else
                        col.nullableType = found.nullableType;
                    col.netTypeStr = found.netTypeStr;
                    col.initConstr = found.initConstr;
                    col.hasStrLength = found.hasStrLength;
                    col.mySQLType = found.mySQLType;

                    return true;
                }
            }
            throw new System.IO.InvalidDataException("Type cannot be found : " + sqlType.ToString());
        }

        private static void InitDict()
        {
            if (dict == null)
                return;

            Column col = null;

            col = new Column();
            col.netType = System.Type.GetType("System.Int64");
            col.netTypeStr = "long";
            col.sqlType = SqlDbType.BigInt;
            col.mySQLType = "Int64";
            col.nullableType = true;
            col.initConstr = "0";
            col.hasStrLength = false;
            dict.Add("BigInt".ToLower(), col);
            dict.Add("[BigInt]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Byte[]");
            col.netTypeStr = "byte[]";
            col.sqlType = SqlDbType.Binary;
            col.mySQLType = "Binary";
            col.nullableType = false;
            col.initConstr = "null";
            col.hasStrLength = true;
            dict.Add("Binary".ToLower(), col);
            dict.Add("[Binary]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Boolean");
            col.netTypeStr = "bool";
            col.sqlType = SqlDbType.Bit;
            col.mySQLType = "Bit";
            col.nullableType = true;
            col.initConstr = "false";
            col.hasStrLength = false;
            dict.Add("Bit".ToLower(), col);
            dict.Add("[Bit]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.String");
            col.netTypeStr = "string";
            col.sqlType = SqlDbType.Char;
            col.mySQLType = "Char";
            col.nullableType = false;
            col.initConstr = "string.Empty";
            col.hasStrLength = true;
            dict.Add("Char".ToLower(), col);
            dict.Add("[Char]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.DateTime");
            col.netTypeStr = "DateTime";
            col.sqlType = SqlDbType.DateTime;
            col.mySQLType = "DateTime";
            col.nullableType = true;
            col.initConstr = "new DateTime()";
            col.hasStrLength = false;
            dict.Add("DateTime".ToLower(), col);
            dict.Add("[DateTime]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Decimal");
            col.netTypeStr = "decimal";
            col.sqlType = SqlDbType.Decimal;
            col.mySQLType = "Decimal";
            col.nullableType = true;
            col.initConstr = "0m";
            col.hasStrLength = false;
            dict.Add("Decimal".ToLower(), col);
            dict.Add("[Decimal]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Double");
            col.netTypeStr = "double";
            col.sqlType = SqlDbType.Float;
            col.mySQLType = "Double";
            col.nullableType = true;
            col.initConstr = "0.0";
            col.hasStrLength = false;
            dict.Add("Float".ToLower(), col);
            dict.Add("[Float]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Double");
            col.netTypeStr = "double";
            col.sqlType = SqlDbType.Float;
            col.mySQLType = "Double";
            col.nullableType = true;
            col.initConstr = "0.0";
            col.hasStrLength = false;
            dict.Add("Double".ToLower(), col);
            dict.Add("[Double]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Byte[]");
            col.netTypeStr = "byte[]";
            col.sqlType = SqlDbType.Image;
            col.mySQLType = "Blob";
            col.nullableType = false;
            col.initConstr = "null";
            col.hasStrLength = true;
            dict.Add("Image".ToLower(), col);
            dict.Add("[Image]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Int32");
            col.netTypeStr = "int";
            col.sqlType = SqlDbType.Int;
            col.mySQLType = "Int32";
            col.nullableType = true;
            col.initConstr = "0";
            col.hasStrLength = false;
            dict.Add("Int".ToLower(), col);
            dict.Add("[Int]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Int32");
            col.netTypeStr = "int";
            col.sqlType = SqlDbType.Int;
            col.mySQLType = "Int32";
            col.nullableType = true;
            col.initConstr = "0";
            col.hasStrLength = false;
            dict.Add("Integer".ToLower(), col);
            dict.Add("[Integer]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Decimal");
            col.netTypeStr = "decimal";
            col.sqlType = SqlDbType.Money;
            col.mySQLType = "Decimal";
            col.nullableType = true;
            col.initConstr = "0m";
            col.hasStrLength = false;
            dict.Add("Money".ToLower(), col);
            dict.Add("[Money]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.String");
            col.netTypeStr = "string";
            col.sqlType = SqlDbType.NChar;
            col.mySQLType = "String";
            col.nullableType = false;
            col.initConstr = "string.Empty";
            col.hasStrLength = true;
            dict.Add("NChar".ToLower(), col);
            dict.Add("[NChar]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.String");
            col.netTypeStr = "string";
            col.sqlType = SqlDbType.NText;
            col.mySQLType = "String";
            col.nullableType = false;
            col.initConstr = "string.Empty";
            col.hasStrLength = true;
            dict.Add("NText".ToLower(), col);
            dict.Add("[NText]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.String");
            col.netTypeStr = "string";
            col.sqlType = SqlDbType.NVarChar;
            col.mySQLType = "String";
            col.nullableType = false;
            col.initConstr = "string.Empty";
            col.hasStrLength = true;
            dict.Add("NVarChar".ToLower(), col);
            dict.Add("[NVarChar]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Single");
            col.netTypeStr = "float";
            col.sqlType = SqlDbType.Real;
            col.mySQLType = "Float";
            col.nullableType = true;
            col.initConstr = "0.0f";
            col.hasStrLength = false;
            dict.Add("Real".ToLower(), col);
            dict.Add("[Real]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Guid");
            col.netTypeStr = "Guid";
            col.sqlType = SqlDbType.UniqueIdentifier;
            col.mySQLType = "Guid";
            col.nullableType = true;
            col.initConstr = "Guid.NewGuid()";
            col.hasStrLength = false;
            dict.Add("UniqueIdentifier".ToLower(), col);
            dict.Add("[UniqueIdentifier]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.DateTime");
            col.netTypeStr = "DateTime";
            col.sqlType = SqlDbType.SmallDateTime;
            col.mySQLType = "DateTime";
            col.nullableType = true;
            col.initConstr = "new DateTime()";
            col.hasStrLength = false;
            dict.Add("SmallDateTime".ToLower(), col);
            dict.Add("[SmallDateTime]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Int16");
            col.netTypeStr = "short";
            col.sqlType = SqlDbType.SmallInt;
            col.mySQLType = "Int16";
            col.nullableType = true;
            col.initConstr = "0";
            col.hasStrLength = false;
            dict.Add("SmallInt".ToLower(), col);
            dict.Add("[SmallInt]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Decimal");
            col.netTypeStr = "decimal";
            col.sqlType = SqlDbType.SmallMoney;
            col.mySQLType = "Decimal";
            col.nullableType = true;
            col.initConstr = "0m";
            col.hasStrLength = false;
            dict.Add("SmallMoney".ToLower(), col);
            dict.Add("[SmallMoney]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.String");
            col.netTypeStr = "string";
            col.sqlType = SqlDbType.Text;
            col.mySQLType = "Text";
            col.nullableType = false;
            col.initConstr = "string.Empty";
            col.hasStrLength = true;
            dict.Add("Text".ToLower(), col);
            dict.Add("[Text]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Byte[]");
            col.netTypeStr = "byte";
            col.sqlType = SqlDbType.Timestamp;
            col.mySQLType = "Timestamp";
            col.nullableType = false;
            col.initConstr = "null";
            col.hasStrLength = true;
            dict.Add("Timestamp".ToLower(), col);
            dict.Add("[Timestamp]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Byte");
            col.netTypeStr = "byte";
            col.sqlType = SqlDbType.TinyInt;
            col.mySQLType = "Byte";
            col.nullableType = true;
            col.initConstr = "0";
            col.hasStrLength = false;
            dict.Add("TinyInt".ToLower(), col);
            dict.Add("[TinyInt]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Byte[]");
            col.netTypeStr = "byte[]";
            col.sqlType = SqlDbType.VarBinary;
            col.mySQLType = "VarBinary";
            col.nullableType = false;
            col.initConstr = "null";
            col.hasStrLength = true;
            dict.Add("VarBinary".ToLower(), col);
            dict.Add("[VarBinary]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.String");
            col.netTypeStr = "string";
            col.sqlType = SqlDbType.VarChar;
            col.mySQLType = "String";
            col.nullableType = false;
            col.initConstr = "string.Empty";
            col.hasStrLength = true;
            dict.Add("VarChar".ToLower(), col);
            dict.Add("[VarChar]".ToLower(), col);

            col = new Column();
            col.netType = System.Type.GetType("System.Object");
            col.netTypeStr = "object";
            col.sqlType = SqlDbType.Variant;
            col.mySQLType = "VarBinary";
            col.nullableType = false;
            col.initConstr = "null";
            col.hasStrLength = true;
            dict.Add("Variant".ToLower(), col);
            dict.Add("[Variant]".ToLower(), col);
        }
    }
}
