using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TraverseToMember : Node
{
    //Positional
    private float _minimalOffset = 2.0f;
    private float _memberProximity;

    public TraverseToMember(AIBlackboard pAIBlackboard, float pMemberProximity)
    {
        aiBlackboard = pAIBlackboard;
        _memberProximity = pMemberProximity;
    }
    
    public override State EvaluateState()
    {
        GetDestinationInMemberProximity();
        return State.Success;
    }

    private void GetDestinationInMemberProximity()
    {
        bool goRandom = Random.Range(0.0f, 1.0f) < 0.5f;
        Vector3 memberPosition = Vector3.zero; 
        List<GameObject> potentialMembers = new List<GameObject>();
        List<GameObject> allCurrentNPC = aiBlackboard.GetAllNPCs();
        if (!goRandom)
        {
            foreach (var npc in allCurrentNPC)
            {
                AIBlackboard memberBlackboard = npc.GetComponent<AIBlackboard>();
                if (memberBlackboard.MemberPair != null || memberBlackboard.NavAgent.velocity.magnitude > 0.1f) continue;
                potentialMembers.Add(npc);
            }

            if (potentialMembers.Count > 0)
            {
                GameObject memberToGoTo = potentialMembers.ElementAt(Random.Range(0, potentialMembers.Count - 1));
                if(aiBlackboard.MemberPair != null)
                    aiBlackboard.MemberPair.GetComponent<AIBlackboard>().MemberPair = null;
                aiBlackboard.MemberPair = memberToGoTo;
                memberToGoTo.GetComponent<AIBlackboard>().MemberPair = aiBlackboard.gameObject;
                memberPosition = memberToGoTo.transform.position;
            }
            else
            {
                goRandom = true;
                if(aiBlackboard.MemberPair != null)
                    aiBlackboard.MemberPair.GetComponent<AIBlackboard>().MemberPair = null;
                aiBlackboard.MemberPair = null;
                memberPosition = aiBlackboard.transform.position;
            }
        }
        
        NavMeshPath navMeshPath = new NavMeshPath();
        Vector3 newPosition = Vector3.zero;
        int i = 0;
        bool randomPointInMemberProximity;
        do
        {
            randomPointInMemberProximity = false;
            Vector2 randomXZDirection = Random.insideUnitCircle.normalized * (goRandom ? Random.Range(_minimalOffset, 20 - i) : Random.Range(2.5f, 2.75f));
            newPosition = new Vector3(randomXZDirection.x , aiBlackboard.transform.position.y, randomXZDirection.y);
            // if (goRandom)
            // {
            //     foreach (var npc in allCurrentNPC)
            //     {
            //         if (((memberPosition + newPosition) - npc.GetComponent<AIBlackboard>().NavAgent.destination).magnitude < _memberProximity)
            //             randomPointInMemberProximity = true;
            //     }
            // }
            i++;
        } while ((!aiBlackboard.NavAgent.CalculatePath(memberPosition + newPosition, navMeshPath) || randomPointInMemberProximity) && i < 20);

        if(i == 20) Debug.Log("Couldn't find path");
        aiBlackboard.NavAgent.SetPath(navMeshPath);
    }
}
