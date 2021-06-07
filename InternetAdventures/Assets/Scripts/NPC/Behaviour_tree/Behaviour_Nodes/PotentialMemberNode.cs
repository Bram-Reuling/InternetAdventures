using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class PotentialMemberNode : Node
{
    private float _memberProximity;
    
    public PotentialMemberNode(AIBlackboard pAIBlackboard, float pMemberProximity)
    {
        aiBlackboard = pAIBlackboard;
        _memberProximity = pMemberProximity;
    }
    
    public override State EvaluateState()
    {
        List<GameObject> allNPCs = aiBlackboard.GetAllNPCs();
        foreach (var currentNPC in allNPCs)
        {
            NavMeshAgent currentNavAgent = currentNPC.GetComponent<NavMeshAgent>();
            if ((currentNavAgent.destination - aiBlackboard.transform.position).magnitude <=
                _memberProximity)
            {
                if (currentNavAgent.velocity.magnitude > 0.1f)
                    nodeState = State.Failure;
                else if (Random.Range(0.0f, 1.0f) < 0.2f)
                    nodeState = State.Success;
                else nodeState = State.Failure;
                return nodeState;
            }
        }

        nodeState = State.Success;
        return nodeState;
    }
}
