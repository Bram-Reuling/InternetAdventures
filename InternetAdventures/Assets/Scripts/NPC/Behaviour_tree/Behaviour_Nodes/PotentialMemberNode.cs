using UnityEngine;

public class PotentialMemberNode : Node
{
    private float _groupProximity = 1.5f;
    
    public PotentialMemberNode(AIBlackboard pAIBlackboard)
    {
        aiBlackboard = pAIBlackboard;
    }
    
    public override State EvaluateState()
    {
        nodeState = State.Success;
        foreach (var npc in aiBlackboard.GetAllNPCs())
        {
            if(npc == aiBlackboard.gameObject) continue;
            if ((npc.GetComponent<AIBlackboard>().NavAgent.destination - aiBlackboard.transform.position).magnitude <= _groupProximity)
            {
                if (npc.GetComponent<AIBlackboard>().NavAgent.hasPath)
                {
                    nodeState = State.Failure;
                    Debug.Log("Halted movement. NPC " + npc.name + " was on its way");
                }
                //Consider moving on
                else nodeState = Random.Range(0.0f, 1.0f) < 0.25f ? State.Success : State.Failure;
            }
        }

        return nodeState;
    }
}
