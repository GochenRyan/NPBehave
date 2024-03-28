using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NPVisualEditor
{
    public static class GraphicUtils
    {
        public static IList<GraphNode> GetChildren(GraphNode node)
        {
            List<GraphNode> children = new();
            Port port = node.Q<Port>("Children");
            foreach (var edge in port.connections)
            {
                GraphNode connectedNode = edge.input.node as GraphNode;
                children.Add(connectedNode);
            }

            return children;
        }

        public static Vector2 GetPosition(GraphNode node)
        {
            Rect nodeWorldBound = node.worldBound;
            return nodeWorldBound.position;
        }

        // TODO: FIX
        public static void OptimizeTreeLayout(GraphNode rootNode)
        {
            Layout(rootNode);
        }

        private static void Layout(GraphNode node)
        {
            IList<GraphNode> children = GetChildren(node);
            if (children.Count == 0) return;

            // Calculate the starting position for child nodes layout
            float start = GetPosition(node).x - (children.Count - 1) * NodeInterval / 2;

            for (int i = 0; i < children.Count; i++)
            {
                float x = start + i * NodeInterval;
                float y = GetPosition(node).y + YInterval;
                SetPosition(children[i], new Vector2(x, y));
                Layout(children[i]);
            }

            // Detect and resolve overlaps
            ResolveOverlaps(node);
        }

        private static void ResolveOverlaps(GraphNode node)
        {
            IList<GraphNode> children = GetChildren(node);
            for (int i = 0; i < children.Count - 1; i++)
            {
                GraphNode currentChild = children[i];
                GraphNode nextChild = children[i + 1];

                // If there's an overlap, move the next child node
                if (GetPosition(nextChild).x <= GetPosition(currentChild).x + NodeInterval)
                {
                    float dx = GetPosition(currentChild).x + NodeInterval - GetPosition(nextChild).x;
                    SetPosition(nextChild, new Vector2(GetPosition(nextChild).x + dx, GetPosition(nextChild).y));
                }
            }
        }

        // Set the position of a node
        public static void SetPosition(GraphNode node, Vector2 position)
        {
            node.style.left = position.x;
            node.style.top = position.y;
        }

        static float NodeInterval = 300f;
        static float YInterval = 200f;
    }
}