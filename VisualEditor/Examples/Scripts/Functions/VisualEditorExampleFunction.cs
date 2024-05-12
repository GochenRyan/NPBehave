using NPSerialization;
using UnityEngine;

namespace NPVisualEditor_Example
{

    public class VisualEditorFunction : MonoBehaviour
    {
        private void Start()
        {
            CreateNPC();
            LoadNodeDataTree();
        }

        private void CreateNPC()
        {
            NPCModel model1001 = new(1001, 100, 20);
            NPCModel model1002 = new(1002, 100, 30);
            Behavoir1001 behavoir1001 = new(model1001);
            Behavoir1002 behavoir1002 = new(model1002);

            InstanceContext.Instance.RegisterReference(behavoir1001, 1001);
            InstanceContext.Instance.RegisterReference(behavoir1002, 1002);
        }

        private void LoadNodeDataTree()
        {

        }

        private NodeDataTree m_nodeDataTree;
    }
}