namespace NPSerialization
{ 
    public static class NodeDataUtils
    {
        public static void AddChild(NodeData parent, NodeData child)
        {
            if (!parent.m_linkedNodeIDs.Contains(child.m_ID))
            {
                parent.m_linkedNodeIDs.Add(child.m_ID);
            }
            child.m_parentID = parent.m_ID;
        }

        public static void AddChildren(NodeData parent, params NodeData[] children)
        {
            foreach (NodeData child in children)
            {
                if (!parent.m_linkedNodeIDs.Contains(child.m_ID))
                {
                    parent.m_linkedNodeIDs.Add(child.m_ID);
                }
                child.m_parentID = parent.m_ID;
            }
        }
    }
}