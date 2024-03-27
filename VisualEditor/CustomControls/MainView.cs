using UnityEngine;
using UnityEngine.UIElements;

namespace NPVisualEditor
{
    public class MainView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<MainView, UxmlTraits>
        {
        }

        public MainView()
        {
            InitView();
        }

        private void InitView()
        {
            this.orientation = TwoPaneSplitViewOrientation.Horizontal;

            m_leftPanel = new VisualElement();
            m_leftPanel.name = "LeftPanel";
            m_leftPanel.style.minWidth = 208.0f;
            this.Add(m_leftPanel);

            m_rightPanel = new VisualElement();
            m_rightPanel.name = "RightPanel";
            m_rightPanel.style.minWidth = 144.0f;
            this.Add(m_rightPanel);

            m_leftPanelSplitView = new TwoPaneSplitView();
            m_leftPanelSplitView.name = "LeftPanelSplitView";
            m_leftPanelSplitView.orientation = TwoPaneSplitViewOrientation.Vertical;

            m_inspectorContainer = new VisualElement();
            m_inspectorContainer.name = "InspectorContainer";
            m_inspectorContainer.style.minHeight = 192.0f;
            m_leftPanelSplitView.Add(m_inspectorContainer);
            m_inspector = new ScrollView();
            m_inspector.name = "Inspector";
            Label inspectorTitle = new Label("Inspector");
            inspectorTitle.name = "InspectorTitle";
            m_inspectorContainer.Add(inspectorTitle);
            m_inspectorContainer.Add(m_inspector);

            m_blackboardContainer = new VisualElement();
            m_blackboardContainer.name = "BlackboardContainer";
            m_blackboardContainer.style.minHeight = 192.0f;
            m_leftPanelSplitView.Add(m_blackboardContainer);
            m_blackboard = new VisualElement();
            m_blackboard.name = "Blackboard";
            Label blackboardTitle = new Label("Blackboard");
            blackboardTitle.name = "BlackboardTitle";
            m_blackboardContainer.Add(blackboardTitle);
            m_blackboardContainer.Add(m_blackboard);

            m_graphicView = new GraphicView();
            m_graphicView.name = "GraphicView";
            m_graphicView.StretchToParentSize();
            m_rightPanel.Add(m_graphicView);

            m_leftPanel.Add(m_leftPanelSplitView);
        }



        private VisualElement m_leftPanel;
        private VisualElement m_rightPanel;
        private TwoPaneSplitView m_leftPanelSplitView;

        private VisualElement m_inspectorContainer;
        private ScrollView m_inspector;
        private VisualElement m_blackboardContainer;
        private VisualElement m_blackboard;

        private GraphicView m_graphicView;
    }
}


