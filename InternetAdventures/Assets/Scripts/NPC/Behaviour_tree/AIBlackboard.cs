using System.Collections.Generic;
using System.Linq;
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
    public NavMeshAgent NavAgent => _navMesh;
    [SerializeField] private NavMeshAgent _navMesh;

    //NavMeshObstacle
    public NavMeshObstacle NavObstacle => navMeshObstacle;
    [SerializeField] private NavMeshObstacle navMeshObstacle;

    //Health
    public float CurrentHealth { get; private set; }
    [SerializeField] protected float initialHealth;
    [SerializeField] protected float criticalHealthThreshold;

    //Animation
    [SerializeField] private Animator animator;

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
        animator.SetFloat("MovementSpeed", NavAgent.velocity.magnitude);
    }
}