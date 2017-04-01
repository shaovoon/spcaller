using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Elmax
{
    public class Comment
    {
        //! Constructor
	    public Comment()
        {
            m_Comment = null; 
        }
	    //! Non-default Constructor
	    public Comment(XmlComment xmlComment) 
        {
            m_Comment = xmlComment;
        }
	    //! Get the Comment data
	    public string GetComment()
        {
	        if(m_Comment==null)
                throw new System.InvalidOperationException("Invalid comment object");

	        return m_Comment.Data;
        }
	    //! Get the length of the Comment in Char size
	    public int GetLength()
        {
	        if(m_Comment==null)
                throw new System.InvalidOperationException("Invalid comment object");

	        return m_Comment.Length;
        }
	    //! Delete the Comment
	    public bool Delete()
        {
	        if(m_Comment==null)
                throw new System.InvalidOperationException("Invalid comment object");

	        XmlNode parent = m_Comment.ParentNode;
	        if(parent!=null)
	        {
		        m_Comment = (XmlComment)(parent.RemoveChild(m_Comment));
	        }
	        else
		        return false;

	        return true;
        }
	    //! Update the Comment
	    public bool Update(string comment)
        {
	        if(m_Comment==null)
		        throw new System.InvalidOperationException("Invalid comment object");

	        m_Comment.ReplaceData(0, comment.Length, comment);
	        return true;
        }
        public XmlComment GetInternalObject()
        {
            return m_Comment;
        }
        public bool IsValid()
        {
            if (m_Comment == null)
                return false;

            return true;
        }
    	//! Comment object
	    private XmlComment m_Comment;
    }
}
