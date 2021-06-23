using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NetworkTraverseToMember : Node
{
    //Positional
    private float _minimalOffset = 2.0f;
    private float _memberProximity;
    private NetworkCommunityMemberBlackboard _communityMemberBlackboard;

    public NetworkTraverseToMember(NetworkCommunityMemberBlackboard pAIBlackboard, float pMemberProximity)
    {
        _communityMemberBlackboard = pAIBlackboard;
        _memberProximity = pMemberProximity;
    }
    
    public override State EvaluateState()
    {
        GetDestinationInMemberProximity();
        return State.Success;
    }

    private void GetDestinationInMemberProximity()
    {
        //If I move on I consider moving randomly by 35%.
        bool goRandom = Random.Range(0.0f, 1.0f) < 0.35f;
        Vector3 memberPosition = Vector3.zero; 
        List<GameObject> potentialMembers = new List<GameObject>();
        List<GameObject> allCurrentNPC = _communityMemberBlackboard.GetAllNPCs();
        if (!goRandom)
        {
            foreach (var npc in allCurrentNPC)
            {
                NetworkCommunityMemberBlackboard memberBlackboard = npc.GetComponent<NetworkCommunityMemberBlackboard>();

                if (memberBlackboard.MemberPair != null || memberBlackboard.NavAgent.velocity.magnitude > 0.1f) continue;
                
                Debug.Log("Adding npc to potential members");
                potentialMembers.Add(npc);
            }

            if (potentialMembers.Count > 0)
            {
                Debug.Log("Potential members count is > 0");
                GameObject memberToGoTo = potentialMembers.ElementAt(Random.Range(0, potentialMembers.Count - 1));
                if (_communityMemberBlackboard.MemberPair != null)
                {
                    _communityMemberBlackboard.MemberPair.GetComponent<NetworkCommunityMemberBlackboard>().MemberPair =
                        null;
                    Debug.Log("Setting member pair to null");
                }
                _communityMemberBlackboard.MemberPair = memberToGoTo;
                memberToGoTo.GetComponent<NetworkCommunityMemberBlackboard>().MemberPair = _communityMemberBlackboard.gameObject;
                Debug.Log("Setting member position");
                memberPosition = memberToGoTo.transform.position;
            }
            else
            {
                Debug.Log("No potential members");
                goRandom = true;
                memberPosition = _communityMemberBlackboard.transform.position;
            }
        }
        
        if(goRandom)
        {
            Debug.Log("GoRandom");
            if(_communityMemberBlackboard.MemberPair != null)
                _communityMemberBlackboard.MemberPair.GetComponent<NetworkCommunityMemberBlackboard>().MemberPair = null;
            _communityMemberBlackboard.MemberPair = null;
        }
        
        NavMeshPath navMeshPath = new NavMeshPath();
        Vector3 newPosition = Vector3.zero;
        int i = 0;
        bool randomPointInMemberProximity;
        do
        {
            randomPointInMemberProximity = false;
            Vector2 randomXZDirection = Random.insideUnitCircle * 
            (goRandom ? Random.Range(_minimalOffset, 20 - i) : Random.Range(1.25f, 2.5f));

            newPosition = new Vector3(randomXZDirection.x , 0, randomXZDirection.y);
            // if (goRandom)
            // {
            //     foreach (var npc in allCurrentNPC)
            //     {
            //         if (((memberPosition + newPosition) - npc.GetComponent<AIBlackboard>().NavAgent.destination).magnitude < _memberProximity)
            //             randomPointInMemberProximity = true;
            //     }
            // }
            i++;
        } while ((!_communityMemberBlackboard.NavAgent.CalculatePath(memberPosition + newPosition, navMeshPath) || randomPointInMemberProximity) && i < 20);

        if(i == 20) Debug.Log("Couldn't find path");
        _communityMemberBlackboard.NavAgent.SetPath(navMeshPath);
    }
}
