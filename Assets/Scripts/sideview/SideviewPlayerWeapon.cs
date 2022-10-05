using System.Collections.Generic;
using UnityEngine;

public class SideviewPlayerWeapon : MonoBehaviour
{
    public SideviewPlayer player;
    List<float> collidedObjectIDList = new();

    // AFTER BEING IN COLLISION
    void OnTriggerStay2D(Collider2D enemy)
    {
        if(player.animator.GetBool("IsAttacking")) {
            if(collidedObjectIDList.Contains(enemy.gameObject.GetInstanceID()))
                return;
            else
                collidedObjectIDList.Add(enemy.gameObject.GetInstanceID());
            enemy.gameObject.GetComponent<SideviewEdinmurgh>().Damage(
                damage: player.attackDamage,
                hitterPosition: player.transform.position,
                hitKnockback: player.attackKnockback);
        }
    }

    public void ClearCollidedObjectIDList() {
        collidedObjectIDList = new();
    }
}
