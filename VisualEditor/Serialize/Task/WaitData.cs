using NPBehave;

namespace NPSerialization
{
    public class WaitData : NodeData
    {
        [System.NonSerialized]
        public Wait m_wait;
        public float m_seconds;

        public WaitData(long id) : base(id)
        {
        }

        public Task CreateTask()
        {
            m_wait = new Wait(m_seconds);
            return m_wait;
        }

        public override Node GetNode()
        {
            return m_wait;
        }
    }
}
