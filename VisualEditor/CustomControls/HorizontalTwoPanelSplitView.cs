using UnityEngine.UIElements;

namespace NPBehave_VisualEditor
{
    public class HorizontalTwoPanelSplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<HorizontalTwoPanelSplitView, UxmlTraits> { }

        public HorizontalTwoPanelSplitView()
        {
            var leftPanel = new VisualElement();
            leftPanel.name = "leftPanel";
            leftPanel.style.minWidth = 192.0f;
            var verticalTwoPanelView = new VerticalTwoPanelSplitView();
            leftPanel.Add(verticalTwoPanelView);
            this.Add(leftPanel);

            var rightPanel = new VisualElement();
            rightPanel.name = "rightPanel";
            rightPanel.style.minWidth = 128.0f;
            this.Add(rightPanel);

            this.orientation = TwoPaneSplitViewOrientation.Horizontal;
        }
    }
}