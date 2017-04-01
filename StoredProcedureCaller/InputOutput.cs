using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoredProcedureCaller
{
    public class InputOutput
    {
        public InputOutput()
        {
            Value = null;
        }
        public InputOutput(Object obj)
        {
            Value = obj;
        }

        public Object Value { get; set; }

        public int GetInt()
        {
            int val = -1;
            if (Value == null)
                throw new NullReferenceException("Value is null!");

            try
            {
                val = Convert.ToInt32(Value);
            }
            catch (System.InvalidCastException ex)
            {
                throw ex;
            }

            return val;
        }
        public bool GetBoolean()
        {
            bool val = false;
            if (Value == null)
                throw new NullReferenceException("Value is null!");

            try
            {
                val = Convert.ToBoolean(Value);
            }
            catch (System.InvalidCastException ex)
            {
                throw ex;
            }

            return val;
        }
        public string GetString()
        {
            string val = null;
            if (Value == null)
                throw new NullReferenceException("Value is null!");

            try
            {
                val = Convert.ToString(Value);
            }
            catch (System.InvalidCastException ex)
            {
                throw ex;
            }

            return val;
        }
        public DateTime GetDateTime()
        {
            DateTime val;
            if (Value == null)
                throw new NullReferenceException("Value is null!");

            try
            {
                val = Convert.ToDateTime(Value);
            }
            catch (System.InvalidCastException ex)
            {
                throw ex;
            }

            return val;
        }

    }
}
