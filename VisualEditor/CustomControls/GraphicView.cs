using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NPVisualEditor
{
    public class GraphicView : GraphView
    {
        public GraphicView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("NodeGraphGridBackground"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            var grid = new GridBackground();
            Insert(0, grid);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            ports.ForEach(
               (port) =>
               {
                   if (startPort.node != port.node && startPort.direction != port.direction)
                   {
                       compatiblePorts.Add(port);
                   }
               }
            );
            return compatiblePorts;
        }

        public Edge CreateEdge(Port oput, Port iput)
        {
            var edge = new Edge { output = oput, input = iput };
            edge?.input.Connect(edge);
            edge?.output.Connect(edge);
            AddElement(edge);
            return edge;
        }

        public GraphNode CreateNode(Vector2 position)
        {
            GraphNode node = new();
            node.SetPosition(new Rect(position, GraphicUtils.DEFAULT_NODE_SIZE));
            AddElement(node);
            return node;
        }

        public void ClearGraphNodes()
        {
            DeleteElements(this.Query<GraphNode>().ToList());
            DeleteElements(this.Query<Edge>().ToList());
            RootNode = null;
        }
        public GraphNode RootNode { get; set; } = null;
    }
}