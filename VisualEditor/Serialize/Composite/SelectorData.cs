using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class SelectorData : NodeData
    {
        [JsonIgnore]
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
