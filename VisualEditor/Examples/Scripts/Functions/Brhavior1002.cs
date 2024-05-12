using NPSerialization;
using System;

namespace NPVisualEditor_Example
{
    [SerializationID(1002)]
    public class Behavoir1002
    {
        public Behavoir1002(NPCModel npc)
        {
            m_npc = npc;
        }

        public void WalkTop()
        {
            m_npc.Power -= POWER_CONSUMPTION;
        }

        protected void WalkDown()
        {
            m_npc.Power -= POWER_CONSUMPTION;
        }

        private void WalkLeft()
        {
            m_npc.Power -= POWER_CONSUMPTION;
        }

        public void WalkRight()
        {
            m_npc.Power -= POWER_CONSUMPTION;
        }

        public void Idle()
        {
            m_npc.Power += POWER_CONSUMPTION;
        }

        public bool Attack()
        {
            Random random = new();
            int num = random.Next(9);
            if (num > 7)
            {
                return false;
            }
            return true;
        }


        public bool Defend()
        {
            Random random = new();
            int num = random.Next(9);
            if (num > 7)
            {
                m_npc.HP -= HP_CONSUMPTION;
                m_npc.Power -= POWER_CONSUMPTION;
                return false;
            }
            return true;
        }

        private NPCModel m_npc;

        private const int HP_CONSUMPTION = 1;
        private const int POWER_CONSUMPTION = 2;
    }
}