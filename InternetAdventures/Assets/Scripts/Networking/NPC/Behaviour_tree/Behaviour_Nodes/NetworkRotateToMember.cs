using UnityEngine;

public class NetworkRotateToMember : Node
{
    private NetworkCommunityMemberBlackboard _communityMemberBlackboard;
    
    public NetworkRotateToMember(in NetworkCommunityMemberBlackboard pAIBlackboard)
    {
        _communityMemberBlackboard = pAIBlackboard;
    }
    
    public override State EvaluateState()
    {
        Vector3 vecToMember = _communityMemberBlackboard.MemberPair.transform.position - _communityMemberBlackboard.transform.position;
        if (Vector3.Angle(_communityMemberBlackboard.transform.forward, vecToMember.normalized) > 5)
        {
            //nodeState = State.Success;
            _communityMemberBlackboard.transform.rotation = Quaternion.Slerp(
                _communityMemberBlackboard.transform.rotation,
                Quaternion.LookRotation(vecToMember, _communityMemberBlackboard.transform.up), 0.02f);
        }
        //else nodeState = State.Failure;
        nodeState = State.Success;

        return nodeState;
    }
}
