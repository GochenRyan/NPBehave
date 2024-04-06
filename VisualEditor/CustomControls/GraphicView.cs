using NPSerialization;
using System;
using System.Collections.Generic;
using UnityEditor;
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

            nodeCreationRequest = CreateNode;
        }

        private void CreateNode(NodeCreationContext nodeCreationContext)
        {

        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is GraphView || evt.target is Node || evt.target is Group)
            {
                evt.menu.AppendAction("Cut", delegate
                {
                    CutSelectionCallback();
                }, (DropdownMenuAction a) => canCutSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }

            if (evt.target is GraphView || evt.target is Node || evt.target is Group)
            {
                evt.menu.AppendAction("Copy", delegate
                {
                    CopySelectionCallback();
                }, (DropdownMenuAction a) => canCopySelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }

            if (evt.target is GraphView)
            {
                evt.menu.AppendAction("Paste", delegate
                {
                    PasteCallback();
                }, (DropdownMenuAction a) => canPaste ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }

            if (evt.target is GraphView || evt.target is Node || evt.target is Group || evt.target is Edge)
            {
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Delete", delegate
                {
                    DeleteSelectionCallback(AskUser.DontAskUser);
                }, (DropdownMenuAction a) => canDeleteSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }

            if (evt.target is GraphView || evt.target is Node || evt.target is Group)
            {
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Duplicate", delegate
                {
                    DuplicateSelectionCallback();
                }, (DropdownMenuAction a) => canDuplicateSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                evt.menu.AppendSeparator();
            }

            if (evt.target is GraphView)
            {
                evt.menu.AppendSeparator();

                //TODO: Cache, tmpData
                var map = NodeDataUtils.GetNodeDataTypeMap();
                foreach (var node in map)
                {
                        foreach (var nodeClsType in node.Value)
                        {
                            string path = "Behavoir Nodes/" + node.Key.ToString() +  "/" + nodeClsType.Name;
                            evt.menu.AppendAction(path, 
                                (DropdownMenuAction dropdownMenuAction) =>
                                {
                                    Type nodeClsType = dropdownMenuAction.userData as Type;
                                    var instance = Activator.CreateInstance(nodeClsType) as NodeData;

                                    var graphNode = CreateNode(dropdownMenuAction.eventInfo.mousePosition);
                                    graphNode.title = nodeClsType.Name.Replace("Data", "");
                                    graphNode.Description = "";
                                    graphNode.SubTitle = NodeDataUtils.GetSubTitle(instance);
                                }
                               , (DropdownMenuAction dropdownMenuAction) => 
                               {
                                   return DropdownMenuAction.Status.Normal; 
                               }, nodeClsType);
                        }
                }
                evt.menu.AppendSeparator();
            }
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