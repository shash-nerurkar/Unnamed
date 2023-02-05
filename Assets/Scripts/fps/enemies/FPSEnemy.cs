using UnityEngine;
using UnityEngine.AI;

public class FPSEnemy : MonoBehaviour
{
    private FPSEnemyStateMachine stateMachine;
    private NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; }

    [SerializeField] private string currentState;

    public FPSEnemyPath path;

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = GetComponent<FPSEnemyStateMachine>();
        agent = GetComponent<NavMeshAgent>();
        
        stateMachine.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
