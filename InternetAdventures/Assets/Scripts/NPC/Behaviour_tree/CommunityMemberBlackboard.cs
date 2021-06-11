using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CommunityMemberBlackboard : AIBlackboard
{
    public GameObject MemberPair;
    [SerializeField] private float minTimer;
    [SerializeField] private float maxTimer;
    [SerializeField] private float memberProximity;
    
    protected override void AssembleBehaviourTree()
    {
        //Health=====================================================================================================================================
        CheckHealthNode checkHealthNode = new CheckHealthNode(this, criticalHealthThreshold);
        SequenceNode healthSequence = new SequenceNode(new List<Node>{new InverterNode(checkHealthNode)});
        
        //Grouping===================================================================================================================================
        RotateToMember rotateToMemberNode = new RotateToMember(this);
        RandomTimerAtPositionNode randomTimerAtPositionNode = new RandomTimerAtPositionNode(this, minTimer, maxTimer);
        PotentialMemberNode potentialMemberNode = new PotentialMemberNode(this, memberProximity);
        TraverseToMember traverseToMember = new TraverseToMember(this, memberProximity);
        SequenceNode groupingSelector = new SequenceNode(new List<Node>{rotateToMemberNode, randomTimerAtPositionNode, potentialMemberNode, traverseToMember});
        
        //Starting Node==============================================================================================================================
        _startingNode = new SelectorNode(new List<Node>() {healthSequence, groupingSelector});
    }
    
    public List<GameObject> GetAllNPCs()
    {
        List<GameObject> allNPCS = GameObject.FindGameObjectsWithTag("AI").ToList();
        allNPCS.Remove(gameObject);
        return allNPCS;
    }
}
