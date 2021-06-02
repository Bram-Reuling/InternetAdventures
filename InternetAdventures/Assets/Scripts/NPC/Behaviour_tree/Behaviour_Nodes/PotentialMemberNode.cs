using UnityEngine;

public class PotentialMemberNode : Node
{
    private float _groupProximity = 2.5f;
    
    public PotentialMemberNode(AIBlackboard pAIBlackboard)
    {
        aiBlackboard = pAIBlackboard;
    }
    
    public override State EvaluateState()
    {
        nodeState = State.Success;
        aiBlackboard.hasMember = false;
        foreach (var npc in aiBlackboard.GetAllNPCs())
        {
            if ((npc.GetComponent<AIBlackboard>().NavAgent.destination - aiBlackboard.transform.position).magnitude <= _groupProximity)
            {
                aiBlackboard.hasMember = true;
                if (npc.GetComponent<AIBlackboard>().NavAgent.velocity.magnitude > 0.15f)
                {
                    nodeState = State.Failure;
                    break;
                }
                //Consider moving on
               nodeState = Random.Range(0.0f, 1.0f) < 0.4f ? State.Success : State.Failure;
            }
        }
        
        return nodeState;
    }
}
