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
            NPCModel model = new(1001, 100, 20);
            Boss boss = new(model);

            InstanceContext.Instance.RegisterReference(boss, 1001);
        }

        private void LoadNodeDataTree()
        {

        }

        private NodeDataTree m_nodeDataTree;
    }
}