using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace StoredProcedureCaller
{
    public class TableTypeGen
    {
        public static string GenCode(TableTypeSignature signature)
        {
            StringBuilder strBuilder = new StringBuilder(20000);

            // write the class now
            strBuilder.Append("public class " + signature.Name +"\n");
            strBuilder.Append("{\n");
            // write the default class constructor now
            strBuilder.Append("    // Default Constructor\n");
            strBuilder.Append("    public " + signature.Name + "()\n");
            strBuilder.Append("    {\n");
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.canBeNull)
                    strBuilder.Append("        " + col.name + " = null;\n");
                else
                    strBuilder.Append("        " + col.name + " = " + col.initConstr + ";\n");
            }
            strBuilder.Append("    }\n\n");
            // write the class constructor now
            strBuilder.Append("    // Constructor\n");
            strBuilder.Append("    public " + signature.Name + "(\n");
            for (int i = 0; i < signature.Columns.Count; ++i)
            {
                Column col = signature.Columns[i];
                if (col.canBeNull && col.nullableType)
                    strBuilder.Append("        " + col.netTypeStr + "? " + col.name + "Temp");
                else
                    strBuilder.Append("        " + col.netTypeStr + " " + col.name + "Temp");

                if (i != signature.Columns.Count-1)
                    strBuilder.Append(",\n");
                else
                    strBuilder.Append(")\n");

            }
            // write the class constructor body now
            strBuilder.Append("    {\n");
            foreach (Column col in signature.Columns)
            {
                    strBuilder.Append("        " + col.name + " = " + col.name+"Temp;\n");
            }
            strBuilder.Append("    }\n\n");
            foreach (Column col in signature.Columns)
            {
                if(col.canBeNull&&col.nullableType)
                    strBuilder.Append("    public " + col.netTypeStr + "? " + col.name );
                else
                    strBuilder.Append("    public " + col.netTypeStr + " " + col.name);

                strBuilder.Append(";\n");
            }
            strBuilder.Append("}\n\n");

            // write the fill DataTable method now.
            strBuilder.Append("DataTable GetDataTable(List<" + signature.Name + "> list)\n");
            strBuilder.Append("{\n");
            strBuilder.Append("    if(list==null)\n");
            strBuilder.Append("        return null;\n");
            strBuilder.Append("    if(list.Count<=0)\n");
            strBuilder.Append("        return null;\n\n");
            strBuilder.Append("    //Create DataTable and add Columns\n");
            strBuilder.Append("    DataTable tbl = new DataTable();\n");
            foreach (Column col in signature.Columns)
            {
                strBuilder.Append("    tbl.Columns.Add(\""+col.name+"\", ");
                strBuilder.Append("System.Type.GetType(\""+col.netType.ToString()+"\"));\n");
            }

            strBuilder.Append("\n    foreach("+ signature.Name + " obj in list)\n");
            strBuilder.Append("    {\n");
            strBuilder.Append("        DataRow newRow = tbl.NewRow();\n");
            foreach (Column col in signature.Columns)
            {
                if (col.canBeNull)
                {
                    strBuilder.Append("        if(obj." + col.name + "==null)\n");
                    strBuilder.Append("            newRow[\""+col.name+"\"] = DBNull.Value;\n");
                    strBuilder.Append("        else\n");
                    strBuilder.Append("            newRow[\"" + col.name + "\"] = obj." + col.name + ";\n\n");
                }
                else
                    strBuilder.Append("        newRow[\""+col.name+"\"] = obj."+col.name+";\n\n");
            }
            strBuilder.Append("        tbl.Rows.Add(newRow);\n");
            strBuilder.Append("    }\n");
            strBuilder.Append("    return tbl;\n");
            strBuilder.Append("}\n\n");

            return strBuilder.ToString();
        }
    }
}
