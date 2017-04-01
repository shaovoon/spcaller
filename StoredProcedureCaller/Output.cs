using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoredProcedureCaller
{
    public class Output : InputOutput
    {
        public Output()
        {
            Value = null;
        }
        public Output(Object obj)
        {
            Value = obj;
        }
    }
}
