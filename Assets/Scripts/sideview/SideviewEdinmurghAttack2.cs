using UnityEngine;

public class SideviewEdinmurghAttack2 : MonoBehaviour
{
    public SideviewEdinmurgh edinmurgh;
    public Collider2D cd;

    // AFTER BEING IN COLLISION
    void OnTriggerEnter2D(Collider2D player)
    {
        if(edinmurgh.animator.GetBool("IsAttacking2")) {
            player.gameObject.GetComponent<SideviewPlayer>().Damage(
                damage: edinmurgh.attack2Damage,
                hitterPosition: transform.position,
                hitKnockback: edinmurgh.attack2Knockback);
            cd.enabled = false;
        }
    }

    // AFTER BEING IN COLLISION
    void OnTriggerStay2D(Collider2D player)
    {
        if(edinmurgh.animator.GetBool("IsAttacking2")) {
            player.gameObject.GetComponent<SideviewPlayer>().Damage(
                damage: edinmurgh.attack2Damage,
                hitterPosition: transform.position,
                hitKnockback: edinmurgh.attack2Knockback);
            cd.enabled = false;
        }
    }
}
