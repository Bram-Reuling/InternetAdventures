using UnityEngine;

public class NetworkRandomTimerAtPositionNode : Node
{
    private readonly float _minTime;
    private readonly float _maxTime;
    private float _timePassed;
    private float _currentTimer;
    private NetworkCommunityMemberBlackboard _communityMemberBlackboard;

    public NetworkRandomTimerAtPositionNode(NetworkCommunityMemberBlackboard pAIBlackboard, float pMinTime, float pMaxTime)
    {
        _communityMemberBlackboard = pAIBlackboard;
        _minTime = pMinTime;
        _maxTime = pMaxTime;
        _currentTimer = Random.Range(_minTime, _maxTime);
    }
    
    public override State EvaluateState()
    {
        nodeState = State.Failure;
        _timePassed += Time.deltaTime;

        if (_timePassed >= _currentTimer)
        {
            nodeState = State.Success;
            _timePassed = 0;
            _currentTimer = Random.Range(_minTime, _maxTime);
            _communityMemberBlackboard.NavAgent.enabled = true;
        }

        if (_timePassed >= _currentTimer * 0.97f)
            _communityMemberBlackboard.NavObstacle.enabled = false;

        return nodeState;
    }
}
