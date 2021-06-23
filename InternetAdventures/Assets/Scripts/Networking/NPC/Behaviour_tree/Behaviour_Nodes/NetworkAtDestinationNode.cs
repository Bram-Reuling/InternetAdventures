using UnityEngine;
using UnityEngine.AI;

public class NetworkAtDestinationNode : Node
{
    private NetworkCommunityMemberBlackboard _communityMemberBlackboard;
    private bool _atDestination;

    public NetworkAtDestinationNode(in NetworkCommunityMemberBlackboard pCommunityMemberBlackboard)
    {
        _communityMemberBlackboard = pCommunityMemberBlackboard;
    }

    public override State EvaluateState()
    {
        if (_communityMemberBlackboard.NavAgent.enabled)
        {
            if (_communityMemberBlackboard.NavAgent.velocity.magnitude < 0.1f &&
                !_communityMemberBlackboard.NavAgent.hasPath &&
                _communityMemberBlackboard.NavAgent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                _communityMemberBlackboard.NavAgent.enabled = false;
                //_communityMemberBlackboard.NavObstacle.enabled = true;
                nodeState = State.Success;
            }
            else nodeState = State.Failure;
        }
        else nodeState = State.Success;

        return nodeState;
    }
}
