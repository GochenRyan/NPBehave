using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class SequenceData : NodeData
    {
        [JsonIgnore]
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
