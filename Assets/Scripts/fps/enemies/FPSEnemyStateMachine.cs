using UnityEngine;

public class FPSEnemyStateMachine : MonoBehaviour
{
    public FPSEnemyBaseState activeState;
    public FPSEnemyPatrolState patrolState;

    public void Initialize() {
        patrolState = new FPSEnemyPatrolState();
        ChangeState(newState: patrolState);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        activeState?.Perform();
    }

    public void ChangeState(FPSEnemyBaseState newState) {
        activeState?.Exit();

        activeState = newState;

        if(activeState != null) {
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<FPSEnemy>();
            activeState.Enter();
        }
    }
}
