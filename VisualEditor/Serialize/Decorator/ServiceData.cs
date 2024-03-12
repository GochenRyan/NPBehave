using NPBehave;

namespace NPSerialization
{
    public class ServiceData : NodeData
    {
        [System.NonSerialized]
        public Service m_service;

        public float m_interval = -1.0f;
        public DelegateData m_delegateData;

        public ServiceData(long id) : base(id)
        {
        }

        public Service CreateService(Node node)
        {
            if (m_delegateData.m_action == null)
                throw new Exception("No Service Method!");

            m_service = new Service(m_interval, m_delegateData.m_action, node);

            return m_service;
        }

        public override Node GetNode()
        {
            return m_service;  
        }
    }
}
