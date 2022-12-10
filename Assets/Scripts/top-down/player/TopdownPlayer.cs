using UnityEngine;
using UnityEngine.InputSystem;

public class TopdownPlayer : MonoBehaviour
{
    public PlayerInput playerMovementInput;
    public Collider2D cd;
    public Rigidbody2D rb;
    public Animator animator;
    public ValueBar healthBar;
    
    // MOVEMENT VARIABLES
    float moveSpeed;
    Vector2 moveDirection; 
    Vector2 faceDirection = new(0, 1);

    // DAMAGE VARIABLES
    float health = 25;
    Timer invincibilityTimer;
    readonly float invincibilityDuration = 2;

    // ATTACK VARIABLES
    public InputAction dashAction;
    Timer dashTimer;
    float dashTime;
    float dashCooldownTime;
    float dashSpeed;

    void Awake() {
        health = 25;
        moveSpeed = 3;
        dashTime = 0.3f;
        dashCooldownTime = 3;
        dashSpeed = 15;
    }

    void Start() {
        dashTimer = gameObject.AddComponent<Timer>();
        invincibilityTimer = gameObject.AddComponent<Timer>();
        PauseButton.GamePausedEvent += OnGamePaused;
        PauseButton.GameUnpausedEvent += OnGameUnpaused;
        healthBar.InitBar(maxValue: health, setValue: true);
        dashAction.Enable();
        dashAction.performed += ctx => { Dash(); };
    }

    void OnGamePaused() {
        print("onGamePaused");
        playerMovementInput.enabled = false;
        dashAction.Disable();
    }

    void OnGameUnpaused() {
        print("onGameUnpaused");
        playerMovementInput.enabled = true;
        dashAction.Enable();
    }

    void Dash() {
        if(dashTimer.IsRunning) return;

        animator.SetBool("isDashing", true);
        dashTimer.StartTimer(
            maxTime: dashTime, 
            onTimerFinish: OnDashFinish
        );
        
        rb.AddForce(faceDirection * dashSpeed, ForceMode2D.Impulse);
    }

    public void OnDashFinish() {
        animator.SetBool("isDashing", false);
        
        dashTimer.StartTimer(
            maxTime: dashCooldownTime, 
            onTimerFinish: OnDashCooldownFinish
        );
    }
    
    public void OnDashCooldownFinish() {}

    public void Damage(int damage, Vector3 hitterPosition, float hitKnockback) {
        health = health < damage ? 0 : health - damage;
        healthBar.Changevalue(value: health);
        
        playerMovementInput.enabled = false;
        dashAction.Disable();

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
        dashAction.Disable();
        cd.enabled = true;
        animator.SetBool("isHit", false);
    }

    void OnDeath() {
        print(":( Player has fallen.");
    }

    void FaceDirection() {
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, faceDirection);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720 * Time.deltaTime);
        rb.MoveRotation(rotation);
    }

    void Update() 
    {
        if(moveDirection.magnitude != 0) faceDirection = moveDirection;
        animator.SetBool("isRunning", moveDirection.magnitude != 0);

        if(!animator.GetBool("isDashing")) {
            rb.velocity = moveDirection * moveSpeed;
            FaceDirection();
        }
    }

    void OnMove(InputValue inputValue) {
        moveDirection = inputValue.Get<Vector2>();
    }
}
