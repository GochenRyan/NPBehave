using NPBehave;
using NPSerialization;
using System.IO;
using UnityEngine;

namespace NPVisualEditor_Example
{
    public class VisualEditorExampleSerialize : MonoBehaviour
    {
        private void Start()
        {
            CreateNodeDataTree();
            Serialize();
        }

        private void CreateNodeDataTree()
        {
            m_nodeDataTree = new NodeDataTree();
            
            var rootData = new RootData(ID);
            m_nodeDataTree.m_nodeDataDict[rootData.m_ID] = rootData;
            m_nodeDataTree.m_rootID = rootData.m_ID;

            var serviceData = new ServiceData(ID);
            serviceData.m_parentID = rootData.m_ID;
            serviceData.m_interval = 0.125f;
            serviceData.m_delegateData.m_action = UpdateBlackboard;
            rootData.m_linkedNodeIDs.Add(serviceData.m_ID);
            m_nodeDataTree.m_nodeDataDict[serviceData.m_ID] = serviceData;

            var selectorData = new SelectorData(ID);
            selectorData.m_parentID = serviceData.m_ID;
            m_nodeDataTree.m_nodeDataDict[selectorData.m_ID] = selectorData;

            var blackboardConditionData = new BlackboardConditionData(ID);
            blackboardConditionData.m_blackboardData = new BlackboardKVData()
            {
                m_key = "playerDistance",
                m_compareType = CompareType.TFloat,
                m_theFloatValue = 7.5f
            };
            blackboardConditionData.m_operator = Operator.IS_SMALLER;
            blackboardConditionData.m_stopsOnChange = Stops.IMMEDIATE_RESTART;
            blackboardConditionData.m_parentID = selectorData.m_ID;
            selectorData.m_linkedNodeIDs.Add(blackboardConditionData.m_ID);

            var sequenceData1 = new SequenceData(ID);
            sequenceData1.m_parentID = blackboardConditionData.m_ID;
            blackboardConditionData.m_linkedNodeIDs.Add(sequenceData1.m_ID);

            var actionData1 = new ActionData(ID);
            actionData1.m_actionData.m_action = SetColor;
            actionData1.m_parentID = sequenceData1.m_ID;
            sequenceData1.m_linkedNodeIDs.Add(actionData1.m_ID);

            var actionData2 = new ActionData(ID);
            actionData2.m_actionData.m_multiFrameFunc = Move;
            actionData2.m_parentID = sequenceData1.m_ID;
            sequenceData1.m_linkedNodeIDs.Add(actionData2.m_ID);

            var sequenceData2 = new SequenceData(ID);
            sequenceData2.m_parentID = selectorData.m_ID;
            selectorData.m_linkedNodeIDs.Add(sequenceData2.m_ID);

            var actionData3 = new ActionData(ID);
            actionData3.m_actionData.m_action = SetColor;
            actionData3.m_parentID = sequenceData2.m_ID;
            sequenceData2.m_linkedNodeIDs.Add(actionData3.m_ID);

            var waitUtilStoppedData = new WaitUtilStoppedData(ID);
            waitUtilStoppedData.m_parentID = sequenceData2.m_ID;
            sequenceData2.m_linkedNodeIDs.Add(waitUtilStoppedData.m_ID);
        }

        private static void UpdateBlackboard()
        {

        }

        private static void SetColor()
        {

        }

        private static Action.Result Move(bool shouldCancel)
        {
            if (!shouldCancel)
            {
                return Action.Result.PROGRESS;
            }
            else
            {
                return Action.Result.FAILED;
            }
        }

        private void Serialize()
        {
            var jsonStream = new JsonStream();
            string path = Path.Combine(Application.dataPath, "test_tree.json");
            jsonStream.Save(m_nodeDataTree, path);
        }

        private long m_id = 0;

        private long ID
        {
            get 
            {
                m_id++;
                return m_id; 
            }
        }

        private NodeDataTree m_nodeDataTree;
    }
}
