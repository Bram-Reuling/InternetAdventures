using System.Collections.Generic;
using UnityEngine;

public class GoodMemberBlackboard : CommunityMemberBlackboard
{
    protected override void AssembleBehaviourTree()
    {
        //Health=====================================================================================================================================
        CheckHealthNode checkHealthNode = new CheckHealthNode(this, criticalHealthThreshold);
        SequenceNode healthSequence = new SequenceNode(new List<Node> {new InverterNode(checkHealthNode)});

        //Pairing====================================================================================================================================
        AtDestinationNode atDestinationNode = new AtDestinationNode(this);
        
        RandomTimerAtPositionNode randomTimerAtPositionNode = new RandomTimerAtPositionNode(this, minTimer, maxTimer);
        PotentialMemberNode potentialMemberNode = new PotentialMemberNode(this, memberProximity);
        TraverseToMember traverseToMember = new TraverseToMember(this, memberProximity);
        SequenceNode findNewMember = new SequenceNode(new List<Node>(){atDestinationNode, randomTimerAtPositionNode, potentialMemberNode, traverseToMember});

        HasMemberNode hasMemberNode = new HasMemberNode(this);
        RotateToMember rotateToMember = new RotateToMember(this);
        SequenceNode atMember = new SequenceNode(new List<Node>() {atDestinationNode, hasMemberNode, rotateToMember});

        SelectorNode pairing = new SelectorNode(new List<Node>() {findNewMember, atMember});
        
        //Starting Node==============================================================================================================================
        _startingNode = new SelectorNode(new List<Node>() {healthSequence, pairing});
    }

    public void TurnBad()
    {
        //Turn bad called
        Debug.Log(gameObject.name + " turned bad!");
        gameObject.AddComponent<BadMemberBlackboard>().InitializeData(minTimer, maxTimer, MemberPair);
        Destroy(this);
        //NavAgent.enabled = false;
        //NavObstacle.enabled = false;
    }
}