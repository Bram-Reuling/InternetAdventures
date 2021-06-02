using UnityEngine;
using UnityEngine.AI;

public class RandomTimerAtPositionNode : Node
{
    private float _timePassed;
    private float _currentTimer;
    private readonly float _minTime;
    private readonly float _maxTime;
    
    public RandomTimerAtPositionNode(AIBlackboard pAIBlackboard, float pMinTime, float pMaxTime)
    {
        aiBlackboard = pAIBlackboard;
        _minTime = pMinTime;
        _maxTime = pMaxTime;
    }
    
    public override State EvaluateState()
    {
        nodeState = State.Failure;
        //aiBlackboard.NavAgent.enabled = false;
        //aiBlackboard.NavObstacle.enabled = true;
        
        if (_timePassed >= _currentTimer)
        {
            nodeState = State.Success;
            _timePassed = 0;
            _currentTimer = Random.Range(_minTime, _maxTime);
            //aiBlackboard.NavObstacle.enabled = false;
            //aiBlackboard.NavAgent.enabled = true;
        }

        //This will be incorrect if behaviour tree is not evaluated every frame.
        if (aiBlackboard.NavAgent.pathStatus == NavMeshPathStatus.PathComplete)
            _timePassed += Time.deltaTime;

        return nodeState;
    }
}
