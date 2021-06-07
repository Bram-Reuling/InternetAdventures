using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]

public class AIBlackboard : MonoBehaviour
{
    //Public
    public GameObject MemberPair;
    [SerializeField] private float minTimer;
    [SerializeField] private float maxTimer;
    
    //Starting node
    private SelectorNode _startingNode;
    
    //Times
    [SerializeField] private float tickTimer;
    private float _timePassed;

    //NavMeshAgent
    public NavMeshAgent NavAgent => _navMesh;
    [SerializeField] private NavMeshAgent _navMesh;

    //NavMeshObstacle
    public NavMeshObstacle NavObstacle => navMeshObstacle;
    [SerializeField] private NavMeshObstacle navMeshObstacle;

    //Health
    public float CurrentHealth { get; private set; }
    [SerializeField] private float initialHealth;
    [SerializeField] private float criticalHealthThreshold;
    
    private void AssembleBehaviourTree()
    {
        //Health=====================================================================================================================================
        CheckHealthNode checkHealthNode = new CheckHealthNode(this, criticalHealthThreshold);
        SequenceNode healthSequence = new SequenceNode(new List<Node>() {new InverterNode(checkHealthNode)});
        
        //Grouping===================================================================================================================================
        RandomTimerAtPositionNode randomTimerAtPositionNode = new RandomTimerAtPositionNode(this, minTimer, maxTimer);
        PotentialMemberNode potentialMemberNode = new PotentialMemberNode(this, 3.0f);
        TraverseToMember traverseToMember = new TraverseToMember(this);
        SequenceNode groupingSelector = new SequenceNode(new List<Node>(){randomTimerAtPositionNode, potentialMemberNode, traverseToMember});
        
        //Starting Node==============================================================================================================================
        _startingNode = new SelectorNode(new List<Node>() {healthSequence, groupingSelector});
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
        List<GameObject> allNPCS = GameObject.FindGameObjectsWithTag("AI").ToList();
        allNPCS.Remove(gameObject);
        return allNPCS;
    }
}