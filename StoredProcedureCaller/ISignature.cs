using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoredProcedureCaller
{
    public interface ISignature
    {
        string Name { get; set; }
        List<Column> Columns { get; set; }
        string Sql { get; set; }

        bool Parse(string signature, bool noNullableTypes);
        bool Load(string filePath);
        bool Save(string filePath);
    }
}
