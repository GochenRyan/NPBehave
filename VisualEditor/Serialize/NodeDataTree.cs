using System.Collections.Generic;

namespace NPSerialization
{
    public class NodeDataTree
    {
        public long m_rootID;

        public Dictionary<long, NodeData> m_nodeDataDict = new Dictionary<long, NodeData>();
    }
}
