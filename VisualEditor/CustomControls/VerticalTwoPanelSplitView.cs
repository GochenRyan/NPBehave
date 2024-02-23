using UnityEngine.UIElements;

namespace NPBehave_VisualEditor
{
    public class VerticalTwoPanelSplitView : TwoPaneSplitView
    {
        public VerticalTwoPanelSplitView()
        {
            var topPanel = new VisualElement();
            topPanel.name = "topPanel";
            topPanel.style.minHeight = 144.0f;
            this.Add(topPanel);

            var downPanel = new VisualElement();
            downPanel.name = "downPanel";
            downPanel.style.minHeight = 96.0f;
            this.Add(downPanel);

            this.orientation = TwoPaneSplitViewOrientation.Vertical;
        }
    }
}