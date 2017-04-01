using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Elmax
{
    public class Element
    {
#region Constructors and Destructors
        public Element()
        {
            m_Doc = null;
            m_Node = null;

            m_strNonExistingParent = string.Empty;
            m_bDeleted = false;
            m_bValid = true;
            m_bRoot = false;
        }
        // Non-default constructor
        public Element(
	        XmlDocument doc, 
	        XmlNode node, 
	        string wstrNonExistingParent, 
	        string wstrName, 
	        bool bValid,
	        bool bRoot)
            {
                m_strNonExistingParent = wstrNonExistingParent;
                m_Doc = doc;
                m_Node = node;
                m_bDeleted = false;
                m_strName = wstrName;
                m_bValid = bValid;
                m_bRoot = bRoot;
            }
        // Copy constructor
        public Element(Element other)
        {
            m_strNonExistingParent = other.m_strNonExistingParent;
            m_Doc = other.m_Doc;
            m_Node = other.m_Node;
            m_bDeleted = other.m_bDeleted;
            m_strName = other.m_strName;
            m_bValid = other.m_bValid;
            m_Attribute = other.m_Attribute;
            m_bRoot = other.m_bRoot;
        }
        public void Assign(Element other)
        {
            this.m_strNonExistingParent = other.m_strNonExistingParent;
            this.m_Doc = other.m_Doc;
            this.m_Node = other.m_Node;
            this.m_bDeleted = other.m_bDeleted;
            this.m_strName = other.m_strName;
            this.m_bValid = other.m_bValid;
            this.m_Attribute = other.m_Attribute;
            this.m_bRoot = other.m_bRoot;
        }
        
#endregion
#region Document and Node Getter and Setter Methods
        //! Set the internal document object
	    public void SetDomDoc(XmlDocument doc)
        {
            m_Doc = doc;
            m_bRoot = true;
	        if(m_Node==null&&m_Doc.DocumentElement!=null)
	        {
		        m_Node = (XmlNode)(m_Doc.DocumentElement);
		        if(m_Node!=null)
		        {
			        m_strName = m_Node.Name;
		        }
	        }
        }
	    //! Get the internal document object
	    public XmlDocument GetDomDoc()
        {
            return m_Doc;
        }
	    //! Set the internal node object
	    public void SetNode(XmlNode node)
        {
            m_Node = node;
        }
	    //! Get the internal node object
	    public XmlNode GetNode()
        {
            return m_Node;
        }
        // Indexer implementation.
        public Element this[string name]
        {
            get
            {
                return GetNodeAt(name);
            }
        }
        // Query node by XPath
        public Element SelectSingleNode(string szXPath)
        {
	        if(m_Node!=null&&m_bValid&&string.IsNullOrEmpty(m_strNonExistingParent))
	        {
		        XmlNode pNode = m_Node.SelectSingleNode(szXPath);

		        if(pNode!=null)
		        {
			        return new Element(m_Doc, pNode, "", pNode.Name, true, false);
		        }
	        }

            return new Element();
        }
        // Query nodes by XPath
        public List<Element> SelectNodes(string szXPath)
        {
            List<Element> list = new List<Element>();
            if (m_Node != null && m_bValid && string.IsNullOrEmpty(m_strNonExistingParent))
            {
                XmlNodeList pList = m_Node.SelectNodes(szXPath);

                if (pList != null)
                {
                    for (int i = 0; i < pList.Count; ++i)
                    {
                        Element ele = new Element(m_Doc, pList[i], "", pList[i].Name, true, false);
                        list.Add(ele);
                    }
                }
            }
            return list;
        }
#endregion

#region Misc Methods
	    //! Get the root name (to access the root)
	    public string GetRootName()
        {
           	if(m_Doc==null)
		        return string.Empty;

	        if(m_Doc.DocumentElement==null)
		        return string.Empty;

	        return m_Doc.DocumentElement.Name;
        }
	    //! Returns true if the attribute with the name exists.
	    public bool Exists
        {
            get
            {
           	    if(m_Node==null)
		            return false;
	            else if(false==m_bValid)
		            return false;
	            else if(m_strNonExistingParent.Length>0)
		            return false;

                return true;
            }
        }
        public string Name
        {
            get
            {
                return m_strName;
            }
        }
#endregion

#region Node Creation, Adding and Removing Methods
        //! Create this element with this optional namespaceUri, if not exists
	    public Element Create(string namespaceUri)
        {
           	ResolveNode(m_strName);
	        if(m_Node!=null)
	        {
		        if(string.IsNullOrEmpty(m_strNonExistingParent) && m_strName==m_Node.Name && m_bValid)
			        return this;
	        }

	        bool bMultipleParent = false;
	        List<string> vec;
	        string wstrNonExistingParent = m_strNonExistingParent;
	        wstrNonExistingParent += "|";

	        wstrNonExistingParent += m_strName;
	        if(wstrNonExistingParent=="|")
		        wstrNonExistingParent=string.Empty;

	        SplitString(wstrNonExistingParent, out vec, out bMultipleParent);

	        string namespaceUriTemp = string.Empty;
	        if(false==string.IsNullOrEmpty(wstrNonExistingParent))
	        {
		        if(m_Doc!=null&&m_Node!=null)
		        {
			        for(int i=0; i<vec.Count; ++i)
			        {
				        if(i==vec.Count-1)
					        namespaceUriTemp = namespaceUri;
				        XmlNode pNew = m_Doc.CreateNode(XmlNodeType.Element, vec[i], namespaceUriTemp);

				        if(pNew!=null)
				        {
					        m_Node = m_Node.AppendChild(pNew);
					        m_bValid = true;
				        }
			        }
			        m_bDeleted = false;
			        m_strNonExistingParent = string.Empty;
		        }
		        else if(m_Doc!=null&&m_Node==null)
		        {
			        for(int i=0; i<vec.Count; ++i)
			        {
				        if(i==vec.Count-1)
					        namespaceUriTemp = namespaceUri;
				        if(m_Node==null)
				        {
					        if(m_Doc.DocumentElement==null)
					        {
						        m_Node = m_Doc.CreateNode(XmlNodeType.Element, vec[i], namespaceUriTemp);
						        //m_Doc.DocumentElement = (XmlElement)(m_Node); // cannot
						        m_bValid = true;
					        }
					        else
					        {
						        m_Node = m_Doc.DocumentElement;
						        m_bValid = true;
					        }
				        }
				        else
				        {
					        XmlNode pNew = m_Doc.CreateNode(XmlNodeType.Element, vec[i], namespaceUri);

					        if(pNew!=null&&m_Node!=null)
					        {
						        m_Node = m_Node.AppendChild(pNew);
						        m_bValid = true;
					        }
				        }

			        }
			        m_bDeleted = false;
			        m_strNonExistingParent = string.Empty;
		        }
		        else
			        throw new InvalidOperationException("No valid xml document and node in this element!");
	        }
	        else // if(wstrNonExistingParent.empty())
	        {
		        if(m_Doc!=null&&m_Node!=null)
		        {
			        if(m_strName==m_Node.Name)
				        return this;
			        else
			        {
				        XmlNode pNew = m_Doc.CreateNode(XmlNodeType.Element, m_strName, namespaceUri);

				        if(pNew!=null)
				        {
					        m_Node = m_Node.AppendChild(pNew);
					        m_bValid = true;
					        m_bDeleted = false;
					        m_strNonExistingParent = string.Empty;
				        }
			        }
		        }
		        else if(m_Doc!=null&&m_Node==null)
		        {
			        if(m_strName==m_Node.Name)
				        return this;
			        else
			        {
				        if(m_Node==null)
				        {
					        if(m_Doc.DocumentElement==null)
					        {
						        m_Node = m_Doc.CreateNode(XmlNodeType.Element, m_strName, namespaceUri);
						        //m_Doc.DocumentElement = (XmlElement)(m_Node); // cannot
						        m_bValid = true;
					        }
					        else
					        {
						        m_Node = m_Doc.DocumentElement;
						        m_bValid = true;
					        }
				        }
				        else
				        {
					        XmlNode pNew = m_Doc.CreateNode(XmlNodeType.Element, m_strName, namespaceUri);

					        if(pNew!=null&&m_Node!=null)
					        {
						        m_Node = m_Node.AppendChild(pNew);
						        m_bValid = true;
					        }
				        }

			        }
			        m_bDeleted = false;
			        m_strNonExistingParent = string.Empty;
		        }
		        else
			        throw new InvalidOperationException("No valid xml document and node in this element!");
        	}

	        return this;
        }
	    //! Always create this element with this optional namespaceUri
	    public Element CreateNew(string namespaceUri)
        {
           	ResolveNode(m_strName);
	        if(false==String.IsNullOrEmpty(m_strNonExistingParent)||false==m_bValid)
	        {
		        return Create(namespaceUri);
	        }
	        else // if(m_strNonExistingParent.empty())
	        {
		        if(m_Doc!=null&&m_Node!=null)
		        {
			        XmlNode pNew = m_Doc.CreateNode(XmlNodeType.Element, m_Node.Name, namespaceUri);

			        XmlNode parent = m_Node.ParentNode;
			        if(parent!=null)
			        {
                        if (parent != m_Doc || (parent == m_Doc && m_Doc.DocumentElement == null))
                        {
                            m_Node = parent.AppendChild(pNew);
                            m_bDeleted = false;
                            m_bValid = true;

                            return this;
                        }

                        return new Element();
                    }
			        else
				        throw new InvalidOperationException("No valid parent found!");
		        }
		        else if(m_Doc!=null&&m_Node==null)
		        {
			        if(m_Doc.DocumentElement==null)
			        {
				        m_Node = m_Doc.CreateNode(XmlNodeType.Element, m_strName, namespaceUri);
				        //m_Doc.DocumentElement = (XmlElement)(m_Node); // cannot
				        m_bValid = true;
			        }
			        else
			        {
				        m_Node = m_Doc.DocumentElement;
				        m_bValid = true;
			        }

			        m_bDeleted = false;
			        return this;
		        }
		        else
			        throw new InvalidOperationException("No valid xml document and node in this element!");
	        }

	        // will not come here
	        //throw new InvalidOperationException("No valid xml document and node in this element!");
	        //return new Element();
        }
	    //! Add this node as child node
	    public bool AddNode(Element node)
        {
           	if(false==string.IsNullOrEmpty(m_strNonExistingParent)||false==m_bValid)
	        {
		        throw new InvalidOperationException("Invalid element");
	        }

	        if(m_Doc!=null&&m_Node!=null)
	        {
		        if(node.m_Node!=null)
		        {
			        node.m_Node = m_Node.AppendChild(node.m_Node);
			        node.m_Doc = m_Doc;
			        node.m_bDeleted = false;
			        return true;
		        }
		        else
			        return false;
	        }

	        throw new InvalidOperationException("No valid xml document and node in this element!");

	        //return false;
        }
	    //! Delete this child node
	    public bool RemoveNode(Element node)
        {
           	if(false==string.IsNullOrEmpty(m_strNonExistingParent)||false==m_bValid||node.m_bDeleted)
	        {
		        throw new InvalidOperationException("Invalid element");
	        }

	        if(m_Doc!=null&&m_Node!=null)
	        {
		        if(node.m_Node!=null)
		        {
			        node.m_Node = m_Node.RemoveChild(node.m_Node);
			        node.m_Doc = null;
			        node.m_bDeleted = true;
			        return true;
		        }
		        else
			        throw new InvalidOperationException("Invalid child node!");
	        }

	        throw new InvalidOperationException("No valid xml document and node in this element!");

	        //return false;
        }
	    //! Delete this node
	    public bool RemoveNode()
        {
            if(false==string.IsNullOrEmpty(m_strNonExistingParent)||false==m_bValid||m_bDeleted)
	        {
		        throw new InvalidOperationException("Invalid element");
	        }

	        if(m_Doc!=null&&m_Node!=null)
	        {
		        XmlNode parent = m_Node.ParentNode;
		        if(parent!=null)
		        {
			        this.m_Node = parent.RemoveChild(this.m_Node);
			        this.m_Doc = null;
			        this.m_bDeleted = true;
			        return true;
		        }
		        else
			        throw new InvalidOperationException("No valid parent!");
	        }
        	
	        throw new InvalidOperationException("No valid xml document and node in this element!");
	        //return false;
        }
#endregion

#region Children and Sibling Access Methods
	    //! Get the collection of sibling elements with the same name
	    public List<Element> AsCollection()
        {
            if (m_Node == null || false == string.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
                throw new System.InvalidOperationException("Invalid Element");

            List<Element> vec = new List<Element>();
            ResolveNullNode(m_strName);
            XmlNode parent = m_Node.ParentNode;
            if (parent == null)
                return vec;
            XmlNodeList pList = parent.ChildNodes;

            for (int i = 0; i < pList.Count; ++i)
            {
                if (pList.Item(i).NodeType == XmlNodeType.Element)
                {
                    string name = pList.Item(i).Name;

                    if (name == m_Node.Name)
                    {
                        Element ele = new Element(m_Doc, pList.Item(i), string.Empty, m_strName, true, false);
                        vec.Add(ele);
                    }
                }
            }
            
            return vec;
        }

        //! Get the collection of sibling elements with the same name
        public List<Element> AsCollection(Predicate<Element> pred)
        {
            if (m_Node == null || false == string.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
                throw new System.InvalidOperationException("Invalid Element");

            List<Element> vec = new List<Element>();
            ResolveNullNode(m_strName);
            XmlNode parent = m_Node.ParentNode;
            if (parent == null)
                return vec;
            XmlNodeList pList = parent.ChildNodes;

            for (int i = 0; i < pList.Count; ++i)
            {
                if (pList.Item(i).NodeType == XmlNodeType.Element)
                {
                    string name = pList.Item(i).Name;

                    if (name == m_Node.Name)
                    {
                        Element ele = new Element(m_Doc, pList.Item(i), string.Empty, m_strName, true, false);
                        if(pred(ele))
                            vec.Add(ele);
                    }
                }
            }

            return vec;
        }

	    //! Get the collection of child elements with same name in vec
	    public List<Element> GetCollection(string name)
        {
            if (m_Node == null || false == string.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
                throw new System.InvalidOperationException("Invalid Element");
            
            List<Element> vec = new List<Element>();
            ResolveNullNode(m_strName);
            XmlNodeList pList = m_Node.ChildNodes;

            if (pList != null)
            {
                for (int i = 0; i < pList.Count; ++i)
                {
                    if (pList.Item(i).NodeType == XmlNodeType.Element)
                    {
                        string nodename = pList.Item(i).Name;

                        if (nodename == name)
                        {
                            Element ele = new Element(m_Doc, pList.Item(i), string.Empty, name, true, false);
                            vec.Add(ele);
                        }
                    }
                }
            }
            
            return vec;
        }

        //! Get the collection of child elements with same name in vec
	    public List<Element> GetCollection(string name, Predicate<Element> pred)
        {
            if (m_Node == null || false == string.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
                throw new System.InvalidOperationException("Invalid Element");
            
            List<Element> vec = new List<Element>();
            ResolveNullNode(m_strName);
            XmlNodeList pList = m_Node.ChildNodes;

            if (pList != null)
            {
                for (int i = 0; i < pList.Count; ++i)
                {
                    if (pList.Item(i).NodeType == XmlNodeType.Element)
                    {
                        string nodename = pList.Item(i).Name;

                        if (nodename == name)
                        {
                            Element ele = new Element(m_Doc, pList.Item(i), string.Empty, name, true, false);
                            if(pred(ele))
                                vec.Add(ele);
                        }
                    }
                }
            }
            
            return vec;
        }

	    //! Query number of children for each Element name
	    public Dictionary<string, UInt32> QueryChildrenNum()
        {
            if (m_Node == null || false == string.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
                throw new System.InvalidOperationException("Invalid Element");

           	Dictionary<string, UInt32> children = new Dictionary<string, UInt32>();
            XmlNodeList pList = m_Node.ChildNodes;

            for (int i = 0; i < pList.Count; ++i)
            {
                if (pList.Item(i).NodeType == XmlNodeType.Element)
                {
                    string nodename = pList.Item(i).Name;

                    uint value = 0;
                    if (children.TryGetValue(nodename, out value))
                    {
                        ++value;
                        children[nodename] = value;
                    }
                    else
                        children[nodename] = 1;
                }
            }

            return children;
        }
#endregion

#region CData Access Methods
	    //! Add this CDataSection with data
	    public bool AddCData(string data)
        {
            if (m_Node == null || false == string.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
	        {
                throw new System.InvalidOperationException("Invalid Element");
	        }
	        else // if(m_strNonExistingParent.empty())
	        {
		        if(m_Doc!=null&&m_Node!=null)
		        {
			        XmlNode pNew = m_Doc.CreateNode(XmlNodeType.CDATA, data, string.Empty);
			        pNew.Value = data;
			        m_Node.AppendChild(pNew);
			        return true;
		        }
	        }

	        return false;
        }

	    //! Delete this CDataSection at this index
	    public bool DeleteAllCData()
        {
	        List<CData> list = GetCDataCollection();

            if (list.Count == 0)
                return false;

            for (int i = 0; i < list.Count; ++i)
            {
                list[i].Delete();
            }

	        return true;
        }
	    //! Get this CDataSection collection
	    public List<CData> GetCDataCollection()
        {
            List<CData> list = new List<CData>();
            if (m_Node == null || false == string.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
                throw new System.InvalidOperationException("Invalid Element");

            XmlNodeList pList = m_Node.ChildNodes;
            if (pList != null)
            {
                int len = pList.Count;
                for (int j = 0; j < len; ++j)
                {
                    XmlNode pNode = pList.Item(j);
                    if (pNode != null && pNode.NodeType == XmlNodeType.CDATA)
                    {
                        list.Add(new CData((XmlCDataSection)(pNode)));
                    }
                }
            }

	        return list;
        }
#endregion

#region Comment Access Methods
	    //! Add this Comment object with this comment string
	    public bool AddComment(string comment)
        {
            if (m_Node == null || false == string.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
	        {
		        throw new System.InvalidOperationException("Invalid Element");
	        }
	        else // if(m_strNonExistingParent.empty())
	        {
		        if(m_Doc!=null&&m_Node!=null)
		        {
			        XmlNode pNew = m_Doc.CreateNode(XmlNodeType.Comment, comment, string.Empty);
			        if(pNew!=null)
			        {
                        pNew.InnerText = comment;
				        m_Node.AppendChild(pNew);
                        return true;
                    }
		        }
	        }

	        return false;
        }
	    //! Delete this Comment at this index
	    public bool DeleteAllComments()
        {
	        List<Comment> list = GetCommentCollection();

            if (list.Count == 0)
                return false;

            for (int i = 0; i < list.Count; ++i)
            {
                list[i].Delete();
            }

	        return true;
        }
	    //! Get this Comment collection
	    public List<Comment> GetCommentCollection()
        {
	        List<Comment> list = new List<Comment>();
            if (m_Node == null || false == string.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
                throw new System.InvalidOperationException("Invalid Element");

            XmlNodeList pList = m_Node.ChildNodes;
            if (pList != null)
            {
                int len = pList.Count;
                for (int j = 0; j < len; ++j)
                {
                    XmlNode pNode = pList.Item(j);
                    if (pNode != null && pNode.NodeType == XmlNodeType.Comment)
                    {
                        list.Add(new Comment((XmlComment)(pNode)));
                    }
                }
            }

            return list;
        }
#endregion

#region Base64 Conversion
	    //==========================
	    //! Convert pSrc to Base64 format
	    //public static string ConvToBase64(byte[] pSrc);
	    //! Convert pSrc to wstrBase64Dest in Base64 format
	    //public static bool ConvToBase64(byte[] pSrc, out string wstrBase64Dest);
	    //! Convert wstrBase64Src from Base64 format to pDest
	    //public static bool ConvFromBase64(string wstrBase64Src, out byte[] pDest);
#endregion

#region Attribute Methods
	    //! Get the attribute with this attrName
	    public Elmax.Attribute Attribute(string attrName)
        {
            Elmax.Attribute attr = new Elmax.Attribute();

            if (m_Node == null || false == String.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
                return attr;

            //if (false == String.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
            //{
            //    Element ele = CreateNew(null);
            //    Assign(ele);
            //}

            attr.SetParam(m_Doc, m_Node, attrName);
	        return attr;
        }
	    //! Get a list of attribute names.
	    public List<string> GetAttributeNames()
        {
   	        List<string> vec = new List<string>();

            if (m_Node == null || false == string.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
                return vec;

	        if(m_Doc!=null&&m_Node!=null)
	        {
		        XmlAttributeCollection attrList = m_Node.Attributes;

		        if(attrList!=null)
		        {
			        for(int i=0; i<attrList.Count; ++i)
			        {
				        string name = attrList.Item(i).Name;
				        vec.Add(name);
			        }
		        }
	        }

	        return vec;
        }
        public List<Elmax.Attribute> GetAttributes()
        {
	        List<Elmax.Attribute> list = new List<Elmax.Attribute>();

            if (m_Node == null || false == string.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
                return list;

	        if(m_Doc!=null&&m_Node!=null)
	        {
		        XmlAttributeCollection attrList = m_Node.Attributes;

		        if(attrList!=null)
		        {
			        for(int i=0; i<attrList.Count; ++i)
			        {
				        Elmax.Attribute attr = new Elmax.Attribute();
				        attr.SetParam(m_Doc, m_Node, attrList[i].Name );
				        list.Add(attr);
			        }
		        }
	        }

	        return list;
        }
#endregion

#region Data Setters
        //! Set the boolean into string value
        //!
        //! @param val is the source boolean value to convert from
        public bool SetBool(bool val)
        {
	        if(SetString(val.ToString()))
		        return true;

	        return false;
        }
        //! Set the character into string value
        //!
        //! @param val is the source character value to convert from
        public bool SetSByte(sbyte val)
        {
	        if(SetString(val.ToString()))
		        return true;

	        return false;
        }
        //! Set the short integer into string value
        //!
        //! @param val is the source short integer value to convert from
        public bool SetShort(short val)
        {
	        if(SetString(val.ToString()))
		        return true;

	        return false;
        }
        //! Set the 32bit integer into string value
        //!
        //! @param val is the source 32bit integer value to convert from
        public bool SetInt(int val)
        {
	        if(SetString(val.ToString()))
		        return true;

	        return false;
        }
        //! Set the 64bit integer into string value
        //!
        //! @param val is the source 64bit integer value to convert from
        public bool SetLong(long val)
        {
	        if(SetString(val.ToString()))
		        return true;

	        return false;
        }
        //! Set the unsigned character into string value
        //!
        //! @param val is the source unsigned character value to convert from
        public bool SetByte(byte val)
        {
	        if(SetString(val.ToString()))
		        return true;

	        return false;
        }
        //! Set the unsigned short into string value
        //!
        //! @param val is the source unsigned short value to convert from
        public bool SetUShort(ushort val)
        {
	        if(SetString(val.ToString()))
		        return true;

	        return false;
        }
        //! Set the unsigned 32bit integer into string value
        //!
        //! @param val is the source unsigned 32bit integer value to convert from
        public bool SetUInt(uint val)
        {
	        if(SetString(val.ToString()))
		        return true;

	        return false;
        }
        //! Set the unsigned 64bit integer into string value
        //!
        //! @param val is the source unsigned 64bit integer value to convert from
        public bool SetULong(ulong val)
        {
	        if(SetString(val.ToString()))
		        return true;

	        return false;
        }
        //! Set the float into string value
        //!
        //! @param val is the source float value to convert from
        public bool SetFloat(float val)
        {
	        if(SetString(val.ToString()))
		        return true;

	        return false;
        }
        //! Set the double into string value
        //!
        //! @param val is the source double value to convert from
        public bool SetDouble(double val)
        {
	        if(SetString(val.ToString()))
		        return true;

	        return false;
        }
       	//! Set the wide string into string value
        //!
        //! @param val is the source wide string value to set from
	    public bool SetString(string val)
        {
	        if(false==String.IsNullOrEmpty(m_strNonExistingParent)||false==m_bValid)
	        {
		        Element ele = CreateNew(null);
                Assign(ele);
            }

	        if(m_Node!=null)
	        {
		        m_Node.InnerText = val;
	        }
	        else
		        return false;

	        return true;
        }
        //! Set the GUID struct into string value
	    //!
	    //! @param val is the source GUID struct value to set from
	    public bool SetGuid(Guid val, bool bRemoveBraces)
        {
	        string strDest;
            if(bRemoveBraces)
                strDest = val.ToString("D");
            else
                strDest = val.ToString("B");

            if(SetString(strDest))
			    return true;

	        return false;
        }

	    //! Set the Date into string value
	    //!
	    //! @param val is the source Date struct value to set from
	    public bool SetDate(DateTime val)
        {
           	string strDest = val.ToString("yyyy'-'MM'-'dd");
	        if(SetString(strDest))
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
        //! Set the file contents into Base64 string value
	    //!
	    //! @param filepath is the source file to set from
	    //! @param bSaveFilename indicates whether to save the filename in the FileName attribute
	    //! @param bSaveFileLength indicates whether to save the file length in the FileLength attribute 
	    //! because GetFileContents sometimes report a longer length which is not ideal if you need 
	    //! to save the file to disk again
	    public bool SetFileContents(string filepath, bool bSaveFilename, bool bSaveFileLength)
        {
            FileInfo fi = new FileInfo(filepath);
	        if(fi.Exists==false)
		        return false;

            byte[] buf = null;

            using (BinaryReader b = new BinaryReader(File.Open(filepath, FileMode.Open)))
            {
                int length = (int)b.BaseStream.Length;
                buf = new byte[length];
                int v = b.Read(buf, 0, length);

                string sbuf = Convert.ToBase64String(buf);

                if (sbuf != null && sbuf.Length > 0)
                {
                    if(SetString(sbuf))
	                {
		                if(bSaveFilename)
		                {
			                string filename = fi.Name;
			                Attribute("FileName").SetString(filename);
		                }
		                if(bSaveFileLength)
                            Attribute("FileLength").SetInt(sbuf.Length);
		                return true;
	                }
                }
            }

            return false;
        }
        //! Set the unsigned integer into hexadecimal string value
        //!
        //! @param val is the source unsigned integer value to set from
        //! @param bAddPrefix indicates whether to add the "0x" prefix
        public bool SetHex(uint val, bool bAddPrefix)
        {
            String strVal = null;
            if(bAddPrefix)
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
	        if(false==GetString(string.Empty, out src))
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
	        if(false==GetString(string.Empty, out src))
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
	        if(false==GetString(string.Empty, out src))
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
	        if(false==GetString(string.Empty, out src))
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
	        if(false==GetString(string.Empty, out src))
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
	        if(false==GetString(string.Empty, out src))
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
	        if(false==GetString(string.Empty, out src))
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
	        if(false==GetString(string.Empty, out src))
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
	    //! Set the string value into wide string if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default wide string value to use if src is invalid or empty
	    public string GetString(string defaultVal)
        {
           	string src = null;
	        if(false==GetString(string.Empty, out src))
		        return defaultVal;

	        return src;
        }
	    //! Convert the string value into GUID if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default GUID value to use if src is invalid or empty
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
	    //! Convert the string value into DateTime object if successful. If not successful, will use the default value
	    //!
	    //! @param defaultVal is the default DateTime value to use if src is invalid or empty
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
	    //! Convert the Base64 string value into char array (remember to delete afterwards) if successful. If not successful, will return NULL
	    //!
	    //! @param filename is the filename saved in the FileName Attribute if available
	    //! @param length is the file length of the char array returned.
	    public byte[] GetFileContents(out string filename, out int length)
        {
	        string src = null;
	        if(false==GetString("", out src))
	        {
		        length = 0;
                filename = "";
		        return null;
	        }

            byte[] p = Convert.FromBase64String(src);

            length = 0;

            if (p!=null)
                length = p.Length;
            
            filename = Attribute("FileName").GetString("");

	        return p;
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

#region Private Methods
        //! Split the str(ing) with delimiter "|/\\" into vec
	    //!
	    //! @param bMultipleParent returns true if there are more than 1 item in vec
        // 
	    private static bool SplitString(string str, out List<string> vec, out bool bMultipleParent)
        {
	        vec = new List<string>();
	        bMultipleParent = false;

	        string temp = str;
	        int size = temp.IndexOf('|');
	        if(size!=-1)
	        {
		        bMultipleParent = true;
	        }
	        size = temp.IndexOf('\\');
	        if(size!=-1)
	        {
		        bMultipleParent = true;
	        }
	        size = temp.IndexOf('/');
	        if(size!=-1)
	        {
		        bMultipleParent = true;
	        }
	        if(bMultipleParent)
	        {
                char[] spaceSeparator = new char[3];
                spaceSeparator[0] = '|';
                spaceSeparator[1] = '/';
                spaceSeparator[2] = '\\';
                string[] col = temp.Split(spaceSeparator);

                if(col!=null)
                {
                    for (int i = 0; i < col.Length; ++i)
                    {
                        if (col[i] != string.Empty)
                            vec.Add(col[i]);
                    }
		        }
	        }

	        if(vec.Count<=0&&string.IsNullOrEmpty(str)==false)
		        vec.Add(str);

	        return true;
        }
	    //! Get Element with this str name
	    private Element GetNodeAt(string str)
        {
	        ResolveNullNode(str);
	        bool bMultipleParent = false;
	        List<string> vec = null;
	        string temp = str;
	        string wstrNonExistingParent = string.Empty;
	        if(false==string.IsNullOrEmpty(m_strNonExistingParent))
	        {
		        temp = m_strNonExistingParent;
		        temp += "|";
		        if(string.IsNullOrEmpty(m_strName)==false)
		        {
			        temp += m_strName;
			        temp += "|";
		        }
		        temp += str;
	        }
	        else
	        {
		        if(m_Node!=null&&m_strName==m_Node.Name)
		        {
			        temp = str;
		        }
		        else
		        {
			        if(string.IsNullOrEmpty(m_strName)==false)
			        {
				        temp = m_strName;
				        temp += "|";
				        temp += str;
			        }
			        else
				        temp = str;
		        }
	        }
	        SplitString(temp, out vec, out bMultipleParent);

	        if(m_bRoot&&m_Node==null)
	        {
                if (m_Doc.DocumentElement == null)
                {
                    if (vec.Count > 0)
                        m_Doc.AppendChild(m_Doc.CreateElement(vec[0]));
                }
		        m_Node = m_Doc.DocumentElement;
		        if(m_Node!=null && vec.Count>0 && vec[0]== m_Node.Name)
		        {
			        m_strName = (m_Node.Name);
		        }
	        }

	        if(m_Doc!=null||m_Node!=null)
	        {
		        if(string.IsNullOrEmpty(m_strNonExistingParent))
		        {
			        if(bMultipleParent)
			        {
				        if(m_Node!=null)
				        {
					        XmlNode pSrc = m_Node;
					        bool found = false;
					        int i=0;
					        if(m_bRoot&&m_strName==m_Node.Name)
						        i = 1;
					        int nFound = i;
					        for(; i<vec.Count; ++i)
					        {
						        // Get the collection from this node
						        // If successful, assign the found node to this element
						        // and find the next element.
						        XmlNodeList pList = pSrc.ChildNodes;
						        if(pList!=null)
						        {
							        int len = pList.Count;
							        for(int j=0; j<len; ++j)
							        {
								        XmlNode pNode = pList.Item(j);
								        if(pNode!=null)
								        {
									        if(vec[i]==pNode.Name)
									        {
										        pSrc = pNode;
										        found = true;
										        ++nFound;
										        break;
									        }
								        }
							        }
							        if(false==found)
							        {
								        for(;i<vec.Count-1;++i)
								        {
									        if(string.IsNullOrEmpty(wstrNonExistingParent)==false)
										        wstrNonExistingParent += "|";

									        wstrNonExistingParent += vec[i];
								        }

								        if(vec.Count>0)
									        temp = vec[vec.Count-1];

								        bool bRoot = m_bRoot;
								        if(pSrc!=m_Node)
									        bRoot = false;

								        return new Element(m_Doc, pSrc, wstrNonExistingParent, temp, false, bRoot);
							        }
						        }
					        }
					        if(found)
					        {
						        for(;i<vec.Count-1;++i)
						        {
							        if(string.IsNullOrEmpty(wstrNonExistingParent)==false)
								        wstrNonExistingParent += "|";

							        wstrNonExistingParent += vec[i];
						        }

						        if(vec.Count>0)
							        temp = vec[vec.Count-1];

						        bool bRoot = m_bRoot;
						        if(pSrc!=m_Node)
							        bRoot = false;

						        bool bValid = false;
						        if(nFound==i)
							        bValid = true;

						        return new Element(m_Doc, pSrc, wstrNonExistingParent, temp, bValid, bRoot);
					        }

				        }
				        else if(m_Doc!=null) // that is if(!m_ptrNode)
				        {
					        if(temp.IndexOf('|')!=-1||temp.IndexOf('\\')!=-1||temp.IndexOf('/')!=-1)
					        {
						        string tmp = string.Empty;
						        for(int i=0; i<vec.Count-1;++i)
						        {
							        tmp += vec[i];
							        if(i!=vec.Count-2)
								        tmp += "|";
						        }
						        string name = string.Empty;
						        if(vec.Count>0)
							        name = vec[vec.Count-1];

						        return new Element(m_Doc, null, tmp, name, false, m_bRoot);
					        }

					        wstrNonExistingParent = m_strNonExistingParent;
					        wstrNonExistingParent += "|";
					        wstrNonExistingParent += m_strName;

					        if(wstrNonExistingParent=="|")
						        wstrNonExistingParent=string.Empty;

					        return new Element(m_Doc, null, wstrNonExistingParent, str, false, m_bRoot);
				        }
			        }
			        else // if(bMultipleParent==false)
			        {
				        if(m_Node!=null)
				        {
					        if(m_bRoot&&str==m_Node.Name)
						        return new Element(m_Doc, m_Node, wstrNonExistingParent, str, true, m_bRoot);

					        XmlNode pSrc = m_Node;
					        XmlNodeList pList = pSrc.ChildNodes;
					        if(pList!=null)
					        {
						        int len = pList.Count;
						        bool found = false;
						        for(int j=0; j<len; ++j)
						        {
							        XmlNode pNode = pList.Item(j);
							        if(pNode!=null)
							        {
								        if(str==pNode.Name)
								        {
									        pSrc = pNode;
									        found = true;
									        break;
								        }
							        }
						        }
						        if(false==found)
						        {
							        if(false==string.IsNullOrEmpty(m_strNonExistingParent))
							        {
								        wstrNonExistingParent = m_strNonExistingParent;
								        wstrNonExistingParent += "|";
								        wstrNonExistingParent += m_strName;
							        }
							        else
								        wstrNonExistingParent = string.Empty;

							        bool bRoot = m_bRoot;
							        if(pSrc!=m_Node)
								        bRoot = false;
        							
							        return new Element(m_Doc, pSrc, wstrNonExistingParent, str, false, bRoot);
						        }
						        else // if(found)
						        {
							        bool bRoot = m_bRoot;
							        if(pSrc!=m_Node)
								        bRoot = false;

							        return new Element(m_Doc, pSrc, wstrNonExistingParent, str, true, bRoot);
						        }
					        }
				        }
				        else if(m_Doc!=null) // that is if(!m_ptrNode)
				        {
					        wstrNonExistingParent = m_strNonExistingParent;
					        wstrNonExistingParent += "|";
					        wstrNonExistingParent += m_strName;

					        if(wstrNonExistingParent=="|")
						        wstrNonExistingParent="";

					        return new Element(m_Doc, m_Node, wstrNonExistingParent, str, false, false); // svv
				        }
			        }
		        }
		        else // if(false == m_strNonExistingParent.empty())
		        {
			        for(int i=0;i<vec.Count-1;++i)
			        {
				        if(string.IsNullOrEmpty(wstrNonExistingParent)==false)
					        wstrNonExistingParent += "|";

				        wstrNonExistingParent += vec[i];
			        }

			        return new Element(m_Doc, null, wstrNonExistingParent, str, false, m_bRoot);
		        }
	        }
	        else
	        {
		        throw new InvalidOperationException("No valid xml document and node in this element!");
	        }

	        return new Element();
        }
	    //! Try to resolve the null m_ptrNode problem
	    private void ResolveNullNode(string str)
        {
	        if(m_Node!=null||m_Doc.DocumentElement==null)
		        return;

	        bool bMultipleParent = false;
	        List<string> vec = null;
	        string temp = str;
	        string wstrNonExistingParent = string.Empty;
	        if(false==string.IsNullOrEmpty(m_strNonExistingParent))
	        {
		        temp = m_strNonExistingParent;
		        temp += "|";
		        temp += m_strName;
	        }
	        else
	        {
		        temp = m_strName;
	        }
	        SplitString(temp, out vec, out bMultipleParent);

	        if(bMultipleParent)
	        {
		        XmlNode pSrc = m_Doc.DocumentElement;
        				
		        for(int i=0; i<vec.Count; ++i)
		        {
			        // Get the collection from this node
			        // If successful, assign the found node to this element
			        // and find the next element.
			        XmlNodeList pList = pSrc.ChildNodes;
			        if(pList!=null)
			        {
				        int len = pList.Count;
				        for(int j=0; j<len; ++j)
				        {
					        XmlNode pNode = pList.Item(j);
					        if(pNode!=null)
					        {
						        if(vec[i]==pNode.Name)
						        {
							        pSrc = pNode;
							        if(m_Node==null&&vec[vec.Count-1]==pNode.Name)
							        {
								        m_Node = pNode;
								        return;
							        }
						        }
					        }
				        }
			        }
		        }
	        }
	        else // if(bMultipleParent==false)
	        {
		        XmlNode pSrc = m_Doc.DocumentElement;
		        if(m_strName==pSrc.Name)
		        {
			        m_Node = m_Doc.DocumentElement;
			        return;
		        }
		        XmlNodeList pList = pSrc.ChildNodes;
		        if(pList!=null)
		        {
			        int len = pList.Count;
			        for(int j=0; j<len; ++j)
			        {
				        XmlNode pNode = pList.Item(j);
				        if(pNode!=null)
				        {
					        if(m_Node==null)
					        {
						        if(str==pNode.Name)
						        {
							        pSrc = pNode;
							        if(m_Node==null&&m_strName==pNode.Name)
							        {
								        m_Node = pNode;
								        return;
							        }
						        }
					        }
				        }
			        }
		        }
	        }
        }
	    private void ResolveNode(string str)
        {
            if (m_Node == null)
	        {
		        m_Node = m_Doc.DocumentElement;
		        if(m_Node==null)
			        return;
            }

	        bool bMultipleParent = false;
	        List<string> vec = null;
	        string temp = str;
	        string wstrNonExistingParent = string.Empty;
	        if(false==string.IsNullOrEmpty(m_strNonExistingParent))
	        {
		        temp = m_strNonExistingParent;
		        temp += "|";
		        temp += m_strName;
	        }
	        else
	        {
		        temp = m_strName;
	        }
	        SplitString(temp, out vec, out bMultipleParent);
	        if(bMultipleParent)
	        {
		        m_strNonExistingParent = string.Empty;
		        for(int i=0; i<vec.Count-1; ++i)
		        {
			        m_strNonExistingParent += vec[i];
			        if(i!=vec.Count-2)
				        m_strNonExistingParent += "|";
		        }
		        if(vec.Count>0)
			        m_strName = vec[vec.Count-1];
	        }

	        return;
        }
	    //! Get the attribute value
	    //!
	    //! @param defaultVal is the default string value to use if src is invalid or empty
	    //! @param val is the string value to be returned
	    private bool GetString(string defaultVal, out string val)
        {
            val = defaultVal;
            if (false == string.IsNullOrEmpty(m_strNonExistingParent) || false == m_bValid)
                return false;

            if (m_Node!=null)
            {
                val = m_Node.InnerText;
                if (string.IsNullOrEmpty(val))
                    val = defaultVal;
            }

            return true;
        }

#endregion

#region Member Variables
	    //! Attribute object
	    public Attribute m_Attribute;
	    //! Delimited string of non existing parent
	    private string m_strNonExistingParent;
	    //! MS XML document object
	    private XmlDocument m_Doc;
	    //! MS XML node object
	    private XmlNode m_Node;
	    //! Stores the deleted state
	    private bool m_bDeleted;
	    //! Node name
	    private string m_strName;
	    //! Stores the valid state
	    private bool m_bValid;
	    //! State this node is root (is true if node 1st set with SetDocDom()
	    private bool m_bRoot;
#endregion
    }
}
