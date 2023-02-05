using UnityEngine;
using TMPro;
using Pathfinding;

public class IsometricEnemy : MonoBehaviour
{
    IsometricPlayer player;
    public Rigidbody2D rb;
    public Collider2D cd;
    public Animator animator;
    public IsometricEnemyWeapon weapon;
    public IsometricShadow shadow;
    public TextMeshProUGUI stateTextLabel;
    public Canvas canvas;
    public CircleRenderer sightIndicator;
    public CircleRenderer attackIndicator;
    public ValueBar healthBar;
    public AIPath aIPath;
    public AIDestinationSetter aIDestinationSetter;
    
    // MOVEMENT VARIABLES
    float moveSpeed;
    Vector2 moveDirection; 
    Vector2 faceDirection = new(0, 1);

    // ATTACK VARIABLES
    int health;
    [HideInInspector] public int attackDamage;
    [HideInInspector] public int attackKnockback;
    Timer attackCooldownTimer;
    float attackCooldownTime;
    float attackActivationDistance;
    Timer invincibilityTimer;
    float invincibilityDuration;

    // PATTERN TYPES
    enum MovePattern { Idle, Patrol, Approach, Count }
    MovePattern movePattern;
    enum MovePatternSighted { Approach, Patrol, Encircle, Count }
    MovePatternSighted movePatternSighted;
    float sightActivationDistance;

    // ENCIRCLE VARIABLES
    float encircleActivationDistance;
    Timer encircleChangeTimer;
    float encircleDirectionDuration;
    Vector3 encircleDirection;

    // PATROL VARIABLES
    bool changePatrolDirectionFlag = true;
    Vector2 patrolDirection;
    enum PatrolState { Go, Return }
    PatrolState patrolState;

    // GOTO VARIABLES
    Vector2 goToStartPosition;
    Vector2 goToEndPosition;

    // STATE VARIABLES
    enum States {
        Idle,
        Move,
        Encircle,
        GoTo,
        Attack,
        Hit,
        Dying,
    }
    States state;
    bool stateLock = true;

    void Awake() {
        health = 25;
        moveSpeed = 3;
        attackDamage = 5;
        attackKnockback = 10;
        attackCooldownTime = 3;
        invincibilityDuration = 2;
        aIPath.maxSpeed = moveSpeed;

        movePattern = MovePattern.Approach;//(MovePattern)Random.Range(0, (int)MovePattern.Count);
        switch(movePattern) {
            case MovePattern.Idle:
                break;
            
            case MovePattern.Patrol:
                patrolState = PatrolState.Go;
                break;

            case MovePattern.Approach:
                break;
        }    
        
        movePatternSighted = MovePatternSighted.Approach;//(MovePatternSighted)Random.Range(0, (int)MovePatternSighted.Count);
        switch(movePatternSighted) {
            case MovePatternSighted.Approach:
                attackActivationDistance = weapon.cd.bounds.extents.magnitude;
                break;
            
            case MovePatternSighted.Encircle:
                encircleActivationDistance = weapon.cd.bounds.extents.magnitude * 3;
                attackActivationDistance = encircleActivationDistance;
                break;

            case MovePatternSighted.Patrol:
                attackActivationDistance = weapon.cd.bounds.extents.magnitude;
                patrolState = PatrolState.Go;
                break;
        }

        sightActivationDistance = weapon.cd.bounds.extents.magnitude * 7;
    }

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<IsometricPlayer>();
        aIDestinationSetter.target = player.transform;
        canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(); 
        encircleChangeTimer = gameObject.AddComponent<Timer>();
        attackCooldownTimer = gameObject.AddComponent<Timer>();
        attackCooldownTimer.StartTimer(maxTime: attackCooldownTime, onTimerFinish: OnAttackCooldownTimerFinish);
        weapon.gameObject.SetActive(false);
        PauseButton.GamePausedEvent += OnGamePaused;
        PauseButton.GameUnpausedEvent += OnGameUnpaused;
        sightIndicator.DrawCircle(radius: sightActivationDistance, color: Color.blue, width: 0.1f);
        attackIndicator.DrawCircle(radius: attackActivationDistance, color: Color.red, width: 0.1f);
        healthBar.InitBar(maxValue: health, setValue: true);
        invincibilityTimer = gameObject.AddComponent<Timer>();
    }

    void OnGamePaused() {
        print("onGamePaused");
    }

    void OnGameUnpaused() {
        print("onGameUnpaused");
    }

    async void Attack() {
        animator.SetBool("isAttacking", true);
        weapon.ActivateWeapon();
        AnimateFaceDirection(faceDirection);
        rb.velocity = Vector2.zero;
        await System.Threading.Tasks.Task.Delay(1000);
        animator.SetBool("isAttacking", false);
        weapon.DeactivateWeapon();
        attackCooldownTimer.StartTimer(maxTime: attackCooldownTime, onTimerFinish: OnAttackCooldownTimerFinish);
        stateLock = true;
    }

    public void Damage(int damage, Vector3 hitterPosition, float hitKnockback) {
        health = health < damage ? 0 : health - damage;
        healthBar.ChangeValue(health);
        
        if(health == 0) {
            ChangeState(States.Dying);
            animator.Play("Death");
        }
        else {
            ChangeState(States.Hit);
            Vector2 hitDirection = new(
                Mathf.Cos(Vector2.Angle(hitterPosition, transform.position) * Mathf.Deg2Rad) * Mathf.Sign((transform.position - hitterPosition).x),
                Mathf.Sin(Vector2.Angle(hitterPosition, transform.position) * Mathf.Deg2Rad) * Mathf.Sign((hitterPosition - transform.position).y)
            );
            print("Player hitDirection:" + hitDirection + ", hitKnockback:" + hitKnockback + ", hitStrength:" + hitDirection*hitKnockback);
            rb.velocity = Vector2.zero;
            rb.AddForce(hitDirection * hitKnockback, ForceMode2D.Impulse);
            animator.SetBool("isHit", true);
            animator.Play("Damage");
            invincibilityTimer.StartTimer(maxTime: invincibilityDuration, onTimerFinish: OnInvincibilityTimerFinished);
        }
    }

    void OnInvincibilityTimerFinished() {
        ChangeState(States.Idle);
        animator.SetBool("isHit", false);
    }

    void OnDeath() {
        print("Enemy down");
    }

    void OnAttackCooldownTimerFinish() {}

    void OnEncircleChangeTimerFinish() {
        encircleDirectionDuration = Random.Range(0.5f, 1f);
        encircleDirection = new(0, 0, Random.value > 0.5 ? 1 : -1);
        if(state == States.Encircle)
            encircleChangeTimer.StartTimer(maxTime: encircleDirectionDuration, onTimerFinish: OnEncircleChangeTimerFinish);
    }

    void SetPatrolDirection() {
        if(!changePatrolDirectionFlag) return;
        else changePatrolDirectionFlag = false;

        RaycastHit2D objectInWay;
        do {
            float patrolAngle = Random.Range(-180, 180);
            patrolDirection = Quaternion.AngleAxis(patrolAngle, Vector3.up) * new Vector2(1, Mathf.Sign(patrolAngle)) * 5;
            objectInWay = Physics2D.CircleCast(
                origin: goToStartPosition,
                direction: patrolDirection,
                radius: cd.bounds.extents.y,
                distance: 5 - cd.bounds.extents.y,
                layerMask: LayerMask.GetMask("Wall") | LayerMask.GetMask("Player hitbox")
            );
            
            // Debug.DrawRay(r.origin, r.direction*(5 - cd.bounds.extents.y), Color.green, duration: 5);
        } while(objectInWay.collider != null); 
    }

    void AnimateFaceDirection(Vector2 faceDirection) {
        shadow.FaceDirection(faceDirection);
        if(faceDirection.x == 0 && faceDirection.y > 0) {
            transform.localScale = new(1, 1, 1);
            animator.SetInteger("faceDirection", 0);
        }
        else if(faceDirection.x > 0 && faceDirection.y > 0) {
            transform.localScale = new(1, 1, 1);
            animator.SetInteger("faceDirection", 1);
        }
        else if(faceDirection.x > 0 && faceDirection.y == 0) {
            transform.localScale = new(1, 1, 1);
            animator.SetInteger("faceDirection", 2);
        }
        else if(faceDirection.x > 0 && faceDirection.y < 0) {
            transform.localScale = new(1, 1, 1);
            animator.SetInteger("faceDirection", 3);
        }
        else if(faceDirection.x == 0 && faceDirection.y < 0) {
            transform.localScale = new(1, 1, 1);
            animator.SetInteger("faceDirection", 4);
        }
        else if(faceDirection.x < 0 && faceDirection.y < 0) {
            transform.localScale = new(-1, 1, 1);
            animator.SetInteger("faceDirection", 3);
        }
        else if(faceDirection.x < 0 && faceDirection.y == 0) {
            transform.localScale = new(-1, 1, 1);
            animator.SetInteger("faceDirection", 2);
        }
        else if(faceDirection.x < 0 && faceDirection.y > 0) {
            transform.localScale = new(-1, 1, 1);
            animator.SetInteger("faceDirection", 1);
        }
    }

    void ChangeState(States state) {
        if(this.state == state) return;
        
        switch(this.state) {
            case States.Idle:
                break;

            case States.Move:
                aIPath.enabled = false;
                break;

            case States.Encircle:
                break;
            
            case States.GoTo:
                break;

            case States.Attack:
                break;

            case States.Hit:
                break;

            case States.Dying:
                break;
        }
    
        switch(state) {
            case States.Idle:
                stateLock = true;
                this.state = state;
                break;

            case States.Move:
                stateLock = true;
                this.state = state;
                aIPath.enabled = true;
                break;

            case States.Encircle:
                stateLock = true;
                this.state = state;
                OnEncircleChangeTimerFinish();
                break;
            
            case States.GoTo:
                stateLock = true;
                this.state = state;
                if(patrolState == PatrolState.Go) {
                    goToStartPosition = transform.position;
                    SetPatrolDirection();
                    goToEndPosition = goToStartPosition + patrolDirection;
                    patrolState = PatrolState.Return;
                }
                else {
                    (goToEndPosition, goToStartPosition) = (goToStartPosition, goToEndPosition);
                    patrolState = PatrolState.Go;
                }
                break;

            case States.Attack:
                stateLock = false;
                this.state = state;
                Attack();
                break;

            case States.Hit:
                stateLock = false;
                this.state = state;
                break;

            case States.Dying:
                stateLock = false;
                this.state = state;
                break;
        }
    }

    void ChooseState() {
        float distanceFromPlayer = (player.transform.position - transform.position).magnitude;

        RaycastHit2D playerRayHit = Physics2D.Raycast(
            origin: transform.position,
            direction: (player.transform.position - transform.position).normalized,
            distance: Mathf.Infinity,
            layerMask: LayerMask.GetMask("Wall") | LayerMask.GetMask("Player hitbox")
        );

        if(distanceFromPlayer > sightActivationDistance) {
            ChangeState(States.Idle);
        }
        else if(playerRayHit.collider.gameObject != player.gameObject) {
            switch(movePattern) {
                case MovePattern.Idle:
                    ChangeState(States.Idle);
                    break;
                
                case MovePattern.Patrol:
                    ChangeState(States.GoTo);
                    break;

                case MovePattern.Approach:
                    ChangeState(States.Move);
                    break;
            }   
        } 
        else {
            if(distanceFromPlayer <= attackActivationDistance && attackCooldownTimer.TimeRemaining == 0) {
                ChangeState(States.Attack);
                changePatrolDirectionFlag = true;
            }
            else {
                switch(movePatternSighted) {
                    case MovePatternSighted.Approach:
                        if(distanceFromPlayer > attackActivationDistance)
                            ChangeState(States.Move);
                        else
                            ChangeState(States.Idle);
                        break;
                    
                    case MovePatternSighted.Encircle:
                        if(distanceFromPlayer <= encircleActivationDistance)
                            ChangeState(States.Encircle);
                        else
                            ChangeState(States.Move);
                        break;

                    case MovePatternSighted.Patrol:
                        ChangeState(States.GoTo);
                        break;
                }
            }
        }
    }

    void FixedUpdate() {
        moveDirection = (player.transform.position - transform.position).normalized;
        if(moveDirection.magnitude != 0) {
            faceDirection = new(
                Mathf.Round(moveDirection.x + Mathf.Sign(moveDirection.x)*0.125f), 
                Mathf.Round(moveDirection.y + Mathf.Sign(moveDirection.y)*0.125f)
            );
        }

        if(stateLock) ChooseState();
        switch(state) {
            case States.Idle:
                stateTextLabel.color = Color.white;
                stateTextLabel.text = "IDLE";
                rb.velocity = Vector2.zero;
                break;
                
            case States.Move:
                stateTextLabel.color = Color.cyan;
                stateTextLabel.text = "MOVE";
                AnimateFaceDirection(faceDirection);
                // rb.velocity = moveDirection * moveSpeed;
                rb.velocity = new(0.0001f, 0);
                break;

            case States.Encircle:
                stateTextLabel.color = Color.cyan;
                stateTextLabel.text = "ENCIRCLE";
                AnimateFaceDirection(faceDirection);
                transform.RotateAround(player.transform.position, encircleDirection, 15 * moveSpeed * Time.deltaTime);
                transform.rotation = new Quaternion(0, 0, 0, 0);
                rb.velocity = new(0.0001f, 0);
                break;

            case States.GoTo:
                stateTextLabel.color = Color.cyan;
                stateTextLabel.text = "GOTO";
                AnimateFaceDirection((goToEndPosition - goToStartPosition).normalized);
                if(Vector3.Distance(transform.position, goToEndPosition) < 0.1f) ChangeState(States.Idle);
                rb.velocity = (goToEndPosition - goToStartPosition).normalized * moveSpeed * 2/3;
                break;

            case States.Attack:
                stateTextLabel.color = Color.yellow;
                stateTextLabel.text = "ATTACK";
                break;

            case States.Hit:
                stateTextLabel.color = Color.red;
                stateTextLabel.text = "HIT";
                break;

            case States.Dying:
                stateTextLabel.color = Color.grey;
                stateTextLabel.text = "DYING";
                break;
        }

        animator.SetBool("isRunning", rb.velocity.magnitude != 0);
    }
}