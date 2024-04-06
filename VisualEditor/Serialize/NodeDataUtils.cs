using NPBehave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static string GetSubTitle(NodeData nodeData)
        {
            var subTitle = string.Empty;
            switch(nodeData.m_nodeType)
            {
                case NodeType.Composite:
                    if (typeof(SelectorData).FullName == nodeData.TYPE_NAME_FOR_SERIALIZATION)
                    {
                        subTitle = "Run children sequentially until one succeeds and succeed (succeeds if one of the children succeeds).";
                    }
                    else if (typeof(SequenceData).FullName == nodeData.TYPE_NAME_FOR_SERIALIZATION)
                    {
                        subTitle = "Run children sequentially until one fails and fail (succeeds if none of the children fails)";
                    }
                    break;
                case NodeType.Decorator:
                    if (typeof(BlackboardConditionData).FullName == nodeData.TYPE_NAME_FOR_SERIALIZATION)
                    {
                        var blackboardConditionData = nodeData as BlackboardConditionData;
                        subTitle = blackboardConditionData.m_blackboardData.m_key + "," + blackboardConditionData.m_operator.ToString() + "," + blackboardConditionData.m_blackboardData.GetValue().ToString() + "," + blackboardConditionData.m_stopsOnChange.ToString();
                    }
                    else if (typeof(ServiceData).FullName == nodeData.TYPE_NAME_FOR_SERIALIZATION)
                    {
                        var serviceData = nodeData as ServiceData;
                        subTitle = serviceData.m_interval.ToString() + "," + serviceData.m_delegateData.m_action.Method.Name;
                    }
                    break;
                case NodeType.Task:
                    if (typeof(ActionData).FullName == nodeData.TYPE_NAME_FOR_SERIALIZATION)
                    {
                        var actionData = nodeData as ActionData;
                        if (actionData.m_actionData.m_action != null)
                        {
                            subTitle = actionData.m_actionData.m_action.Method.Name;
                        }
                        else if (actionData.m_actionData.m_singleFrameFunc != null)
                        {
                            subTitle = actionData.m_actionData.m_singleFrameFunc.Method.Name;
                        }
                        else if (actionData.m_actionData.m_multiFrameFunc != null)
                        {
                            subTitle = actionData.m_actionData.m_multiFrameFunc.Method.Name;
                        }
                        else if (actionData.m_actionData.m_multiFrameFunc2 != null)
                        {
                            subTitle = actionData.m_actionData.m_multiFrameFunc2.Method.Name;
                        }
                    }
                    else
                    {
                        subTitle = nodeData.TYPE_NAME_FOR_SERIALIZATION;
                    }
                    break;
            }

            return subTitle;
        }

        public static Dictionary<NodeType, List<Type>> GetNodeDataTypeMap()
        {
            var nodeDataTypeMap = new Dictionary<NodeType, List<Type>>();

            var nodeDataTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(NodeData)));

            foreach (var type in nodeDataTypes)
            {
                var instance = Activator.CreateInstance(type) as NodeData;
                NodeType nodeType = instance.m_nodeType;
                if (nodeDataTypeMap.ContainsKey(nodeType))
                {
                    nodeDataTypeMap[nodeType].Add(type);
                }
                else
                {
                    nodeDataTypeMap.Add(nodeType, new List<Type>() { type });
                }
            }

            return nodeDataTypeMap;
        }
    }
}