using NPBehave;
using System.Collections.Generic;

namespace NPSerialization
{
    /// <summary>
    /// Base node data
    /// </summary>
    public abstract class NodeData
    {
        public long m_ID;
        public NodeType m_nodeType;
        public List<long> m_linkedNodeIDs = new List<long>();
        public long m_parentID;
        public string m_description;
        
        public NodeData(long id) 
        {
            m_ID = id;
        }

        public abstract Node GetNode();
    }
}

