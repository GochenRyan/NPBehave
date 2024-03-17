using NPBehave;
using System.Collections.Generic;
using TMPro;

namespace NPSerialization
{
    public class NodeDataTree
    {
        public long m_rootID;

        public Dictionary<long, NodeData> m_nodeDataDict = new();

        public void CreateNPBehaveTree()
        {
            foreach (var pair in m_nodeDataDict)
            {
                switch (pair.Value.m_nodeType) 
                {
                    case NodeType.Task:
                        pair.Value.CreateTask();
                        break;
                    case NodeType.Decorator:
                        if (m_nodeDataDict.TryGetValue(pair.Value.m_linkedNodeIDs[0], out NodeData tmpDecorateeData))
                        {
                            pair.Value.CreateDecorator(tmpDecorateeData.GetNode());
                        }
                        else
                        {
                            throw new Exception("Node lost!");
                        }
                        break;
                    case NodeType.Composite:
                        List<Node> tmpNodes = new();
                        foreach(var linkID in pair.Value.m_linkedNodeIDs)
                        {
                            if (m_nodeDataDict.TryGetValue(linkID, out NodeData tmp))
                            {
                                tmpNodes.Add(tmp.GetNode());
                            }
                            else
                            {
                                throw new Exception("Node lost!");
                            }
                        }
                        pair.Value.CreateComposite(tmpNodes.ToArray());
                        break;
                }
            }
        }
    }
}
