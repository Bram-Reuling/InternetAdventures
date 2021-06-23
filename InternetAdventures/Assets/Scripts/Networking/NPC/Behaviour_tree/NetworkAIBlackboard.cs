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
    [SerializeField] private NavMeshAgent _navMeshAgent;

    public NavMeshData NavMesh => _navMesh;
    [SerializeField] private NavMeshData _navMesh;

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
        _navMeshAgent = GetComponent<NavMeshAgent>();
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
        CurrentHealth = initialHealth;
        //navMeshObstacle = GetComponent<NavMeshObstacle>();
        animator = transform.GetChild(1).GetComponent<Animator>();
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