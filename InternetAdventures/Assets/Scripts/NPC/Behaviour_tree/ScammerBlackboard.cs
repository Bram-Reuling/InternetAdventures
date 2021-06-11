using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScammerBlackboard : AIBlackboard
{
    [SerializeField] private GameObject objectToFollow;
    [SerializeField] private float distanceToObject;
    
    protected override void AssembleBehaviourTree()
    {
        FollowObjectNode followObjectNode = new FollowObjectNode(this, objectToFollow, distanceToObject);
        SequenceNode followSequence = new SequenceNode(new List<Node>() {followObjectNode});

        _startingNode = new SelectorNode(new List<Node>() {followSequence});
    }
}