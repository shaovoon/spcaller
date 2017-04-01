using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace StoredProcedureCaller
{
    public class Column
    {
        public Column()
        {
            netType = System.Type.GetType("System.Int32");
            netTypeStr = "int";
            sqlType = SqlDbType.Int;
            _name = null;
            parameterName = null;
            direction = ParameterDirection.Input;
            length = 0;
            inputOuput = null;
            nullableType = false;
            canBeNull = false;
            tableType = false;
            tableTypeStr = string.Empty;
            initConstr = string.Empty;
            hasStrLength = false;
            mySQLType = string.Empty;
        }
        private string _name;
        public Type netType;
        public string netTypeStr;
        public SqlDbType sqlType;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                parameterName = "@" + _name;
            }

        }
        public string parameterName
        {
            get;
            private set;
        }
        public ParameterDirection direction;
        public uint length;
        public InputOutput inputOuput;
        public bool nullableType;
        public bool canBeNull;
        public bool tableType;
        public string tableTypeStr;
        public string initConstr;
        public bool hasStrLength;
        public string mySQLType;
    }
}
