using System.Collections;
using UnityEngine;
using TMPro;

public class SideviewEdinmurgh : MonoBehaviour
{
    // COMPONENTS
    SideviewPlayer player;
    public Rigidbody2D rb;
    public Animator animator;
    public Collider2D cd;
    public SpriteRenderer spriteRenderer;
    public Collider2D attack1Collider;
    public Collider2D attack2Collider;
    public TextMeshProUGUI stateTextLabel;

    // MOVEMENT VARIABLES
    readonly float moveSpeed = 7;
    Vector2 moveDirection; 

    // ATTACK VARIABLES
    Vector3 playerPosition;

    // ATTACK 1 VARIABLES
    [HideInInspector] public readonly float attack1Damage = 5;
    [HideInInspector] public readonly float attack1Knockback = 10;
    readonly float activationDistanceAttack1 = 10;
    Timer flyTimeoutAttack1Timer;
    readonly float flyTimeoutAttack1Duration = 1.5f;
    
    // ATTACK 2 VARIABLES
    [HideInInspector] public readonly float attack2Damage = 5;
    [HideInInspector] public readonly float attack2Knockback = 10;
    readonly float activationDistanceAttack2 = 0.75f;
    Vector3 attack2InitialPosition;

    // DAMAGE VARIABLES
    float health = 25;

    // STATE VARIABLES
    enum States {
        Initial,
        Fly,
        Hit,
        Recover,
        Attack1,
        Attack2,
        Dying,
    }
    States state;
    bool stateLock = true;

    void Awake() {
        flyTimeoutAttack1Timer = this.gameObject.AddComponent<Timer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<SideviewPlayer>();
        transform.GetChild(3).GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void Damage(float damage, Vector3 hitterPosition, float hitKnockback) {
        if(state == States.Hit || state == States.Dying) return;

        // IF WAS ATTACKING, THEN DISABLE RELEVANT COLLIDERS AND ANIMATION HANDLING VARS
        animator.SetBool("IsAttacking1", false);
        animator.SetBool("IsAttacking2", false);
        attack1Collider.enabled = false;
        attack2Collider.enabled = false;

        health = health < damage ? 0 : health - damage;
        Vector2 hitDirection = new(
            Mathf.Cos(Vector2.Angle(hitterPosition, transform.position) * Mathf.Deg2Rad) * Mathf.Sign((transform.position - hitterPosition).x),
            Mathf.Sin(Vector2.Angle(hitterPosition, transform.position) * Mathf.Deg2Rad) * Mathf.Sign((hitterPosition - transform.position).y)
        );
        print("Enemy hitDirection:" + hitDirection + ", hitKnockback:" + hitKnockback + ", hitStrength:" + hitDirection*hitKnockback);

        if(health == 0) {
            ChangeState(States.Dying);
            if(!animator.GetBool("IsTouchingGround"))
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(hitDirection * hitKnockback, ForceMode2D.Impulse);
            }
        }
        else {
            ChangeState(States.Hit);
            rb.velocity = Vector2.zero;
            rb.AddForce(hitDirection * hitKnockback, ForceMode2D.Impulse);
        }
    }

    void OnDyingFinished() {
        // CLEAR CHARACTER HERE
    }

    void Attack1() {
        rb.velocity = Vector2.zero;
        playerPosition = player.transform.position;
        animator.SetBool("IsAttacking1", true);
    }

    void OnAttack1WindupFinished() {
        attack1Collider.enabled = true;
        float distance = Vector2.Distance(playerPosition, transform.position);
        float dirX = Mathf.Sign((playerPosition - transform.position).x);
        float time = 4f*1f/4f;
        float speedX = Mathf.Clamp(2*distance/time, 20, 35);
        rb.velocity = new Vector2(dirX*speedX, time*Physics.gravity.magnitude/2);
    }

    void OnAttack1Finished() {
        attack1Collider.enabled = false;
        animator.SetBool("IsAttacking1", false);
        animator.SetBool("IsRecovering", true);
        ChangeState(States.Recover);
    }

    void Attack2() {
        rb.velocity = Vector2.zero;
        attack2InitialPosition = transform.position;
        playerPosition = player.transform.position;
        transform.localScale = new Vector3(player.transform.position.x > transform.position.x ? -1 : 1, 1, 1);
        stateTextLabel.transform.localScale = transform.localScale;
        animator.SetBool("IsAttacking2", true);
    }

    void OnAttack2WindupFinished() {
        attack2Collider.enabled = true;
        StartCoroutine(Dash(startPosition: transform.position, endPosition: playerPosition, waitTime: ((1/30)*2)));
    }

    void OnAttack2HalfFinished() {
        StartCoroutine(Dash(startPosition: transform.position, endPosition: attack2InitialPosition, waitTime: ((1/24)*2)));
    }

    void OnAttack2Finished() {
        attack2Collider.enabled = false;
        animator.SetBool("IsAttacking2", false);
        ChangeState(States.Initial);
    }

    IEnumerator Dash(Vector3 startPosition, Vector3 endPosition, float waitTime) {
        for (float fractionDistanceCovered = 0; fractionDistanceCovered <= 1; fractionDistanceCovered += 0.1f)
        {
            rb.MovePosition(Vector3.Lerp(startPosition, endPosition, fractionDistanceCovered));
            yield return new WaitForSeconds(waitTime / 10); // BECAUSE THERE ARE 10 ITERATIONS IN THE LOOP
        }
    }

    public void OnFlyTimeoutAttack1TimerFinish() {
        ChangeState(States.Attack1);
    }

    void ChangeState(States state) {
        switch(state) {
            case States.Initial:
                stateLock = true;
                this.state = States.Initial;
                animator.Play("edinmurgh_idle");
                break;

            case States.Fly:
                stateLock = true;
                this.state = States.Fly;
                break;

            case States.Recover:
                stateLock = false;
                this.state = States.Recover;
                break;

            case States.Hit:
                stateLock = false;
                this.state = States.Hit;
                animator.SetBool("IsHit", true);
                animator.Play("edinmurgh_hit");
                flyTimeoutAttack1Timer.PauseTimer();
                break;

            case States.Attack1:
                stateLock = false;
                this.state = States.Attack1;
                flyTimeoutAttack1Timer.PauseTimer();
                Attack1();
                break;

            case States.Attack2:
                stateLock = false;
                this.state = States.Attack2;
                flyTimeoutAttack1Timer.PauseTimer();
                Attack2();
                break;

            case States.Dying:
                stateLock = false;
                this.state = States.Dying;
                flyTimeoutAttack1Timer.PauseTimer();
                break;
        }
    }

    void ChooseState() {
        float distanceFromPlayer = (player.transform.position - transform.position).magnitude;
        if(distanceFromPlayer >= activationDistanceAttack1) {
            ChangeState(States.Attack1);
        }
        else if(distanceFromPlayer <= activationDistanceAttack2) {
            ChangeState(States.Attack2);
        }
        else {
            if(state != States.Fly) {
                flyTimeoutAttack1Timer.StartTimer(maxTime: flyTimeoutAttack1Duration, onTimerFinish: OnFlyTimeoutAttack1TimerFinish);
            }
            ChangeState(States.Fly);
        }
    }

    void FixedUpdate() {
        if(stateLock) ChooseState();
        switch(state) {
            case States.Initial:
                transform.localScale = new Vector3(player.transform.position.x > transform.position.x ? -1 : 1, 1, 1);
                stateTextLabel.transform.localScale = transform.localScale;
                stateTextLabel.color = Color.white;
                stateTextLabel.text = "INITIAL";
                break;
                
            case States.Fly:
                transform.localScale = new Vector3(player.transform.position.x > transform.position.x ? -1 : 1, 1, 1);
                stateTextLabel.transform.localScale = transform.localScale;
                stateTextLabel.color = Color.cyan;
                stateTextLabel.text = "FLY";
                moveDirection = (player.transform.position - transform.position).normalized;
                rb.velocity = moveDirection * moveSpeed;
                break;

            case States.Hit:
                if(rb.velocity.magnitude < 0.3f)  {
                    animator.SetBool("IsHit", false);
                    ChangeState(States.Initial);
                }
                stateTextLabel.color = Color.red;
                stateTextLabel.text = "HIT";
                break;

            case States.Recover:
                if(rb.velocity.magnitude < 1) {  
                    animator.SetBool("IsRecovering", false);
                    ChangeState(States.Initial);
                }
                stateTextLabel.color = Color.green;
                stateTextLabel.text = "RECOVER";
                break;

            case States.Attack1:
                stateTextLabel.color = Color.yellow;
                stateTextLabel.text = "ATTACK 1";
                break;

            case States.Attack2:
                stateTextLabel.color = Color.magenta;
                stateTextLabel.text = "ATTACK 2";
                break;

            case States.Dying:
                if(animator.GetBool("IsTouchingGround"))
                    animator.Play("edinmurgh_die");
                stateTextLabel.color = Color.grey;
                stateTextLabel.text = "DYING";
                break;
        }
        animator.SetFloat("MoveSpeed", rb.velocity.magnitude);
    }
}
