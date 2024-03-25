using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NPVisualEditor
{
    public class GraphNode : Node
    {
        public new class UxmlFactory : UxmlFactory<GraphNode, UxmlTraits>
        {
        }

        public GraphNode() : base( AssetDatabase.GetAssetPath(Resources.Load<VisualTreeAsset>("GraphNode")))
        {
            m_parentPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(Node));
            m_parentPort.name = "Parent";
            inputContainer.Add(m_parentPort);

            m_childrenPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(Node));
            m_childrenPort.name = "Children";
            outputContainer.Add(m_childrenPort);

            RefreshExpandedState();
            RefreshPorts();
        }

        Port m_parentPort;
        Port m_childrenPort;
    }
}