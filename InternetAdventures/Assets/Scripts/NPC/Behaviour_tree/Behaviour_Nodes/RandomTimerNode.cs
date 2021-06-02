using UnityEngine;
using UnityEngine.AI;

public class RandomTimerNode : Node
{
    private float _timePassed;
    private float _currentTime;
    private readonly float _minTime;
    private readonly float _maxTime;
    
    public RandomTimerNode(AIBlackboard pAIBlackboard, float pMinTime, float pMaxTime)
    {
        aiBlackboard = pAIBlackboard;
        _minTime = pMinTime;
        _maxTime = pMaxTime;
    }
    
    public override State EvaluateState()
    {
        nodeState = State.Failure;
        
        if (_timePassed >= _currentTime)
        {
            nodeState = State.Success;
            _timePassed = 0;
            _currentTime = Random.Range(_minTime, _maxTime);
        }

        //This will be incorrect if behaviour tree is not evaluated every frame.
        if (aiBlackboard.NavAgent.pathStatus == NavMeshPathStatus.PathComplete)
            _timePassed += Time.deltaTime;
        
        return nodeState;
    }
}
