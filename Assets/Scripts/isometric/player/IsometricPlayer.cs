using UnityEngine;
using UnityEngine.InputSystem;

public class IsometricPlayer : MonoBehaviour
{
    public PlayerInput playerMovementInput;
    public Collider2D cd;
    public Rigidbody2D rb;
    public Animator animator;
    public IsometricPlayerWeapon prism;
    public IsometricPlayerBasicAttack basicAttack;
    public IsometricShadow shadow;
    public IsometricHUDBottomPanel bottomPanel;
    
    // MOVEMENT VARIABLES
    float moveSpeed;
    Vector2 moveDirection; 
    Vector2 faceDirection = new(0, 1);

    // DAMAGE VARIABLES
    float health = 25;
    [HideInInspector] public int attackDamage;
    [HideInInspector] public int attackKnockback;
    Timer invincibilityTimer;
    readonly float invincibilityDuration = 2;

    // ATTACK VARIABLES
    enum AbilityStatus { Available, InProgress, Cooldown, }
    readonly AbilityStatus[] abilityStatuses = new AbilityStatus[5];
    readonly Timer[] abilityTimers = new Timer[5];
    readonly Timer[] abilityCooldownTimers = new Timer[5];
    int[] abilityCastTimes;
    int[] abilityCooldownTimes;
    public InputAction[] abilityActions = new InputAction[5];
    readonly float dashSpeed = 15;

    void Awake() {
        health = 25;
        moveSpeed = 3;
        attackDamage = 5;
        attackKnockback = 10;

        abilityCastTimes = new int[] {150, 500, 1000, 1000, 2500};
        abilityCooldownTimes = new int[] {1000, 2000, 3000, 4000, 5500};
    }

    void Start() {
        invincibilityTimer = gameObject.AddComponent<Timer>();
        for(int i = 0; i < 5; i++) {
            abilityTimers[i] = gameObject.AddComponent<Timer>();
            abilityCooldownTimers[i] = gameObject.AddComponent<Timer>();
            abilityActions[i].Enable();
            abilityStatuses[i] = AbilityStatus.Available;
        }
        bottomPanel.InitBottomPanel(
            maxHealth: health,
            abilityCooldownTimes: abilityCooldownTimes,
            abilityCooldownTimers: abilityCooldownTimers
        );
        abilityActions[0].started += context => Attack(0);
        abilityActions[1].started += context => Attack(1);
        abilityActions[2].started += context => Attack(2);
        abilityActions[3].started += context => Attack(3);
        abilityActions[4].started += context => Attack(4);
        PauseButton.GamePausedEvent += OnGamePaused;
        PauseButton.GameUnpausedEvent += OnGameUnpaused;
    }

    void OnGamePaused() {
        print("onGamePaused");
        playerMovementInput.enabled = false;
        foreach(InputAction abilityAction in abilityActions) {
            abilityAction.Disable();
        }
    }

    void OnGameUnpaused() {
        print("onGameUnpaused");
        playerMovementInput.enabled = true;
        foreach(InputAction abilityAction in abilityActions) {
            abilityAction.Enable();
        }
    }

    void Attack(int abilityNum) {
        if(abilityStatuses[abilityNum] != AbilityStatus.Available) return;
        animator.SetBool("ability" + abilityNum, true);
        abilityStatuses[abilityNum] = AbilityStatus.InProgress;
        abilityTimers[abilityNum].StartTimerWithIntParameter(
            maxTime: abilityCastTimes[abilityNum]/1000.0f, 
            onTimerFinishWithIntParam: OnAbilityFinish,
            intParam: abilityNum
        );
        bottomPanel.OnAbilityStart(abilityCastTimes[abilityNum], abilityTimers[abilityNum]);
        switch(abilityNum) {
            case 0:
                rb.AddForce(faceDirection * dashSpeed, ForceMode2D.Impulse);
                basicAttack.cd.enabled = true;
                break;

            case 1:
                prism.Attack(abilityNum: abilityNum);
                break;

            case 2:
                prism.Attack(abilityNum: abilityNum);
                break;

            case 3:
                prism.Attack(abilityNum: abilityNum);
                break;

            case 4:
                rb.velocity = Vector2.zero;
                prism.Attack(abilityNum: abilityNum);
                break;
        }     
    }

    public void OnAbilityFinish(int abilityNum) {
        animator.SetBool("ability" + abilityNum, false);
        abilityStatuses[abilityNum] = AbilityStatus.Cooldown;
        abilityCooldownTimers[abilityNum].StartTimerWithIntParameter(
            maxTime: abilityCooldownTimes[abilityNum]/1000.0f, 
            onTimerFinishWithIntParam: OnAbilityCooldownFinish,
            intParam: abilityNum
        );
        bottomPanel.OnAbilityFinish();
        bottomPanel.OnAbilityCooldownStart(abilityNum: abilityNum);
        switch(abilityNum) {
            case 0:
                basicAttack.cd.enabled = false;
                break;

            case 1:
                prism.OnAbilityFinish(abilityNum);
                break;

            case 2:
                prism.OnAbilityFinish(abilityNum);
                break;

            case 3:
                prism.OnAbilityFinish(abilityNum);
                break;

            case 4:
                prism.OnAbilityFinish(abilityNum);
                break;
        }
    }
    
    public void OnAbilityCooldownFinish(int abilityNum) {
        abilityStatuses[abilityNum] = AbilityStatus.Available;
        bottomPanel.OnAbilityCooldownFinish(abilityNum: abilityNum);
    }

    public void Damage(int damage, Vector3 hitterPosition, float hitKnockback) {
        health = health < damage ? 0 : health - damage;
        bottomPanel.healthBar.Changevalue(health);
        
        playerMovementInput.enabled = false;
        foreach(InputAction abilityAction in abilityActions) {
            abilityAction.Disable();
        }

        rb.velocity = Vector2.zero;
        cd.enabled = false;

        if(health == 0) {
            animator.Play("Death");
        }
        else {
            Vector2 hitDirection = new(
                Mathf.Cos(Vector2.Angle(hitterPosition, transform.position) * Mathf.Deg2Rad) * Mathf.Sign((transform.position - hitterPosition).x),
                Mathf.Sin(Vector2.Angle(hitterPosition, transform.position) * Mathf.Deg2Rad) * Mathf.Sign((hitterPosition - transform.position).y)
            );
            print("Player hitDirection:" + hitDirection + ", hitKnockback:" + hitKnockback + ", hitStrength:" + hitDirection*hitKnockback);
            rb.AddForce(hitDirection * hitKnockback, ForceMode2D.Impulse);
            animator.SetBool("isHit", true);
            animator.Play("Damage");
            invincibilityTimer.StartTimer(maxTime: invincibilityDuration, onTimerFinish: OnInvincibilityTimerFinished);
        }
    }

    void OnInvincibilityTimerFinished() {
        playerMovementInput.enabled = true;
        foreach(InputAction abilityAction in abilityActions) {
            abilityAction.Enable();
        }
        cd.enabled = true;
        animator.SetBool("isHit", false);
    }

    void OnDeath() {
        print(":( Player has fallen.");
    }

    void FaceDirection() {
        prism.FaceDirection(faceDirection);
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

    void Update() 
    {
        if(
            abilityStatuses[0] != AbilityStatus.InProgress &&
            abilityStatuses[4] != AbilityStatus.InProgress
        ) {
            if(moveDirection.magnitude != 0) faceDirection = moveDirection;
            animator.SetBool("isRunning", moveDirection.magnitude != 0);
            rb.velocity = moveDirection * moveSpeed;
        }
        else {
            animator.SetBool("isRunning", false);
        }
        FaceDirection();
    }

    void OnMove(InputValue inputValue) {
        moveDirection = inputValue.Get<Vector2>();
    }
}
