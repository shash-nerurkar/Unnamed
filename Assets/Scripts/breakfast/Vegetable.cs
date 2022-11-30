using System.Collections;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class Vegetable : MonoBehaviour
{
    // COMPONENTS
    public Rigidbody2D rb;
    public Animator animator;
    public Collider2D lateralCollider;
    public CircleCollider2D longitudinalCollider;
    public SpriteRenderer spriteRenderer;
    public Transform topLeft;
    public Transform bottomRight;
    public TextMeshProUGUI stateTextLabel;
    public CircleCollider2D bowlCollider;
    public CuttingBoard cuttingBoard;
    public Knife knife;
    public TextMeshProUGUI cutVegetablesListLabel;

    // MOVEMENT VARIABLES
    Vector3 jumpLocation;
    Vector3 jumpDirection;
    Vector3 topLeftPosition;
    Vector3 bottomRightPosition;

    // CONFUSED VARIABLES
    Vector3 confusedDirection;
    [SerializeField] private float confusedSpeed;

    // CAUGHT VARIABLES
    Hand hand;
    [HideInInspector] public bool isInBowl = true;
    [HideInInspector] public bool isOnCuttingBoard = false;
    [HideInInspector] public bool isCuffed = false;

    // FLEE VARIABLES
    Timer fleeCooldownTimer;
    Coroutine rotateToPathCoroutine;
    Coroutine followPathCoroutine;

    // CUT VARIABLES
    int cutsDone = 0;
    public int maxCuts;

    // STATE VARIABLES
    public enum States {
        Free,
        Confused,
        FleeInit,
        Flee,
        Cuffed,
        Caught,
        FinalJump,
        Dying,
    }
    States state = States.Free;


    void Awake() {
        fleeCooldownTimer = gameObject.AddComponent<Timer>();
        hand = FindObjectOfType<Hand>();
        topLeftPosition = topLeft.position + new Vector3(spriteRenderer.bounds.size.x/2, -spriteRenderer.bounds.size.y/2);
        bottomRightPosition += bottomRight.position + new Vector3(-spriteRenderer.bounds.size.x/2, spriteRenderer.bounds.size.y/2);
        ChangeState(States.Free);
    }

    void StartFleeCooldownTimer(float additionalTime = 0) {
        if(fleeCooldownTimer.IsRunning) return;
        
        float fleeCooldownTime = additionalTime + Random.Range(5, 25);
        fleeCooldownTimer.StartTimer(maxTime: fleeCooldownTime, onTimerFinish: OnFleeCooldownFinish);
    }

    void OnFleeCooldownFinish() {
        ChangeState(States.FleeInit);
    }

    public void Cut() {
        if(cutsDone == maxCuts) {
            cutVegetablesListLabel.text += "| " + name + " ";
            knife.RemoveVegetable();
            cuttingBoard.ToggleVegetableStatus();
            Destroy(gameObject);
        }
        else {
            cutsDone += 1;
            animator.SetInteger("Cuts", cutsDone);
        }
    }

    IEnumerator RotateToPath(Quaternion endRotation) {
        if(rotateToPathCoroutine != null) StopCoroutine(rotateToPathCoroutine);

        Quaternion startRotation = transform.rotation;
        for(float i = 0; i <= 1; i+=0.01f) {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, i);
            yield return null;
        }
        ChangeState(States.Flee);
    }

    IEnumerator FollowPath(Vector3 endPosition, States nextState) {
        if(followPathCoroutine != null) StopCoroutine(followPathCoroutine);
        
        Vector3 startPosition = transform.position;
        float iterationDuration = Mathf.Clamp((endPosition - startPosition).magnitude, 1, 5);
        for(float i = 0; i <= 1; i+=0.001f*iterationDuration) {
            transform.position = Vector3.Lerp(startPosition, endPosition, i);
            yield return null;
        }
        ChangeState(nextState);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.collider.tag == "Bowl") {
            isInBowl = true;
        }
        else if(collision.collider.tag == "Cutting board") {
            isOnCuttingBoard = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if(collision.collider.tag == "Bowl") {
            isInBowl = false;
        }
        else if(collision.collider.tag == "Cutting board") {
            isOnCuttingBoard = false;
        }
    }

    public void ChangeState(States state) {
        if(this.state == States.Cuffed) return;

        this.state = state;
        switch(state) {
            case States.Free:
                if(isInBowl) {
                    ChangeState(States.Confused);
                }
                else if(isOnCuttingBoard && 
                        cuttingBoard.GetVegetable() == null && 
                        transform.position.x >= cuttingBoard.transform.position.x - cuttingBoard.cd.bounds.extents.x + lateralCollider.bounds.extents.x &&
                        transform.position.x <= cuttingBoard.transform.position.x + cuttingBoard.cd.bounds.extents.x - lateralCollider.bounds.extents.x && 
                        transform.position.y >= cuttingBoard.transform.position.y - cuttingBoard.cd.bounds.extents.y + lateralCollider.bounds.extents.y &&
                        transform.position.y <= cuttingBoard.transform.position.y + cuttingBoard.cd.bounds.extents.y - lateralCollider.bounds.extents.y) {
                    cuttingBoard.ToggleVegetableStatus(vegetable: this);
                    ChangeState(States.Cuffed);
                }
                else {
                    ChangeState(States.FleeInit);
                }
                break;
            
            case States.Confused:
                animator.SetBool("IsCaught", false);
                animator.SetBool("IsConfused", true);
                longitudinalCollider.enabled = true;
                lateralCollider.enabled = false;
                confusedDirection = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 1);
                StartFleeCooldownTimer(additionalTime: 0);
                break;

            case States.FleeInit:
                animator.SetBool("IsCaught", false);
                animator.SetBool("IsRunning", true);
                longitudinalCollider.enabled = true;
                lateralCollider.enabled = false;
                if (Random.value < 0.5f) {
                    jumpLocation = new(topLeftPosition.x, Random.Range(bottomRightPosition.y, topLeftPosition.y), 0);
                }
                else {
                    jumpLocation = new(Random.Range(topLeftPosition.x, bottomRightPosition.x), bottomRightPosition.y, 0);
                }
                jumpDirection = (jumpLocation - transform.position).normalized;
                rotateToPathCoroutine = StartCoroutine(RotateToPath(endRotation: Quaternion.AngleAxis(Mathf.Atan2(jumpDirection.y, jumpDirection.x) * Mathf.Rad2Deg, Vector3.forward)));
                break;

            case States.Flee:
                followPathCoroutine = StartCoroutine(FollowPath(endPosition: jumpLocation, nextState: States.FinalJump));
                break;

            case States.Cuffed:
                isCuffed = true;
                break;

            case States.Caught:
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsConfused", false);
                animator.SetBool("IsCaught", true);
                longitudinalCollider.enabled = false;
                lateralCollider.enabled = true;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                if(cuttingBoard.GetVegetable() == this) {
                    cuttingBoard.ToggleVegetableStatus();
                }
                if(rotateToPathCoroutine != null) StopCoroutine(rotateToPathCoroutine);
                if(followPathCoroutine != null) StopCoroutine(followPathCoroutine);
                break;

            case States.FinalJump:
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsJumping", true);
                followPathCoroutine = StartCoroutine(FollowPath(endPosition: jumpLocation + jumpDirection*2, nextState: States.Dying));
                break;

            case States.Dying:
                animator.Play(stateName: "Die");
                lateralCollider.enabled = false;
                longitudinalCollider.enabled = false;
                break;
        }
    }

    void FixedUpdate() {
        switch(state) {
            case States.Free:
                stateTextLabel.color = Color.white;
                stateTextLabel.text = "FREE";
                break;
                
            case States.Confused:
                if(Vector3.Distance(transform.position + confusedDirection*0.3f, bowlCollider.transform.position) >= bowlCollider.radius - longitudinalCollider.radius) {
                    confusedDirection = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 1);
                }
                rb.MovePosition(transform.position + confusedDirection*confusedSpeed*Time.deltaTime);
                stateTextLabel.color = Color.cyan;
                stateTextLabel.text = "CONFUSED";
                break;

            case States.FleeInit:
                stateTextLabel.color = Color.yellow;
                stateTextLabel.text = "FLEE INITIAL";
                break;

            case States.Flee:
                stateTextLabel.color = Color.green;
                stateTextLabel.text = "FLEE";
                break;

            case States.Cuffed:
                stateTextLabel.color = Color.magenta;
                stateTextLabel.text = "CUFFED";
                break;

            case States.Caught:
                transform.position = hand.transform.position;
                stateTextLabel.color = Color.red;
                stateTextLabel.text = "CAUGHT";
                break;

            case States.FinalJump:
                stateTextLabel.color = Color.magenta;
                stateTextLabel.text = "FINAL JUMP";
                break;

            case States.Dying:
                stateTextLabel.color = Color.grey;
                stateTextLabel.text = "DYING";
                break;
        }
    }
}
