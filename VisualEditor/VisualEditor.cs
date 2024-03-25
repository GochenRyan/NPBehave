using NPVisualEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class VisualEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/VisualEditor")]
    public static void ShowExample()
    {
        VisualEditor wnd = GetWindow<VisualEditor>();
        wnd.titleContent = new GUIContent("VisualEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        VisualTreeAsset visualEditor = Resources.Load<VisualTreeAsset>("VisualEditor");
        TemplateContainer editorInstance = visualEditor.CloneTree();
        editorInstance.StretchToParentSize();
        rootVisualElement.Add(editorInstance);
    }
}
