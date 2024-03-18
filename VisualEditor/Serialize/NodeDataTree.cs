using NPBehave;
using System.Collections.Generic;
using TMPro;

namespace NPSerialization
{
    public class NodeDataTree
    {
        public long m_rootID;

        public Dictionary<long, NodeData> m_nodeDataDict = new();

        public void CreateTreeByNodeData()
        {
            if (!m_nodeDataDict.ContainsKey(m_rootID))
                return;

            Stack<NodeData> stack = new();
            Queue<NodeData> tmpQueue = new();
            tmpQueue.Enqueue(m_nodeDataDict[m_rootID]);

            while (tmpQueue.Count > 0) 
            {
                NodeData tmp = tmpQueue.Dequeue();
                if (tmp.m_linkedNodeIDs.Count > 0)
                {
                    for (int i = 0; i <  tmp.m_linkedNodeIDs.Count; ++i)
                    {
                        if (m_nodeDataDict.TryGetValue(tmp.m_linkedNodeIDs[i], out var linkedNodeData))
                        {
                            tmpQueue.Enqueue(linkedNodeData);
                        }
                    }
                }
                stack.Push(tmp);
            }

            // Create this tree from dependencies to root
            
            while(stack.Count > 0)
            {
                NodeData current = stack.Pop();
                switch (current.m_nodeType)
                {
                    case NodeType.Task:
                        current.CreateTask();
                        break;
                    case NodeType.Decorator:
                        if (m_nodeDataDict.TryGetValue(current.m_linkedNodeIDs[0], out NodeData tmpDecorateeData))
                        {
                            current.CreateDecorator(tmpDecorateeData.GetNode());
                        }
                        else
                        {
                            throw new Exception("Node lost!");
                        }
                        break;
                    case NodeType.Composite:
                        List<Node> tmpNodes = new();
                        foreach (var linkID in current.m_linkedNodeIDs)
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
                        current.CreateComposite(tmpNodes.ToArray());
                        break;
                }
            }
        }
    }
}
