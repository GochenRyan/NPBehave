using NPBehave;

namespace NPSerialization
{
    public class BlackboardKVData
    {
        public string m_key;
        public CompareType m_compareType;
        public string m_theStringValue;
        public bool m_theBoolValue;
        public float m_theFloatValue;
        public int m_theIntValue;

        public void SetBlackBoardValue(Blackboard blackboard)
        {
            switch (m_compareType)
            {
                case CompareType.TString:
                    blackboard[m_key] = this.m_theStringValue;
                    break;
                case CompareType.TFloat:
                    blackboard[m_key] = this.m_theFloatValue;
                    break;
                case CompareType.TInt:
                    blackboard[m_key] = this.m_theIntValue;
                    break;
                case CompareType.TBoolean:
                    blackboard[m_key] = this.m_theBoolValue;
                    break;
            }
        }
    }
}