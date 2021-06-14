using UnityEngine;

public class ConvertToBadNode : Node
{
    private readonly CommunityMemberBlackboard _communityMemberBlackboard;
    private readonly float minConvinceInterval, maxConvinceInterval;
    private float _timePassed;
    private float _currentTimer;

    public ConvertToBadNode(in CommunityMemberBlackboard pCommunityMemberBlackboard, in float pMinConvinceInterval, in float pMaxConvinceInterval)
    {
        _communityMemberBlackboard = pCommunityMemberBlackboard;
        minConvinceInterval = pMinConvinceInterval;
        maxConvinceInterval = pMaxConvinceInterval;
    }
    
    public override State EvaluateState()
    {
        // if (_timePassed >= _currentTimer)
        // {
        //     _timePassed = 0;
        //     _currentTimer = Random.Range(minConvinceInterval, maxConvinceInterval);
        //     
        //     //Try to convince
        //     if (_communityMemberBlackboard.MemberPair != null &&
        //         _communityMemberBlackboard.NavAgent.velocity.magnitude < 0.1f &&
        //         _communityMemberBlackboard.NavAgent.enabled == false)
        //     {
        //         if (Random.Range(1.0f, 2.0f) > 0.9f)
        //         {
        //             _communityMemberBlackboard.MemberPair.GetComponent<GoodMemberBlackboard>().TurnBad();
        //             _communityMemberBlackboard.MemberPair = null;
        //         }
        //     }
        // }
        // else 
            nodeState = State.Success;
        
        _timePassed += Time.deltaTime;
        return nodeState;
    }
}
