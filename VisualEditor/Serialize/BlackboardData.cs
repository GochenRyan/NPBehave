using NPBehave;

namespace NPSerialization
{
    public class BlackboardData : NodeData
    {
        public string m_key;
        public CompareType m_compareType;
        public string theStringValue;
        public bool theBoolValue;
        public float theFloatValue;
        public int theIntValue;

        public void SetBlackBoardValue(Blackboard blackboard)
        {
            switch (m_compareType)
            {
                case CompareType.TString:
                    blackboard[m_key] = this.theStringValue;
                    break;
                case CompareType.TFloat:
                    blackboard[m_key] = this.theFloatValue;
                    break;
                case CompareType.TInt:
                    blackboard[m_key] = this.theIntValue;
                    break;
                case CompareType.TBoolean:
                    blackboard[m_key] = this.theBoolValue;
                    break;
            }
        }
    }
}