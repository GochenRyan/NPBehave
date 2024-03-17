using NPBehave;
using System.Collections.Generic;

namespace NPSerialization
{
    /// <summary>
    /// Base node data
    /// </summary>
    /// <note>
    /// Maybe I should use Schema, but Super Base Class is enough
    /// </note>
    public class NodeData
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

        public virtual Node GetNode() => null;

        public virtual Task CreateTask() => null;

        public virtual Decorator CreateDecorator(Node node) => null;

        public virtual Composite CreateComposite(Node[] nodes) => null;
    }
}

