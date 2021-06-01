using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]

public class AIBlackboard : MonoBehaviour
{
    //Starting node
    private SelectorNode _startingNode;
    
    //Times
    [SerializeField] private float tickTimer;
    private float _timePassed;

    //NavMeshAgent
    public NavMeshAgent NavAgent => _navMesh;
    [SerializeField] private NavMeshAgent _navMesh;

    //Health
    public float CurrentHealth { get; private set; }
    [SerializeField] private float initialHealth;
    [SerializeField] private float criticalHealthThreshold;
    
    private void AssembleBehaviourTree()
    {
        //Health=====================================================================================================================================
        CheckHealthNode checkHealthNode = new CheckHealthNode(this, criticalHealthThreshold);
        SequenceNode healthSequence = new SequenceNode(new List<Node>() {new InverterNode(checkHealthNode)});
        
        //Patrol=====================================================================================================================================
        PatrolNode patrolNode = new PatrolNode(this);
        SequenceNode patrolSequence = new SequenceNode(new List<Node>() {patrolNode});
        
        //Starting Node==============================================================================================================================
        _startingNode = new SelectorNode(new List<Node>() {healthSequence, patrolSequence});
    }

    private void Start()
    {
        InitializeData();
        AssembleBehaviourTree();
    }

    private void InitializeData()
    {
        CurrentHealth = initialHealth;
    }

    private void Update()
    {
        _timePassed += Time.deltaTime;
        
        if (_timePassed > tickTimer)
        {
            _timePassed = 0;
        }
        
        _startingNode.EvaluateState();
    }

    public List<GameObject> GetAllNPCs()
    {
        return GameObject.FindGameObjectsWithTag("AI").ToList();
    }
}