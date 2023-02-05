public abstract class FPSEnemyBaseState
{
    public FPSEnemy enemy;
    public FPSEnemyStateMachine stateMachine;

    public abstract void Enter();
    public abstract void Perform();
    public abstract void Exit();
}
