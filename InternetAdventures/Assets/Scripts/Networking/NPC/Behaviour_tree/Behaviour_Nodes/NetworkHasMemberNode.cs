using UnityEngine;

public class NetworkHasMemberNode : Node
{
    private NetworkCommunityMemberBlackboard _communityMemberBlackboard;

    public NetworkHasMemberNode(in NetworkCommunityMemberBlackboard pCommunityMemberBlackboard)
    {
        _communityMemberBlackboard = pCommunityMemberBlackboard;
    }

    public override State EvaluateState()
    {
        nodeState = _communityMemberBlackboard.MemberPair != null ? State.Success : State.Failure;
        return nodeState;
    }
}
