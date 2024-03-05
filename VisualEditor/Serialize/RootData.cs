using NPBehave;

namespace NPSerialization
{
    public class RootData : NodeData
    {
        [System.NonSerialized]
        public Root m_root;

        public override Node GetNode()
        {
            return m_root;
        }
    }
}