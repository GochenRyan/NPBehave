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

            CreateNode(new Vector2(10, 10));
            CreateNode(new Vector2(10, 300));
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
            GraphNode node = new GraphNode();
            node.SetPosition(new Rect(position, DEFAULT_NODE_SIZE));
            AddElement(node);
            return node;
        }

        public readonly Vector2 DEFAULT_NODE_SIZE = new(150, 200);
    }
}