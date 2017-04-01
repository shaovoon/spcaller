using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Elmax
{
    public class Attribute
    {
        #region Constructors
        public Attribute()
        {
            m_Doc = null;
            m_Node = null;

            m_strAttrName = string.Empty;
        }

        // Non-default constructor
        public Attribute(
	        XmlDocument doc, 
	        XmlNode node)
        {
            m_Doc = doc;
            m_Node = node;
        }
        #endregion

        #region Misc Methods
        //! Returns true if the attribute with the name exists.
	    public bool Exists
        {
            get
            {
                if (m_Doc == null || m_Node == null)
                    return false;

                string wstrValue = null;
                bool bExists = false;
                GetAttributeAt(m_strAttrName, out wstrValue, out bExists);

                return bExists;
            }
        }
        public string Name
        {
            get
            {
                return m_strAttrName;
            }
        }
	    //! Create this attribute with this optional namespaceUri
	    public bool Create(string namespaceUri)
        {
	        if(m_Doc!=null&&m_Node!=null)
	        {
		        bool bExists = false;
		        string wstrValue = null;
		        GetAttributeAt(m_strAttrName, out wstrValue, out bExists);
		        if(false==bExists)
		        {
			        XmlAttributeCollection attrList = m_Node.Attributes;
			        XmlAttribute pAttr = m_Doc.CreateAttribute(m_strAttrName, namespaceUri);

			        if(attrList!=null&&pAttr!=null)
                        attrList.SetNamedItem(pAttr);

                    return true;
		        }
	        }

	        return false;
        }

	    //! Delete this attribute
	    public bool Delete()
        {
	        if(m_Node!=null)
	        {
		        bool bExists = false;
		        string wstrValue;
		        GetAttributeAt(m_strAttrName, out wstrValue, out bExists);
		        if(bExists)
		        {
			        XmlAttributeCollection attrList = m_Node.Attributes;

			        if(attrList!=null)
				        attrList.RemoveNamedItem(m_strAttrName);
		        }
		        else
			        return false;
	        }
	        else
		        throw new InvalidOperationException("No valid node in this Attribute!");

	        return true;
        }

	    //! Set the data members
	    public void SetParam(
		    XmlDocument doc, 
		    XmlNode node, 
		    string wstrAttrName)
        {
            m_Doc = doc;
            m_Node = node;
            m_strAttrName = wstrAttrName;
        }
        #endregion
        #region Data Setters
        //! Set the boolean into string value
        //!
        //! @param val is the source boolean value to convert from
        public bool SetBool(bool val)
        {
            if (SetString(val.ToString()))
                return true;

            return false;
        }
        //! Set the character into string value
        //!
        //! @param val is the source character value to convert from
        public bool SetSByte(sbyte val)
        {
            if (SetString(val.ToString()))
                return true;

            return false;
        }
        //! Set the short integer into string value
        //!
        //! @param val is the source short integer value to convert from
        public bool SetShort(short val)
        {
            if (SetString(val.ToString()))
                return true;

            return false;
        }
        //! Set the 32bit integer into string value
        //!
        //! @param val is the source 32bit integer value to convert from
        public bool SetInt(int val)
        {
            if (SetString(val.ToString()))
                return true;

            return false;
        }
        //! Set the 64bit integer into string value
        //!
        //! @param val is the source 64bit integer value to convert from
        public bool SetLong(long val)
        {
            if (SetString(val.ToString()))
                return true;

            return false;
        }
        //! Set the unsigned character into string value
        //!
        //! @param val is the source unsigned character value to convert from
        public bool SetByte(byte val)
        {
            if (SetString(val.ToString()))
                return true;

            return false;
        }
        //! Set the unsigned short into string value
        //!
        //! @param val is the source unsigned short value to convert from
        public bool SetUShort(ushort val)
        {
            if (SetString(val.ToString()))
                return true;

            return false;
        }
        //! Set the unsigned 32bit integer into string value
        //!
        //! @param val is the source unsigned 32bit integer value to convert from
        public bool SetUInt(uint val)
        {
            if (SetString(val.ToString()))
                return true;

            return false;
        }
        //! Set the unsigned 64bit integer into string value
        //!
        //! @param val is the source unsigned 64bit integer value to convert from
        public bool SetULong(ulong val)
        {
            if (SetString(val.ToString()))
                return true;

            return false;
        }
        //! Set the float into string value
        //!
        //! @param val is the source float value to convert from
        public bool SetFloat(float val)
        {
            if (SetString(val.ToString()))
                return true;

            return false;
        }
        //! Set the double into string value
        //!
        //! @param val is the source double value to convert from
        public bool SetDouble(double val)
        {
            if (SetString(val.ToString()))
                return true;

            return false;
        }
        //! Set the ascii string into string value
	    //!
	    //! @param val is the source ascii string value to set from
	    public bool SetString(string val)
        {
            if (m_Doc == null || m_Node == null)
                throw new System.InvalidOperationException("Invalid element");

	        bool bExists = false;
	        string wstrValue = null;
	        GetAttributeAt(m_strAttrName, out wstrValue, out bExists);
	        if(false==bExists)
		        Create(null);

		    XmlAttributeCollection attrList = m_Node.Attributes;

		    if(attrList!=null)
		    {
			    for(int i=0; i<attrList.Count; ++i)
			    {
				    string name = attrList.Item(i).Name;
				    if(m_strAttrName==name)
				    {
					    attrList.Item(i).InnerText = val;
					    return true;
				    }
			    }

			    XmlAttribute pAttr = m_Doc.CreateAttribute(m_strAttrName);

			    if(attrList!=null&&pAttr!=null)
			    {
				    pAttr.InnerText = val;
				    attrList.SetNamedItem(pAttr);
                    return true;
			    }
		    }

	        return false;
        }

	    //! Set the GUID struct into string value
	    //!
	    //! @param val is the source GUID struct value to set from
	    public bool SetGuid(Guid val, bool bRemoveBraces)
        {
            string strDest;
            if (bRemoveBraces)
                strDest = val.ToString("D");
            else
                strDest = val.ToString("B");

            if (SetString(strDest))
                return true;

            return false;
        }
        //! Set the Date into string value
	    //!
	    //! @param val is the source Date struct value to set from
	    public bool SetDate(DateTime val)
        {
            string strDest = val.ToString("yyyy'-'MM'-'dd");
            if (SetString(strDest))
                return true;

            return false;
        }
	    //! Set the DateAndTime into string value
	    //!
	    //! @param val is the source DateAndTime object value to set from
	    public bool SetDateTime(DateTime val)
        {
           	string strDest = val.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
	        if(SetString(strDest))
		        return true;

	        return false;
        }
        //! Set the unsigned integer into hexadecimal string value
        //!
        //! @param val is the source unsigned integer value to set from
        //! @param bAddPrefix indicates whether to add the "0x" prefix
        public bool SetHex(uint val, bool bAddPrefix)
        {
            String strVal = null;
            if (bAddPrefix)
                strVal = String.Format("0x{0:X}", val);
            else
                strVal = String.Format("{0:X}", val);

            if (SetString(strVal))
                return true;

            return false;
        }
        #endregion
        #region Data Getters 
        //! Convert the string value into boolean if successful. If not successful, will use the default value
	    //! ("true" or "yes" or "1" or "ok" get true value 
	    //! while "false" or "no" or "0" or "cancel" get false value)
	    //!
	    //! @param defaultVal is the default boolean value to use if src is invalid or empty
	    public bool GetBool(bool defaultVal)
        {
           	string src = null;
	        if(false==GetString(string.Empty, out src))
		        return defaultVal;

	        bool val = defaultVal;
	        if(src.Length<=0)
		        return val;

	        string src2 = src.ToLower();

	        if(src2=="true"||src2=="yes"||src2=="1"||src2=="ok")
		        val = true;
	        else if(src2=="false"||src2=="no"||src2=="0"||src2=="cancel")
		        val = false;
	        else
		        val = defaultVal;

	        return val;
        }
	    //! Convert the string value into character if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default character value to use if src is invalid or empty
	    public sbyte GetSByte(sbyte defaultVal)
        {
           	string src = null;
	        if(false==GetString(string.Empty, out src))
		        return defaultVal;

	        sbyte val = defaultVal;
	        
            try
            {
                val = sbyte.Parse(src);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

	        return val;
        }
	    //! Convert the string value into short integer if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default short integer value to use if src is invalid or empty
	    public short GetShort(short defaultVal)
        {
           	string src = null;
	        if(false==GetString(string.Empty, out src))
		        return defaultVal;

	        short val = defaultVal;
	        
            try
            {
                val = short.Parse(src);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

	        return val;
        }
	    //! Convert the string value into 32bit integer if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default 32bit integer value to use if src is invalid or empty
	    public int GetInt(int defaultVal)
        {
            string src = null;
            if (false == GetString(string.Empty, out src))
                return defaultVal;

            int val = defaultVal;

            try
            {
                val = int.Parse(src);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

            return val;
        }
	    //! Convert the string value into 64bit integer if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default 64bit integer value to use if src is invalid or empty
	    public long GetLong(long defaultVal)
        {
            string src = null;
            if (false == GetString(string.Empty, out src))
                return defaultVal;

            long val = defaultVal;

            try
            {
                val = long.Parse(src);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

            return val;
        }
	    //! Convert the string value into unsigned character if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default unsigned character value to use if src is invalid or empty
	    public byte GetByte(byte defaultVal)
        {
            string src = null;
            if (false == GetString(string.Empty, out src))
                return defaultVal;

            byte val = defaultVal;

            try
            {
                val = byte.Parse(src);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

            return val;
        }
	    //! Convert the string value into unsigned short if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default unsigned short value to use if src is invalid or empty
	    public ushort GetUShort(ushort defaultVal)
        {
            string src = null;
            if (false == GetString(string.Empty, out src))
                return defaultVal;

            ushort val = defaultVal;

            try
            {
                val = ushort.Parse(src);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

            return val;
        }
	    //! Convert the string value into unsigned 32bit integer if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default unsigned 32bit integer value to use if src is invalid or empty
	    public uint GetUInt(uint defaultVal)
        {
            string src = null;
            if (false == GetString(string.Empty, out src))
                return defaultVal;

            uint val = defaultVal;

            try
            {
                val = uint.Parse(src);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

            return val;
        }
	    //! Convert the string value into unsigned 64bit integer if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default unsigned 64bit integer value to use if src is invalid or empty
	    public ulong GetULong(ulong defaultVal)
        {
            string src = null;
            if (false == GetString(string.Empty, out src))
                return defaultVal;

            ulong val = defaultVal;

            try
            {
                val = ulong.Parse(src);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

            return val;
        }
	    //! Convert the string value into float if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default float value to use if src is invalid or empty
	    public float GetFloat(float defaultVal)
        {
            string src = null;
            if (false == GetString(string.Empty, out src))
                return defaultVal;

            float val = defaultVal;

            try
            {
                val = float.Parse(src);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

            return val;
        }
	    //! Convert the string value into double if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default double value to use if src is invalid or empty
	    public double GetDouble(double defaultVal)
        {
            string src = null;
            if (false == GetString(string.Empty, out src))
                return defaultVal;

            double val = defaultVal;

            try
            {
                val = double.Parse(src);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

            return val;
        }
	    //! Convert the string value into ascii string if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default ascii string value to use if src is invalid or empty
	    public string GetString(string defaultVal)
        {
           	string src = null;
	        if(false==GetString(string.Empty, out src))
		        return defaultVal;

	        return src;
        }
	    //! Convert the string value into GUID if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default GUID string value to use if src is invalid or empty
	    public Guid GetGuid(Guid defaultVal)
        {
           	string src = null;
	        if(false==GetString(string.Empty, out src))
		        return defaultVal;

	        Guid val = defaultVal;
	        
            try
            {
                val = new Guid(src);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

	        return val;
        }
	    //! Convert the string value into Date object if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default Date value to use if src is invalid or empty
	    public DateTime GetDate(DateTime defaultVal)
        {
            string src = null;
            if (false == GetString(string.Empty, out src))
                return defaultVal;

            DateTime val = defaultVal;

            try
            {
                val = DateTime.Parse(src);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

            return val;
        }
	    //! Convert the string value into DateAndTime object if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default DateAndTime value to use if src is invalid or empty
	    public DateTime GetDateTime(DateTime defaultVal)
        {
           	string src = null;
	        if(false==GetString(string.Empty, out src))
		        return defaultVal;

	        DateTime val = defaultVal;
	        
            try
            {
                val = DateTime.Parse(src);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

	        return val;
        }
   	    //! Get the attribute value
	    //!
	    //! @param defaultVal is the default string value to use if src is invalid or empty
	    //! @param val is the string value to be returned
	    private bool GetString(string defaultVal, out string val)
        {
            if(m_Doc==null||m_Node==null)
                throw new InvalidOperationException("Invalid element!");

            string wstrValue = null;
	        bool bExists = false;
	        GetAttributeAt(m_strAttrName, out wstrValue, out bExists);
	        if(false==bExists||string.IsNullOrEmpty(wstrValue))
	        {
		        val = defaultVal;
		        return false;
	        }
	            
            if (string.IsNullOrEmpty(wstrValue))
            {
                val = defaultVal;
                return false;
            }

            val = wstrValue;
            return true;
        }
        //! Convert the hexadecimal string into unsigned integer if successful. If not successful, will use the default value
        //!
        //! @param defaultVal is the default unsigned integer value to use if src is invalid or empty
        public uint ReadHex(uint defaultVal)
        {
            string src = null;
            if (false == GetString(string.Empty, out src))
                return defaultVal;

            uint val = defaultVal;

            if (src.Length > 1 && src[0] == '0' && (src[1] == 'X' || src[1] == 'x'))
            {
                string str2 = string.Empty;
                for (int i = 2; i < src.Length; ++i)
                    str2 += src[i];

                src = str2;
            }

            try
            {
                val = Convert.ToUInt32(src, 16);
            }
            catch (System.Exception)
            {
                val = defaultVal;
            }

            return val;
        }
        #endregion
        #region Private methods
        //! Get attribute with this name
	    //!
	    //! @param wstrName is attribute name to get
	    //! @param wstrValue is the attribute value 
	    //! @param bExists states if this attribute exists
	    private bool GetAttributeAt(string wstrAttrName, out string wstrValue, out bool bExists)
        {
	        bExists = false;
            wstrValue = string.Empty;
	        if(m_Node!=null)
	        {
		        XmlAttributeCollection attrList = m_Node.Attributes;

		        if(attrList!=null)
		        {
			        for(int i=0; i<attrList.Count; ++i)
			        {
				        string name = attrList.Item(i).Name;
				        if(wstrAttrName==name)
				        {
					        wstrValue = attrList.Item(i).InnerText;
					        bExists = true;
					        return true;
				        }
			        }
		        }
	        }

	        return false;
        }
        #endregion

        #region Member Variables
        //! MS XML document object
	    private XmlDocument m_Doc;
	    //! MS XML node object
        private XmlNode m_Node;
	    //! Attribute name
        private string m_strAttrName;
        #endregion
    }
}
