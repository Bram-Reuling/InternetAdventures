using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]

public abstract class AIBlackboard : MonoBehaviour
{
    //Starting node
    protected SelectorNode _startingNode;
    
    //Times
    [SerializeField] private float tickTimer;
    private float _timePassed;

    //NavMeshAgent
    public NavMeshAgent NavAgent => _navMeshAgent;
    [SerializeField] private NavMeshAgent _navMeshAgent;

    //NavMeshObstacle
    public NavMeshObstacle NavObstacle => navMeshObstacle;
    [SerializeField] private NavMeshObstacle navMeshObstacle;

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
        InitializeData();
        AssembleBehaviourTree();
    }

    protected virtual void InitializeData()
    {
        CurrentHealth = initialHealth;
    }

    protected virtual void Update()
    {
        _timePassed += Time.deltaTime;
        
        if (_timePassed > tickTimer)
        {
            _timePassed = 0;
        }
        
        _startingNode.EvaluateState();
        animator.SetFloat(MovementSpeed, NavAgent.velocity.magnitude);
    }
}