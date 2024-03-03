using NPBehave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPSerialization
{
    public class ActionData : NodeData
    {
        public Action m_action;
        public DelegateData m_actionData;

        public virtual Task CreateTask()
        {
            return m_action;
        }
    }
}