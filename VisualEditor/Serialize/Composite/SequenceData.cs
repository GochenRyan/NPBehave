using NPBehave;

namespace NPSerialization
{
    public class SequenceData : NodeData
    {
        [System.NonSerialized]
        public Sequence m_sequence;

        public SequenceData(long id) : base(id)
        {
        }

        public Composite CreateComposite(Node[] nodes)
        {
            m_sequence = new Sequence(nodes);
            return m_sequence;
        }

        public override Node GetNode()
        {
            return m_sequence;
        }
    }
}
