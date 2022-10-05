using UnityEngine;

public class SideviewEdinmurghAttack1 : MonoBehaviour
{
    public SideviewEdinmurgh edinmurgh;
    public Collider2D cd;

    // AFTER BEING IN COLLISION
    void OnTriggerEnter2D(Collider2D player)
    {
        if(edinmurgh.animator.GetBool("IsAttacking1")) {
            player.gameObject.GetComponent<SideviewPlayer>().Damage(
                damage: edinmurgh.attack1Damage, 
                hitterPosition: edinmurgh.transform.position,
                hitKnockback: edinmurgh.attack1Knockback);
            cd.enabled = false;
        }
    }

    // AFTER BEING IN COLLISION
    void OnTriggerStay2D(Collider2D player)
    {
        if(edinmurgh.animator.GetBool("IsAttacking1")) {
            player.gameObject.GetComponent<SideviewPlayer>().Damage(
                damage: edinmurgh.attack1Damage, 
                hitterPosition: edinmurgh.transform.position,
                hitKnockback: edinmurgh.attack1Knockback);
            cd.enabled = false;
        }
    }
}
