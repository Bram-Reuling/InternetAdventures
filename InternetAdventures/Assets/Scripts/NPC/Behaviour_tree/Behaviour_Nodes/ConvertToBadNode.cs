using UnityEngine;

public class ConvertToBadNode : Node
{
    private readonly CommunityMemberBlackboard _communityMemberBlackboard;
    private GameObject _oldMember;
    private float _timePassed;
    private float _currentTimer;

    public ConvertToBadNode(in CommunityMemberBlackboard pCommunityMemberBlackboard)
    {
        _communityMemberBlackboard = pCommunityMemberBlackboard;
    }
    
    public override State EvaluateState()
    {
        if (_oldMember != _communityMemberBlackboard.MemberPair)
        {
            _oldMember = _communityMemberBlackboard.MemberPair;
            
            //Note: The probability that a good community member is convinced to turn bad is currently 15%.
            if (Random.Range(0.0f, 1.0f) <= 0.15f)
            {
                //Convinced to turn bad
                try
                {
                    GoodMemberBlackboard goodMemberBlackboard = _communityMemberBlackboard.MemberPair.GetComponent<GoodMemberBlackboard>();
                    goodMemberBlackboard.TurnBad();
                    LoseWinHandler.AddToBadList(_communityMemberBlackboard.MemberPair);
                    _oldMember = _communityMemberBlackboard.MemberPair;
                    nodeState = State.Success;
                }
                catch
                {
                    Debug.Log("Member was already bad");
                }
            }
            else
            {
                //Not successfully convinced
                nodeState = State.Failure;
            }   
        }

        return nodeState;
    }
}
