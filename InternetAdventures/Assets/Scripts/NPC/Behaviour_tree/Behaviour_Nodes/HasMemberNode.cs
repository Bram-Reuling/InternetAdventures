using UnityEngine;

public class HasMemberNode : Node
{
    private CommunityMemberBlackboard _communityMemberBlackboard;

    public HasMemberNode(in CommunityMemberBlackboard pCommunityMemberBlackboard)
    {
        _communityMemberBlackboard = pCommunityMemberBlackboard;
    }

    public override State EvaluateState()
    {
        nodeState = _communityMemberBlackboard.MemberPair != null ? State.Success : State.Failure;
        Debug.Log("Has member ");
        return nodeState;
    }
}
