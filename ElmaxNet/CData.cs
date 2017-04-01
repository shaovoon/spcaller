using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Elmax
{
    public class CData
    {
        //! Constructor
        public CData()
        {
            m_CData = null;
        }
	    //! Non-default Constructor
        public CData(XmlCDataSection cdata)
        {
            m_CData = cdata;
        }

	    //! Get the CDataSection data
        public string GetData()
        {
	        if(m_CData==null)
                throw new System.InvalidOperationException("Invalid CData object");

	        return m_CData.Data;
        }
	    //! Get the length of the CDataSection data in wchar_t size
        public long GetLength()
        {
	        if(m_CData==null)
                throw new System.InvalidOperationException("Invalid CData object");

	        return m_CData.Length;
        }
	    //! Delete the CDataSection
        public bool Delete()
        {
	        if(m_CData==null)
                throw new System.InvalidOperationException("Invalid CData object");

	        XmlNode parent = m_CData.ParentNode;
	        if(parent!=null)
	        {
		        m_CData = (XmlCDataSection)(parent.RemoveChild(m_CData));
	        }
	        else
		        return false;

	        return true;
        }

	    //! Update the CDataSection
        public bool Update(string data)
        {
	        if(m_CData==null)
                throw new System.InvalidOperationException("Invalid CData object");

	        m_CData.ReplaceData(0, data.Length, data);
	        return true;
        }
        public XmlCDataSection GetInternalObject()
        {
            return m_CData;
        }
        public bool IsValid()
        {
            if (m_CData == null)
                return false;

            return true;
        }
        //! CDataSection object
        private XmlCDataSection m_CData;

    }
}
