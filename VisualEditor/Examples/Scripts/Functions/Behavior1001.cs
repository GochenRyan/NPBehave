using NPSerialization;
using UnityEngine;
using Random = System.Random;

namespace NPVisualEditor_Example
{
    [SerializationID(1001)]
    public partial class Behavoir1001
    {
        public Behavoir1001(NPCModel npc)
        {
            m_npc = npc;
        }

        public void WalkTop()
        {
            m_npc.Power -= POWER_CONSUMPTION;
            m_npc.Direction = Vector2.up;
        }

        protected void WalkDown()
        {
            m_npc.Power -= POWER_CONSUMPTION;
            m_npc.Direction = Vector2.down;
        }

        private void WalkLeft()
        {
            m_npc.Power -= POWER_CONSUMPTION;
            m_npc.Direction = Vector2.left;
        }

        public void WalkRight()
        {
            m_npc.Power -= POWER_CONSUMPTION;
            m_npc.Direction = Vector2.right;
        }

        public void Idle()
        {
            m_npc.Power += POWER_CONSUMPTION;
            m_npc.Direction = Vector2.zero;
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