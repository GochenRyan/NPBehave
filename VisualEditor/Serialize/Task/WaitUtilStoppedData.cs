using NPBehave;

namespace NPSerialization
{
    public class WaitUtilStoppedData : ActionData
    {
        bool sucessWhenStopped;
        public override Task CreateTask()
        {
            return m_action;
        }
    }
}
