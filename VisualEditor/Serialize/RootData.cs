using NPBehave;

namespace NPSerialization
{
    public class RootData : NodeData
    {
        [System.NonSerialized]
        public Root m_root;

        public RootData(long id) : base(id)
        {
        }

        public override Node GetNode()
        {
            return m_root;
        }
    }
}