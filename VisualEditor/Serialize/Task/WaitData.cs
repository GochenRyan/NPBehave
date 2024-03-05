using NPBehave;

namespace NPSerialization
{
    public class WaitData : ActionData
    {
        public float m_seconds;

        public override Task CreateTask()
        {
            return m_action;
        }
    }
}
