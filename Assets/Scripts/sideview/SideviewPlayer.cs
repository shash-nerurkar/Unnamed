using UnityEngine;
using UnityEngine.InputSystem;

public class SideviewPlayer : MonoBehaviour
{
    // INPUT ACTIONS
    public InputAction jumpAction;
    public InputAction moveAction;
    public InputAction attackAction;

    // COMPONENTS
    public Rigidbody2D rb;
    public Animator animator;
    public Collider2D cd;
    public SpriteRenderer spriteRenderer;
    public CircleCollider2D weaponCollider;
    public SideviewPlayerWeapon weapon;
    public HealthBar healthBar;

    // MOVEMENT VARIABLES
    readonly float moveSpeed = 5;
    Vector2 moveDirection; 

    // ATTACK VARIABLES
    readonly float attackModeDuration = 4;
    Timer attackModeTimer;
    [HideInInspector] public readonly float attackDamage = 5;
    [HideInInspector] public readonly float attackKnockback = 10;

    // DAMAGE VARIABLES
    Timer invincibilityTimer;
    readonly float invincibilityDuration = 2;
    float health = 250;

    void Awake()
    {
        invincibilityTimer = this.gameObject.AddComponent<Timer>();
        attackModeTimer = this.gameObject.AddComponent<Timer>();
        healthBar.InitBar(health);
        healthBar.ChangeHealth(health);
    }

    public void Damage(float damage, Vector3 hitterPosition, float hitKnockback) {
        if(animator.GetBool("IsHit") || animator.GetBool("IsDying") || animator.GetBool("IsInvincible")) return;

        health = health < damage ? 0 : health - damage;
        healthBar.ChangeHealth(health);
        
        Vector2 hitDirection = new(
            Mathf.Cos(Vector2.Angle(hitterPosition, transform.position) * Mathf.Deg2Rad) * Mathf.Sign((transform.position - hitterPosition).x),
            Mathf.Sin(Vector2.Angle(hitterPosition, transform.position) * Mathf.Deg2Rad) * Mathf.Sign((hitterPosition - transform.position).y)
        );
        print("Player hitDirection:" + hitDirection + ", hitKnockback:" + hitKnockback + ", hitStrength:" + hitDirection*hitKnockback);

        jumpAction.Disable();
        moveAction.Disable();
        attackAction.Disable();
        
        if(health == 0) {
            animator.SetBool("IsDying", true);
            if(animator.GetBool("IsTouchingGround")) {
                animator.Play("player_die");
            }
            else {
                rb.velocity = Vector2.zero;
                rb.AddForce(hitDirection * hitKnockback, ForceMode2D.Impulse);
            }
        }
        else {
            animator.SetBool("IsHit", true);
            animator.Play("player_hit");
            rb.velocity = Vector2.zero;
            rb.AddForce(hitDirection * hitKnockback, ForceMode2D.Impulse);
            animator.SetBool("IsInvincible", true);
            invincibilityTimer.StartTimer(maxTime: invincibilityDuration, onTimerFinish: OnInvincibilityTimerFinished);
        }
    }

    void OnInvincibilityTimerFinished() {
        animator.SetBool("IsInvincible", false);
    }

    void OnDyingFinished() {
        // FINISH GAME HERE
    }

    void OnEnable()
    {
        jumpAction.Enable();
        moveAction.Enable();
        attackAction.Enable();
    }

    void OnDisable()
    {
        jumpAction.Disable();
        moveAction.Disable();
        attackAction.Disable();
    }

    void Start()
    {
        jumpAction.started += context => {
            if(animator.GetBool("IsTouchingGround")) 
                rb.AddForce(new Vector2(0, 10), ForceMode2D.Impulse);
        };

        moveAction.started += context => {
            spriteRenderer.transform.localScale = new Vector3(-moveAction.ReadValue<Vector2>().x, spriteRenderer.transform.localScale.y, 0);
        };

        attackAction.started += context => {
            if(animator.GetBool("IsTouchingGround")) {
                moveAction.Disable();
                jumpAction.Disable();
                attackModeTimer.StartTimer(maxTime: attackModeDuration, onTimerFinish: OnAttackModeTimerFinished);
                animator.SetBool("IsAttacking", true);
                animator.SetBool("IsInAttackMode", true);
            }
        };
    }

    void OnAttackWindupFinished() {
        weaponCollider.enabled = true;
    }

    void OnAttackFinished() {
        weaponCollider.enabled = false;
        moveAction.Enable();
        jumpAction.Enable();
        animator.SetBool("IsAttacking", false);
        weapon.ClearCollidedObjectIDList();
    }

    void OnAttackModeTimerFinished() {
        animator.SetBool("IsInAttackMode", false);
    }

    void FixedUpdate()
    {
        if(animator.GetBool("IsHit")) 
        {
            if(rb.velocity.magnitude < 1) {
                animator.SetBool("IsHit", false);
                jumpAction.Enable();
                moveAction.Enable();
                attackAction.Enable();
            }
        }
        else {
            moveDirection = moveAction.ReadValue<Vector2>();
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
        }
        if(animator.GetBool("IsTouchingGround")) {
            if(animator.GetBool("IsDying"))
                animator.Play("player_die");
            else
                animator.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.x));
        }
    }
}
