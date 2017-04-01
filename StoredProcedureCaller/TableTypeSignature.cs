using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Elmax;

namespace StoredProcedureCaller
{
    public class TableTypeSignature
    {

        public string Name { get; set; }
        public List<Column> Columns { get; set; }

        public TableTypeSignature()
        {
            Name = string.Empty;
            Columns = new List<Column>();
        }
        public TableTypeSignature(string signature)
        {
            Columns = new List<Column>();
            Parse(signature, true);
        }
        public bool Parse(string signature, bool noNullableTypes)
        {
            Columns.Clear();
            string signLower = signature.ToLower();

            int pos = signLower.IndexOf("type");

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
                    if (IsWhitespace(c)==false)
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
                throw new InvalidDataException("Empty table type name");
            }

            pos = signature.IndexOf("(");

            if (pos == -1)
                return true; // no parameters for this stored procedure

            // Parsing parameters
            do
            {
                bool BeforeParsingName = true;
                bool ParsingName = false;
                bool ParsingNameEnded = false;
                bool ParsingType = false;
                bool ParsingTypeEnded = false;
                bool ParsingOutputType = false;
                bool ParsingLength = false;

                Column col = new Column();
                string name = string.Empty;
                string type = string.Empty;
                string output = string.Empty;
                string slength = string.Empty;
                for (int i = pos + 1; i < signature.Length; ++i)
                {
                    char c = signature[i];
                    if (BeforeParsingName && ParsingName == false)
                    {
                        if (IsWhitespace(c))
                            continue;
                        else
                        {
                            ParsingName = true;
                            name += c;
                        }
                    }
                    else if (ParsingName && ParsingNameEnded == false)
                    {
                        if (c == ',')
                            continue;

                        if (IsWhitespace(c) == false)
                            name += c;
                        else
                            ParsingNameEnded = true;
                    }
                    else if (ParsingNameEnded && ParsingType == false)
                    {
                        string lowerName = name.ToLower();
                        if(lowerName=="primary"||lowerName=="constraint")
                            break;

                        name = name.Trim('[', ']');

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
                        if (IsWhitespace(c) == false && c != ',' && c != '(' && c != ')')
                            type += c;
                        else
                        {
                            ParsingTypeEnded = true;
                            if (c == ')') // signature ended
                            {
                                bool b = TypeSetter.FillType(type, col, noNullableTypes);
                                if (b == false)
                                {
                                    throw new InvalidDataException("Invalid type of \"" + type + "\"?");
                                }
                                col.name = name;
                                Columns.Add(col);

                                pos = i;
                                break;
                            }
                        }

                        if (c == ',' || c == ')') // ) means last parameter
                        {
                            bool b = TypeSetter.FillType(type, col, noNullableTypes);
                            if (b == false)
                            {
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
                            else if (c == ',' || c == ')') // no output type specified, so is input only.
                            {
                                bool b = TypeSetter.FillType(type, col, noNullableTypes);
                                if (b == false)
                                {
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
                        else // ParsingLength == true
                        {
                            if (c != ')')
                            {
                                slength += c;
                            }
                            else
                            {
                                bool b = TypeSetter.FillType(type, col, noNullableTypes);
                                if (b == false)
                                {
                                    throw new InvalidDataException("Invalid type of \"" + type + "\"?");
                                }
                                TypeSetter.FillNullType(output, col);
                                try
                                {
                                    col.length = Convert.ToUInt32(slength);
                                }
                                catch (System.Exception)
                                {
                                    throw new InvalidDataException("Invalid length :" + slength + "!");
                                }
                                col.name = name;
                                Columns.Add(col);
                                pos = i;
                                break;

                            }
                        }
                    }
                    else if (ParsingOutputType)
                    {
                        if (IsWhitespace(c) == false && c != ')' && c != ',')
                            output += signature[i];
                        else
                        {
                            bool b = TypeSetter.FillType(type, col, noNullableTypes);
                            if (b == false)
                            {
                                throw new InvalidDataException("Invalid type of \"" + type + "\"?");
                            }
                            TypeSetter.FillNullType(output, col);
                            col.name = name;
                            Columns.Add(col);
                            pos = i;
                            break;
                        }

                    }
                }
            }
            while ((pos = signature.IndexOf(",", pos + 1)) != -1);

            return false;
        }
        private static bool IsWhitespace(char c)
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
        private static bool IsDelimiter(char c)
        {
            if (c == ' ')
                return true;
            else if (c == '\t')
                return true;
            else if (c == '\r')
                return true;
            else if (c == '\n')
                return true;
            else if (c == '(')
                return true;
            else if (c == ')')
                return true;
            else if (c == ',')
                return true;

            return false;
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
