using UnityEngine;

namespace NPVisualEditor_Example
{
    public class NPC : MonoBehaviour
    {
        private void Start()
        {
            m_sprite = this.transform.Find("Sprite").gameObject;
            m_animator = m_sprite.GetComponent<Animator>();
        }

        private void Update()
        {
            switch (NPCModel.AnimState)
            {
                case AnimState.IDLE:
                    break;
                case AnimState.WALK:
                    break;
                case AnimState.ATTACK:
                    break;
                case AnimState.DEFEND: 
                    break;
            }
        }

        public NPCModel NPCModel
        {
            get; set;
        }

        private GameObject m_sprite;
        private Animator m_animator;
    }
}