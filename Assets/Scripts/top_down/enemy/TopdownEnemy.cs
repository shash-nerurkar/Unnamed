using UnityEngine;
using TMPro;

public class TopdownEnemy : MonoBehaviour
{
    public TopdownPlayer player;
    public TopdownGate gate;
    public TopdownEnemyTurret turret;
    public TopdownEnemyHull hull;
    public TextMeshProUGUI stateTextLabel;
    public Canvas canvas;
    public CircleRenderer attackMaxIndicator;
    public CircleRenderer attackMinIndicator;
    public TopdownBulletBasic bulletBasic;
    public GameObject bulletContainer;

    // MOVE VARIABLES
    Vector3 moveDestinationPosition;

    // GATE VARIABLES
    bool shouldGoInactive = false;

    // ATTACK VARIABLES
    [HideInInspector] public int attackDamage;
    [HideInInspector] public int attackKnockback;
    Timer attackCooldownTimer;
    float attackCooldownTime;
    float attackActivationDistanceMax;
    float attackActivationDistanceMin;

    // PATTERN TYPES
    enum MoveStates { Scan, Idle, Count }
    MoveStates stateOutOfSight;
    MoveStates stateInSight;

    // STATE VARIABLES
    enum States { Inactive, Idle, Look, Move, Attack, Patrol }
    States state;
    bool stateLock = false;

    void Awake() {
        attackDamage = 5;
        attackKnockback = 10;
        attackCooldownTime = 3;
        
        stateOutOfSight = (MoveStates)Random.Range(0, (int)MoveStates.Count);
        switch(stateOutOfSight) {
            case MoveStates.Idle:
                break;

            case MoveStates.Scan:
                break;
        }   
        
        stateInSight = (MoveStates)Random.Range(0, (int)MoveStates.Count);
        switch(stateInSight) {
            case MoveStates.Idle:
                break;

            case MoveStates.Scan:
                break;
        }   
        
        attackActivationDistanceMin = 1;
        attackActivationDistanceMax = 6;

        state = States.Inactive;
    }

    void Start() {
        canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        attackCooldownTimer = gameObject.AddComponent<Timer>();
        attackCooldownTimer.StartTimer(maxTime: attackCooldownTime, onTimerFinish: OnAttackCooldownTimerFinish);
        PauseButton.GamePausedEvent += OnGamePaused;
        PauseButton.GameUnpausedEvent += OnGameUnpaused;
        attackMaxIndicator.DrawCircle(radius: attackActivationDistanceMax, color: Color.blue, width: 0.1f);
        attackMinIndicator.DrawCircle(radius: attackActivationDistanceMin, color: Color.red, width: 0.1f);
    }

    void OnGamePaused() {
        print("onGamePaused");
    }

    void OnGameUnpaused() {
        print("onGameUnpaused");
    }

    public void DeactivateAndCloseGate() {
        shouldGoInactive = false;
        ChangeState(States.Inactive);
        gate.Close();
    }

    public void MoveToAndDeactivate(Vector3 destinationPosition) {
        this.moveDestinationPosition = gate.spawn.position;
        ChangeState(States.Move);
        shouldGoInactive = true;
    }

    public void ActivateAndMoveTo(Vector3 destinationPosition) {
        this.moveDestinationPosition = destinationPosition;
        ChangeState(States.Move);
    }

    async void Attack() {
        // animator.SetBool("isAttacking", true);
        await System.Threading.Tasks.Task.Delay(1000);
        TopdownBulletBasic bullet = Instantiate(bulletBasic, turret.crosshair.position, Quaternion.identity);
        bullet.Initialize(direction: turret.transform.up, parent: bulletContainer.transform);
        // animator.SetBool("isAttacking", false);
        attackCooldownTimer.StartTimer(maxTime: attackCooldownTime, onTimerFinish: OnAttackCooldownTimerFinish);
        stateLock = true;
    }

    void OnAttackCooldownTimerFinish() {}

    void ChangeState(States state) {
        if(this.state == state) return;
        
        switch(this.state) {
            case States.Inactive:
                break;
                
            case States.Idle:
                break;

            case States.Look:
                break;

            case States.Move:
                if(shouldGoInactive) DeactivateAndCloseGate();
                break;

            case States.Patrol:
                break;

            case States.Attack:
                break;
        }
    
        switch(state) {
            case States.Inactive:
                stateLock = false;
                break;

            case States.Idle:
                stateLock = true;
                break;

            case States.Look:
                stateLock = true;
                break;

            case States.Move:
                stateLock = false;
                hull.MoveToDestination(moveDestinationPosition);
                break;

            case States.Patrol:
                stateLock = true;
                break;

            case States.Attack:
                stateLock = false;
                Attack();
                break;
        }
    
        this.state = state;
    }

    void ChooseState() {
        float distanceFromPlayer = (player.transform.position - transform.position).magnitude;

        RaycastHit2D playerRayHit = Physics2D.Raycast(
            origin: turret.crosshair.transform.position,
            direction: (player.transform.position - transform.position).normalized,
            distance: Mathf.Infinity,
            layerMask: LayerMask.GetMask("Wall") | LayerMask.GetMask("Player hitbox")
        );

        if(distanceFromPlayer > attackActivationDistanceMax) {
            ChangeState(States.Idle);
        }
        else if(playerRayHit.collider != null && playerRayHit.collider.gameObject != player.gameObject) {
            switch(stateOutOfSight) {
                case MoveStates.Idle:
                    ChangeState(States.Idle);
                    break;
                
                case MoveStates.Scan:
                    ChangeState(States.Look);
                    break;
            }   
        } 
        else {
            RaycastHit2D turretPlayerRayHit = Physics2D.Raycast(
                origin: turret.crosshair.transform.position,
                direction: turret.transform.up,
                distance: Mathf.Infinity,
                layerMask: LayerMask.GetMask("Wall") | LayerMask.GetMask("Player hitbox")
            );

            switch(stateInSight) {
                case MoveStates.Idle:
                    if(distanceFromPlayer > attackActivationDistanceMin && attackCooldownTimer.TimeRemaining == 0)
                        ChangeState(States.Attack);
                    else
                        ChangeState(States.Idle);
                    break;
                
                case MoveStates.Scan:
                    if(turretPlayerRayHit.collider != null && turretPlayerRayHit.collider.gameObject == player.gameObject && 
                        distanceFromPlayer > attackActivationDistanceMin && attackCooldownTimer.TimeRemaining == 0)
                        ChangeState(States.Attack);
                    else
                        ChangeState(States.Look);
                    break;
            }
        }
    }

    void FixedUpdate() {
        if(stateLock) ChooseState();
        switch(state) {
            case States.Inactive:
                stateTextLabel.color = Color.gray;
                stateTextLabel.text = "INACTIVE";
                break;

            case States.Idle:
                stateTextLabel.color = Color.white;
                stateTextLabel.text = "IDLE";
                break;
                
            case States.Look:
                stateTextLabel.color = Color.cyan;
                stateTextLabel.text = "LOOK";
                turret.UpdateTarget(player.transform.position - transform.position);
                break;

            case States.Move:
                if((moveDestinationPosition - transform.position).magnitude < 0.1) stateLock = true;
                stateTextLabel.color = Color.green;
                stateTextLabel.text = "MOVE";
                break;

            case States.Attack:
                stateTextLabel.color = Color.yellow;
                stateTextLabel.text = "ATTACK";
                break;

            case States.Patrol:
                stateTextLabel.color = Color.magenta;
                stateTextLabel.text = "PATROL";
                break;
        }
    }
}
