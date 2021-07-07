using Mirror;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(NavMeshAgent))]

public abstract class NetworkAIBlackboard : NetworkBehaviour
{
    //Starting node
    protected SelectorNode _startingNode;
    
    //Times
    [SerializeField] private float tickTimer;
    private float _timePassed;

    //NavMeshAgent
    public NavMeshAgent NavAgent => _navMeshAgent;
    [SerializeField] protected NavMeshAgent _navMeshAgent;

    public NavMeshObstacle NavObstacle => _navMeshObstacle;
    [SerializeField] protected NavMeshObstacle _navMeshObstacle;
    
    public NavMeshData NavMesh => _navMesh;
    [SerializeField] protected NavMeshData _navMesh;

    [SerializeField] protected bool alwaysGoRandom;
    
    //Health
    public float CurrentHealth { get; private set; }
    [SerializeField] protected float initialHealth;
    [SerializeField] protected float criticalHealthThreshold;

    //Animation
    [SerializeField] private Animator animator;
    private static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");

    protected abstract void AssembleBehaviourTree();
    
    private void Start()
    {
        ServerStart();
    }

    [ServerCallback]
    private void ServerStart()
    {
        InitializeData();
        AssembleBehaviourTree();
    }

    protected virtual void InitializeData()
    {
        initialHealth = 100;
        CurrentHealth = initialHealth;
        animator = transform.GetChild(1).GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        criticalHealthThreshold = 20;
    }

    [ServerCallback]
    protected virtual void Update()
    {
        _timePassed += Time.deltaTime;
        
        if (_timePassed > tickTimer)
        {
            _timePassed = 0;
        }

        if (_startingNode == null) return;
        
        _startingNode.EvaluateState();
        animator.SetFloat(MovementSpeed, NavAgent.velocity.magnitude);
    }
}