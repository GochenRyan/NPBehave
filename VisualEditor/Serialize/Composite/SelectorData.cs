using NPBehave;

namespace NPSerialization
{
    public class SelectorData : NodeData
    {
        [System.NonSerialized]
        public Selector m_selector;

        public SelectorData(long id) : base(id)
        {
        }

        public Composite CrateComposite(Node[] nodes)
        {
            m_selector = new Selector(nodes);
            return m_selector;
        }

        public override Node GetNode()
        {
            return m_selector;
        }
    }
}
