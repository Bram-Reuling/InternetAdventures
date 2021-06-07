using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TraverseToMember : Node
{
    //Positional
    private float _minimalOffset = 4.0f;

    public TraverseToMember(AIBlackboard pAIBlackboard)
    {
        aiBlackboard = pAIBlackboard;
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
        if (!goRandom)
        {
            foreach (var npc in aiBlackboard.GetAllNPCs())
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
        do
        {
            Debug.Log("Go to member " + !goRandom);
            Vector2 randomXZDirection = Random.insideUnitCircle * (goRandom ? Random.Range(_minimalOffset, 12.5f) : Random.Range(2.5f, 2.75f));
            newPosition = new Vector3(randomXZDirection.x, aiBlackboard.transform.position.y, randomXZDirection.y);
            aiBlackboard.NavAgent.CalculatePath(memberPosition + newPosition, navMeshPath);
            i++;
        } while (navMeshPath.status == NavMeshPathStatus.PathPartial && i < 10);

        aiBlackboard.NavAgent.SetDestination(memberPosition + newPosition);
    }
}
