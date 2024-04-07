using System;
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
            m_descLabel = this.Q<Label>("description-label");
            m_subTitleLabel = this.Q<Label>("subtitle-label");

            m_parentPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(Node));
            m_parentPort.name = "Parent";
            inputContainer.Add(m_parentPort);

            m_childrenPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(Node));
            m_childrenPort.name = "Children";
            outputContainer.Add(m_childrenPort);

            RefreshExpandedState();
            RefreshPorts();
        }

        public string Description
        {
            get
            {
                return (m_descLabel != null) ? m_descLabel.text : string.Empty;
            }
            set
            {
                if (m_descLabel != null)
                {
                    m_descLabel.text = value;
                }
            }
        }

        public string SubTitle
        {
            get
            {
                return (m_subTitleLabel != null) ? m_subTitleLabel.text : string.Empty;
            }
            set
            {
                if (m_subTitleLabel != null)
                {
                    m_subTitleLabel.text = value;
                }
            }
        }

        public long ID { get; set; }

        public override void OnSelected()
        {
            base.OnSelected();
            SelectedCB?.Invoke(this, EventArgs.Empty);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            UnselectedCB?.Invoke(this, EventArgs.Empty);
        }

        Port m_parentPort;
        Port m_childrenPort;

        private readonly Label m_descLabel;
        private readonly Label m_subTitleLabel;

        public event EventHandler SelectedCB;
        public event EventHandler UnselectedCB;
    }
}