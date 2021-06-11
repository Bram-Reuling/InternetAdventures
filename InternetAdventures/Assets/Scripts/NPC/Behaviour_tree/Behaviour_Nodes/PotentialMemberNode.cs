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
            //Checks if there are AI destinations within my range.
            if ((currentNavAgent.destination - aiBlackboard.transform.position).magnitude <=
                _memberProximity)
            {
                //Checks if the AI is still traversing, meaning it walkings towards me.
                if (currentNavAgent.velocity.magnitude > 0.1f) nodeState = State.Failure;
                //If close-by but standing still, I consider moving on by 10%.
                else if (Random.Range(0.0f, 1.0f) < 0.1f) nodeState = State.Success;
                else nodeState = State.Failure;
                return nodeState;
            }
        }

        //Move on if there was no AI' destination in my range.
        nodeState = State.Success;
        return nodeState;
    }
}
