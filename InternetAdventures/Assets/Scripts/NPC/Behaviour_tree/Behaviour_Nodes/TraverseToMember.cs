using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TraverseToMember : Node
{
    //Positional
    private float _minimalOffset = 5.0f;

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
        bool goRandom = Random.Range(0.0f, 1.0f) < 0.4f;
        Vector3 memberPosition = Vector3.zero; 
        List<GameObject> potentialMembers = new List<GameObject>();
        if (!goRandom)
        {
            foreach (var npc in aiBlackboard.GetAllNPCs())
            {
                if (npc.GetComponent<AIBlackboard>().hasMember || npc.GetComponent<AIBlackboard>().NavAgent.velocity.magnitude > 0.15f) continue;
                potentialMembers.Add(npc);
            }

            if (potentialMembers.Count > 0)
            {
                GameObject memberToGoTo = potentialMembers.ElementAt(Random.Range(0, potentialMembers.Count - 1));
                memberToGoTo.GetComponent<AIBlackboard>().hasMember = true;
                memberPosition = memberToGoTo.transform.position;
            }
            else goRandom = true;
        }
        
        NavMeshPath navMeshPath = new NavMeshPath();
        Vector3 newPosition = Vector3.zero;
        int i = 0;
        do
        {
            Vector2 randomXZDirection = Random.insideUnitCircle * (goRandom ? Random.Range(_minimalOffset, 12.5f) : Random.Range(2.0f, 2.5f));
            newPosition = new Vector3(randomXZDirection.x, aiBlackboard.transform.position.y, randomXZDirection.y);
            aiBlackboard.NavAgent.CalculatePath(memberPosition + newPosition, navMeshPath);
            i++;
        } while (navMeshPath.status == NavMeshPathStatus.PathPartial && i < 10);

        aiBlackboard.NavAgent.SetDestination(memberPosition + newPosition);
    }
}
