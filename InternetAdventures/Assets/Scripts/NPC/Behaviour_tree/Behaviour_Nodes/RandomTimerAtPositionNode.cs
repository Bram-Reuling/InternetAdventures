using UnityEngine;
using UnityEngine.AI;

public class RandomTimerAtPositionNode : Node
{
    private float _timePassed;
    private float _currentTimer;
    private readonly float _minTime;
    private readonly float _maxTime;
    private bool _destinationReached;
    
    public RandomTimerAtPositionNode(AIBlackboard pAIBlackboard, float pMinTime, float pMaxTime)
    {
        aiBlackboard = pAIBlackboard;
        _minTime = pMinTime;
        _maxTime = pMaxTime;
    }
    
    public override State EvaluateState()
    {
        nodeState = State.Failure;
        if (_timePassed >= _currentTimer)
        {
            nodeState = State.Success;
            _timePassed = 0;
            _currentTimer = Random.Range(_minTime, _maxTime);
            return nodeState;
        }

        if (aiBlackboard.NavAgent.pathStatus == NavMeshPathStatus.PathComplete)
            _timePassed += Time.deltaTime;

        return nodeState;
    }
}
