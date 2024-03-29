using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NetworkTraverseToMember : Node
{
    //Positional
    private float _minimalOffset = 3.5f;
    private float _memberProximity;
    private NetworkCommunityMemberBlackboard _communityMemberBlackboard;
    private bool _alwaysGoRandom;

    public NetworkTraverseToMember(NetworkCommunityMemberBlackboard pAIBlackboard, float pMemberProximity, bool pAlwaysGoRandom)
    {
        _communityMemberBlackboard = pAIBlackboard;
        _memberProximity = pMemberProximity;
        _alwaysGoRandom = pAlwaysGoRandom;
    }
    
    public override State EvaluateState()
    {
        GetDestinationInMemberProximity();
        return State.Success;
    }

    private void GetDestinationInMemberProximity()
    {
        //If I move on I consider moving randomly by 35%.
        bool goRandom = _alwaysGoRandom || Random.Range(0.0f, 1.0f) < 0.35f;
        Vector3 memberPosition = Vector3.zero; 
        List<GameObject> potentialMembers = new List<GameObject>();
        List<GameObject> allCurrentNPC = _communityMemberBlackboard.GetAllNPCs();
        if (!goRandom)
        {
            NavMeshPath navMeshPaff = new NavMeshPath();
            foreach (var npc in allCurrentNPC)
            {
                NetworkCommunityMemberBlackboard memberBlackboard = npc.GetComponent<NetworkCommunityMemberBlackboard>();
                if (memberBlackboard.MemberPair != null || memberBlackboard.NavAgent.velocity.magnitude > 0.1f || Mathf.Abs
                        (_communityMemberBlackboard.transform.position.y - npc.transform.position.y) > 1.0f ||
                    !_communityMemberBlackboard.NavAgent.CalculatePath(npc.transform.position, navMeshPaff) || navMeshPaff
                        .corners[navMeshPaff.corners.Length - 1] != npc.transform.position)
                {
                    continue;
                }
                potentialMembers.Add(npc);
            }

            if (potentialMembers.Count > 0)
            {
                //Debug.Log("Potential members count is > 0");
                GameObject memberToGoTo = potentialMembers.ElementAt(Random.Range(0, potentialMembers.Count - 1));
                if (_communityMemberBlackboard.MemberPair != null)
                {
                    _communityMemberBlackboard.MemberPair.GetComponent<NetworkCommunityMemberBlackboard>().MemberPair = null;
                    //Debug.Log("Setting member pair to null");
                }
                _communityMemberBlackboard.MemberPair = memberToGoTo;
                memberToGoTo.GetComponent<NetworkCommunityMemberBlackboard>().MemberPair = _communityMemberBlackboard.gameObject;
                //Debug.Log("Setting member position");
                memberPosition = memberToGoTo.transform.position;
            }
            else
            {
                goRandom = true;
            } 
        }
        
        if(goRandom)
        {
            if(_communityMemberBlackboard.MemberPair != null)
                _communityMemberBlackboard.MemberPair.GetComponent<NetworkCommunityMemberBlackboard>().MemberPair = null;
            _communityMemberBlackboard.MemberPair = null;
            memberPosition = _communityMemberBlackboard.transform.position;
        }
        
        NavMeshPath navMeshPath = new NavMeshPath();
        Vector3 newPosition = Vector3.zero;
        bool notInMemberProximty;
        int i = 0;
        do
        {
            notInMemberProximty = true;
            Vector2 randomXZDirection = Random.insideUnitCircle.normalized * (goRandom ? Random.Range(_minimalOffset, 20.0f + _minimalOffset - i) 
                : Random.Range(1.75f, _memberProximity));

            newPosition = new Vector3(randomXZDirection.x, 0, randomXZDirection.y);

            if (goRandom)
            {
                foreach (var npc in allCurrentNPC)
                {
                    if (((memberPosition + newPosition) - npc.GetComponent<NetworkAIBlackboard>().NavAgent.destination).magnitude < _memberProximity)
                        notInMemberProximty = false;
                }
            }
            
        } while ((!_communityMemberBlackboard.NavAgent.CalculatePath(memberPosition + newPosition, navMeshPath) || !(navMeshPath.corners[navMeshPath.corners.Length - 1] == (memberPosition + newPosition)) || !notInMemberProximty) && i++ < 20);
        if(i == 20) Debug.Log("Couldn't find path");
        _communityMemberBlackboard.NavAgent.SetPath(navMeshPath);
    }
}
