using UnityEngine;

public class RotateToMember : Node
{
    private CommunityMemberBlackboard _communityMemberBlackboard;
    
    public RotateToMember(in CommunityMemberBlackboard pAIBlackboard)
    {
        _communityMemberBlackboard = pAIBlackboard;
    }


    public override State EvaluateState()
    {
        nodeState = State.Success;
        if(_communityMemberBlackboard.MemberPair != null && _communityMemberBlackboard.NavAgent.velocity.magnitude < 0.1f && _communityMemberBlackboard.NavAgent.enabled == false){
            Vector3 vecToMember = _communityMemberBlackboard.MemberPair.transform.position - _communityMemberBlackboard.transform.position;
            if(Vector3.Angle(_communityMemberBlackboard.transform.forward, vecToMember.normalized) > 5){
                _communityMemberBlackboard.transform.rotation = Quaternion.Slerp(_communityMemberBlackboard.transform.rotation, Quaternion.LookRotation(vecToMember, _communityMemberBlackboard.transform.up), 0.02f);
            }
        }
        return nodeState;
    }
}
