using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;

    public Rigidbody2D rb;
    public Animator animator;

    Vector2 movementDir;

    // Update is called once per frame, HENCE NO PHYSICS HERE! 
    void Update()
    {
        // INPUT
        movementDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // WALK ANIMATION PARAMETERS
        animator.SetFloat("Horizontal", movementDir.x);
        animator.SetFloat("Vertical", movementDir.y);
        animator.SetFloat("Speed", movementDir.sqrMagnitude);
    }

    void FixedUpdate() 
    {
        // MOVEMENT
        rb.MovePosition(rb.position + movementDir * moveSpeed * Time.fixedDeltaTime);
    }
}
