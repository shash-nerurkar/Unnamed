using UnityEngine;

public class SideviewFloorCollider : MonoBehaviour
{
    public Animator playerAnimator;
    // UPON ENTERING COLLISION
    void OnTriggerEnter2D(Collider2D other)
    {
        // PLAYER COMES IN CONTACT WITH GROUND
        playerAnimator.SetBool("IsTouchingGround", true);
    }

    // UPON EXITING COLLISION
    void OnTriggerExit2D(Collider2D other)
    {
        // PLAYER BREAKS CONTACT WITH GROUND
        playerAnimator.SetBool("IsTouchingGround", false);
    }
}
