using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Elmax
{
    public class Document
    {
        public Document(XmlDocument doc) 
        {
            m_Doc = doc;
        }
	    public List<Elmax.Element> GetElementsByTagName(string tagName)
        {
            if (m_Doc == null)
                throw new System.InvalidOperationException("Invalid document");

            List<Elmax.Element> list = new List<Elmax.Element>();

            XmlNodeList pList = m_Doc.GetElementsByTagName(tagName);

            ConvNodeListToList(pList, ref list);

            return list;
        }
        public List<Elmax.Element> GetElementsByTagName(string tagName, string namespaceURI)
        {
            if (m_Doc == null)
                throw new System.InvalidOperationException("Invalid document");

            List<Elmax.Element> list = new List<Elmax.Element>();

            XmlNodeList pList = m_Doc.GetElementsByTagName(tagName, namespaceURI);

            ConvNodeListToList(pList, ref list);

            return list;
        }
        public Elmax.Element GetElementById(string id)
        {
            if (m_Doc == null)
                throw new System.InvalidOperationException("Invalid document");

            XmlNode node = m_Doc.GetElementById(id);

            if(node!=null)
                return new Element(m_Doc, node, "", node.Name, true, false);

            return new Element();
        }
	    public List<Elmax.Element> SelectNodes(string xpath)
        {
            if (m_Doc == null)
                throw new System.InvalidOperationException("Invalid document");
            
            List<Elmax.Element> list = new List<Elmax.Element>();

	        XmlNodeList pList = m_Doc.SelectNodes(xpath);

	        ConvNodeListToList(pList, ref list);

	        return list;
        }
	    public Elmax.Element SelectSingleNode(string xpath)
        {
            if (m_Doc == null)
                throw new System.InvalidOperationException("Invalid document");

	        XmlNode node = m_Doc.SelectSingleNode(xpath);

            if (node != null)
            {
                return new Element(m_Doc, node, "", node.Name, true, false);
            }

            return new Element();
        }
        private void ConvNodeListToList(XmlNodeList pList, ref List<Elmax.Element> list)
        {
	        if(pList==null)
		        return;

	        for(int i=0; i<pList.Count; ++i)
	        {
		        Element ele = new Element(m_Doc, pList[i], "", pList[i].Name, true, false );
		        list.Add(ele);
	        }
        }

        private XmlDocument m_Doc;
    }
}
