using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Elmax;

namespace StoredProcedureCaller
{
    public enum SQLType
    {
        NONE,
        SELECT,
        INSERT,
        UPDATE,
        DELETE,
    }

    public class SPSignature : StoredProcedureCaller.ISignature
    {

        public string Name { get; set; }
        public bool HasTableParam { get; set; }
        public int TableParamNum { get; set; }
        public string Sql { get; set; }
        public List<Column> Columns { get; set; }

        public SPSignature()
        {
            Name = string.Empty;
            Columns = new List<Column>();
            HasTableParam = false;
        }
        public SPSignature(string signature)
        {
            Columns = new List<Column>();
            HasTableParam = false;
            Parse(signature, true);
        }
        #region Load and Save method
        public bool Load(string filePath)
        {
            if(File.Exists(filePath)==false)
                return false;

            Name = string.Empty;
            Columns.Clear();

            XmlDocument doc;
            bool b = CreateAndLoadXml(out doc, filePath);
            if (b)
            {
                Element root = new Element();
                root.SetDomDoc(doc);

                root = root["SPSignature"];
                if (root.Exists == false)
                {
                    return false;
                }

                if (root["SPName"].Exists)
                {
                    Name = root["SPName"].GetString("Error");
                }
                else
                    return false;

                Element eleColumns = root["Columns"];
                if (eleColumns.Exists)
                {
                    List<Element> list = eleColumns.GetCollection("Column");
                    for (int i = 0; i < list.Count; ++i)
                    {
                        Column col = new Column();
                        col.name = list[i].GetString("Error!");
                        //col.parameterName = "@" + col.name;

                        string sNetType = list[i].Attribute("netType").GetString("System.Int32");
                        col.netType = System.Type.GetType(sNetType);

                        string sSqlType = list[i].Attribute("sqlType").GetString("Int");
                        col.sqlType = (System.Data.SqlDbType)Enum.Parse(typeof(System.Data.SqlDbType), sSqlType);
                        col.length = list[i].Attribute("length").GetUInt(0);
                        string sDirection = list[i].Attribute("direction").GetString("Input");
                        col.direction = (System.Data.ParameterDirection)Enum.Parse(typeof(System.Data.ParameterDirection), sDirection);;

                        Columns.Add(col);
                    }
                }
                else
                    return false;

                return true;
            }

            return false;
        }
        public bool Save(string filePath)
        {
            XmlDocument doc;
            bool b = CreateAndInitDom(out doc);
            if (b)
            {
                Element root = new Element();
                root.SetDomDoc(doc);

                root = root["SPSignature"];

                root["SPName"].SetString(Name);

                Element eleColumns = root["Columns"].Create(null);
                foreach (Column col in Columns)
                {
                    Element eleCol = eleColumns["Column"].CreateNew(null);
                    eleCol.SetString(col.name);
                    eleCol.Attribute("netType").SetString(col.netType.ToString());
                    eleCol.Attribute("sqlType").SetString(col.sqlType.ToString());

                    eleCol.Attribute("length").SetUInt(col.length);
                    eleCol.Attribute("direction").SetString(col.direction.ToString());
                }

                return SaveXml(doc, filePath);
            }

            return false;
        }
        #endregion // Load and Save method

        public bool Parse(string signature, bool noNullableTypes)
        {
            Columns.Clear();
            string signLower = signature.ToLower();

            int pos = signLower.IndexOf("proc");

            if (pos == -1)
                return false;

            bool PosInSpaceBefProcName = false;
            bool PosProcName = false;
            string spname = string.Empty;
            for (int i = pos; i < signature.Length; ++i)
            {
                char c = signature[i];

                if (PosInSpaceBefProcName == false)
                {
                    if (IsWhitespace(c) == false)
                        continue;
                    else
                    {
                        PosInSpaceBefProcName = true;
                    }
                }
                else if (PosInSpaceBefProcName && PosProcName==false)
                {
                    if (IsWhitespace(c))
                        continue;
                    else
                    {
                        spname += c;
                        PosProcName = true;
                    }
                }
                else if (PosProcName)
                {
                    if (IsWhitespace(c)==false&&c!='(')
                        spname += c;
                    else
                    {
                        
                        break;
                    }
                }
            }

            Name = spname;
            if (string.IsNullOrEmpty(Name))
            {
                throw new InvalidDataException("Empty stored procedure name");
            }

            pos = signature.IndexOf("@");

            if (pos == -1)
                return true; // no parameters for this stored procedure

            pos -= 1;
            // Parsing parameters
            while((pos = signature.IndexOf("@", pos+1)) !=-1)
            {
                bool ParsingName = true;
                bool ParsingNameEnded = false;
                bool ParsingType = false;
                bool ParsingTypeEnded = false;
                bool ParsingOutputType = false;
                bool ParsingLength = false;
                bool ParsingLengthEnded = false;

                Column col = new Column();
                string name = string.Empty;
                string type = string.Empty;
                string output = string.Empty;
                string slength = string.Empty;
                for (int i = pos + 1; i < signature.Length; ++i)
                {
                    char c = signature[i];
                    if (ParsingName && ParsingNameEnded == false)
                    {
                        if(c==',')
                            continue;

                        if (IsWhitespace(c) == false)
                            name += c;
                        else 
                            ParsingNameEnded = true;
                    }
                    else if (ParsingNameEnded && ParsingType == false)
                    {
                        if (IsWhitespace(c) == false)
                        {
                            ParsingType = true;
                            type += c;
                        }
                        else
                            continue;
                    }
                    else if (ParsingType && ParsingTypeEnded == false)
                    {
                        if (IsWhitespace(c) == false && c != ',' && c != '(' && c != ')' && c != '=')
                            type += c;
                        else
                        {
                            ParsingTypeEnded = true;
                            if (c == ')' || i == signature.Length - 1) // signature ended
                            {
                                bool b = TypeSetter.FillType(type, col, noNullableTypes);
                                if (b == false)
                                {
                                    if (col.tableType)
                                    {
                                        HasTableParam = true;
                                    }
                                    else
                                        throw new InvalidDataException("Invalid type of \"" + type + "\"?");
                                }
                                col.name = name;
                                Columns.Add(col);

                                pos = i;
                                break;
                            }
                        }

                        if (c == ',' || c == ')' || c == '=' || i == signature.Length-1) // ) means last parameter
                        {
                            bool b = TypeSetter.FillType(type, col, noNullableTypes);
                            if (b == false)
                            {
                                if (col.tableType)
                                {
                                    HasTableParam = true;
                                }
                                else
                                    throw new InvalidDataException("Invalid type of \"" + type + "\"?");
                            }
                            col.name = name;
                            Columns.Add(col);

                            pos = i;
                            break;
                        }

                        if (c == '(')
                            ParsingLength = true;
                    }
                    else if (ParsingTypeEnded && ParsingOutputType == false)
                    {
                        if (ParsingLength == false)
                        {
                            if (c == '(')
                            {
                                ParsingLength = true;
                            }
                            else if (c == ',' || c == ')' || c == '=' || i == signature.Length - 1) // no output type specified, so is input only.
                            {
                                bool b = TypeSetter.FillType(type, col, noNullableTypes);
                                if (b == false)
                                {
                                    if (col.tableType)
                                    {
                                        HasTableParam = true;
                                    }
                                    else
                                        throw new InvalidDataException("Invalid type of \"" + type + "\"?");
                                }
                                col.name = name;
                                Columns.Add(col);
                                pos = i;
                                break;
                            }
                            else if (IsWhitespace(c) == false)
                            {
                                ParsingOutputType = true;
                                output += signature[i];
                            }
                            else
                                continue;
                        }
                        else if(ParsingLength == true)
                        {
                            if (ParsingLengthEnded == false && IsWhitespace(c) == false && c != ')' && c != ',')
                            {
                                slength += c;
                            }
                            else if (ParsingLengthEnded == false)
                            {
                                ParsingLengthEnded = true;
                            }
                            else if (IsWhitespace(c))
                            {
                                continue;
                            }
                            else if (ParsingLengthEnded)
                            {
                                if (c == ',' || c == ')' || i == signature.Length - 1) // parsing ended for this parameter
                                {
                                    bool b = TypeSetter.FillType(type, col, noNullableTypes);
                                    if (b == false)
                                    {
                                        if (col.tableType)
                                        {
                                            HasTableParam = true;
                                        }
                                        else
                                            throw new InvalidDataException("Invalid type of \"" + type + "\"?");
                                    }

                                    try
                                    {
                                        col.length = Convert.ToUInt32(slength);
                                    }
                                    catch (System.Exception)
                                    {
                                    }

                                    col.name = name;
                                    Columns.Add(col);
                                    pos = i;
                                    break;
                                }

                                if (c == 'i' || c == 'I' || c == 'o' || c == 'O')
                                {
                                    output += c;
                                    ParsingOutputType = true;
                                }
                            }
                        }
                    }
                    else if (ParsingOutputType)
                    {
                        if (IsWhitespace(c) == false && c != ')' && c != ',' && c != '=')
                            output += signature[i];
                        else
                        {
                            bool b = TypeSetter.FillOutputType(output, col);
                            if (b == false)
                            {
                                throw new InvalidDataException("Invalid output type of \"" + output + "\"?");
                            }

                            b = TypeSetter.FillType(type, col, noNullableTypes);
                            if (b == false)
                            {
                                if (col.tableType)
                                {
                                    HasTableParam = true;
                                }
                                else
                                    throw new InvalidDataException("Invalid type of \"" + type + "\"?");
                            }

                            try
                            {
                                col.length = Convert.ToUInt32(slength);
                            }
                            catch (System.Exception)
                            {
                            }

                            col.name = name;
                            Columns.Add(col);
                            pos = i;
                            break;
                        }

                    }
                }
            }

            TableParamNum = 0;
            if (HasTableParam)
            {
                foreach (Column col in Columns)
                {
                    if (col.tableType)
                        ++TableParamNum;
                }
            }
            return false;
        }
        public static bool IsWhitespace(char c)
        {
            if(c==' ')
                return true;
            else if(c=='\t')
                return true;
            else if(c=='\r')
                return true;
            else if(c=='\n')
                return true;

            return false;
        }

        public bool AddColumn(string name, string type, string length)
        {
            Column col = new Column();
            bool b = TypeSetter.FillType(type, col, true);
            if (b == false)
            {
                if (col.tableType)
                {
                    HasTableParam = true;
                }
                else
                    throw new InvalidDataException("Invalid type of \"" + type + "\"?");
            }

            if (name.Length > 0 && name[0] == '@')
                name = name.Substring(1);

            col.name = name;
            col.canBeNull = false;
            col.nullableType = false;

            if (col.hasStrLength)
            {
                try
                {
                    uint len = uint.Parse(length);
                    col.length = len;
                }
                catch (System.Exception)
                {
                    col.length = 0;
                }
            }
            else
                col.length = 0;

            for (int i = 0; i < Columns.Count; ++i)
            {
                if(Columns[i].name.ToLower()==name.ToLower())
                    throw new InvalidDataException(string.Format("Parameter '{0}' is already exists", name));
            }

            Columns.Add(col);

            return true;
        }

        public void ClearAllColumns()
        {
            Columns.Clear();
        }

        public SQLType GetType(string sql)
        {
            SQLType type = SQLType.NONE;

            sql = sql.Trim();

            if (sql.ToLower().IndexOf("select") == 0)
                type = SQLType.SELECT;
            else if (sql.ToLower().IndexOf("insert") == 0)
                type = SQLType.INSERT;
            else if (sql.ToLower().IndexOf("update") == 0)
                type = SQLType.UPDATE;
            else if (sql.ToLower().IndexOf("delete") == 0)
                type = SQLType.DELETE;

            return type;
        }

        public List<string> GetAllParam(string sql)
        {
            List<string> list = new List<string>();

            int pos = 0;
            do
            {
                pos = sql.IndexOf('@', pos);

                if (pos == -1)
                    break;

                string param = string.Empty;
                int i = pos;
                for (; i < sql.Length; ++i)
                {

                    if (Char.IsWhiteSpace(sql[i]) == false
                        && sql[i] != '-' && sql[i] != ';' 
                        && sql[i] != ')' && sql[i] != ',')
                    {
                        param += sql[i];
                    }
                    else
                    {
                        list.Add(param);
                        param = string.Empty;
                        break;
                    }
                }

                pos = i;
                if (string.IsNullOrEmpty(param)==false)
                {
                    list.Add(param);
                    break;
                }
            }
            while (true);
            return list;
        }


        #region Private XML helper methods
        private bool CreateAndInitDom(out XmlDocument doc)
        {
            doc = new XmlDocument();
            if (doc != null)
            {
                XmlProcessingInstruction pi = doc.CreateProcessingInstruction("xml", " version='1.0' encoding='UTF-8'");
                doc.AppendChild(pi);
            }
            else
                return false;
            return true;
        }
        private bool SaveXml(XmlDocument doc, System.String strFilename)
        {
            System.String szPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            szPath = Path.Combine(szPath, strFilename);
            doc.Save(szPath);
            FileInfo fi = new FileInfo(szPath);

            return fi.Exists;
        }
        private bool CreateAndLoadXml(out XmlDocument doc, System.String strFilename)
        {
            System.String szPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            szPath = Path.Combine(szPath, strFilename);

            doc = new XmlDocument();
            try
            {
                doc.Load(szPath);
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }
        #endregion // Private XML helper methods

    }
}
