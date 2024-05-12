using NPBehave;

namespace NPVisualEditor_Example
{
    public partial class Behavoir1001
    {
        public void LoadBehavoirNodes()
        {

        }

        private void UpdateBlackBoard()
        {
            m_blackboard["I_HP"] = m_npc.HP;
            m_blackboard["I_POWER"] = m_npc.Power;
        }

        private Root m_behaviorTree;
        private Blackboard m_blackboard;
    }
}