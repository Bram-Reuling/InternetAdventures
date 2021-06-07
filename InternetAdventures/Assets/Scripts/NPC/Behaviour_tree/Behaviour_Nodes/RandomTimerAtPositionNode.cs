using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomTimerAtPositionNode : Node
{
    private readonly float _minTime;
    private readonly float _maxTime;
    private float _timePassed;
    private float _currentTimer;
    private bool _pathComplete;

    public RandomTimerAtPositionNode(AIBlackboard pAIBlackboard, float pMinTime, float pMaxTime)
    {
        aiBlackboard = pAIBlackboard;
        _minTime = pMinTime;
        _maxTime = pMaxTime;
        _currentTimer = Random.Range(_minTime, _maxTime);
    }
    
    public override State EvaluateState()
    {
        nodeState = State.Failure;
        if (!aiBlackboard.NavAgent.hasPath && aiBlackboard.NavAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            _pathComplete = true;
            aiBlackboard.NavAgent.enabled = false;
            aiBlackboard.NavObstacle.enabled = true;
        }
        
        if (_pathComplete)
            _timePassed += Time.deltaTime;
        
        
        if (_timePassed >= _currentTimer)
        {
            nodeState = State.Success;
            _timePassed = 0;
            _currentTimer = Random.Range(_minTime, _maxTime);
            aiBlackboard.NavAgent.enabled = true;
            _pathComplete = false;
        }

        if (_timePassed >= _currentTimer * 0.97f)
            aiBlackboard.NavObstacle.enabled = false;

        return nodeState;
    }
}
