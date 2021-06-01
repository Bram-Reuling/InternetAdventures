using System.Collections.Generic;
using UnityEngine;

public class PotentialMemberNode : Node
{
    private float _groupProximity = 2.0f;
    
    public PotentialMemberNode(AIBlackboard pAIBlackboard)
    {
        aiBlackboard = pAIBlackboard;
    }
    
    public override State EvaluateState()
    {
        nodeState = State.Failure;
        foreach (var npc in aiBlackboard.GetAllNPCs())
        {
            if ((npc.GetComponent<AIBlackboard>().NavAgent.destination - aiBlackboard.transform.position).magnitude <= _groupProximity)
            {
                if (npc.GetComponent<AIBlackboard>().NavAgent.hasPath)
                {
                    //Wait
                }
                //Consider moving on
                nodeState = State.Success;
            }
        }
        
        //If failure consider moving on

        return nodeState;
    }
}
