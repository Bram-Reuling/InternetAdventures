using UnityEngine;
using UnityEngine.AI;

public class NetworkConvertToBadNode : Node
{
    private readonly NetworkCommunityMemberBlackboard _communityMemberBlackboard;
    private GameObject _oldMember;
    private float _timePassed;
    private float _currentTimer;

    public NetworkConvertToBadNode(in NetworkCommunityMemberBlackboard pCommunityMemberBlackboard)
    {
        _communityMemberBlackboard = pCommunityMemberBlackboard;
    }
    
    public override State EvaluateState()
    {
        if (_oldMember != _communityMemberBlackboard.MemberPair)
        {
            _oldMember = _communityMemberBlackboard.MemberPair;
            if (Random.Range(0.0f, 1.0f) <= 1.0f)
            {
                //Convinced to turn bad
                try
                {
                    NetworkGoodMemberBlackboard goodMemberBlackboard = _communityMemberBlackboard.MemberPair.GetComponent<NetworkGoodMemberBlackboard>();
                    goodMemberBlackboard.TurnBad();
                    NetworkLoseWinHandler.AddToBadList(_communityMemberBlackboard.MemberPair);
                    _communityMemberBlackboard.networkChatBubble.ChangeToBad();
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
