using UnityEngine;

public class RandomTimerAtPositionNode : Node
{
    private readonly float _minTime;
    private readonly float _maxTime;
    private float _timePassed;
    private float _currentTimer;
    private CommunityMemberBlackboard _communityMemberBlackboard;

    public RandomTimerAtPositionNode(CommunityMemberBlackboard pAIBlackboard, float pMinTime, float pMaxTime)
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

        if (_timePassed >= _currentTimer * 0.95f)
            _communityMemberBlackboard.NavObstacle.enabled = false;

        return nodeState;
    }
}
