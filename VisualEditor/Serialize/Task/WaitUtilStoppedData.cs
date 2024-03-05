using NPBehave;

namespace NPSerialization
{
    public class WaitUtilStoppedData : NodeData
    {
        [System.NonSerialized]
        public WaitUntilStopped m_waitUtilStopped;
        bool sucessWhenStopped;

        public Task CreateTask()
        {
            m_waitUtilStopped = new WaitUntilStopped(sucessWhenStopped);
            return m_waitUtilStopped;
        }

        public override Node GetNode()
        {
            return m_waitUtilStopped;
        }
    }
}
