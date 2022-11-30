using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricPlayerBasicAttack : MonoBehaviour
{
    public IsometricPlayer player;
    public Collider2D cd;

    // UPON ENTERING COLLISION
    void OnTriggerEnter2D(Collider2D enemy)
    {
        enemy.GetComponent<IsometricEnemy>().Damage(
            damage: player.attackDamage,
            hitterPosition: player.transform.position,
            hitKnockback: player.attackKnockback
        );
    }

    // UPON EXITING COLLISION
    void OnTriggerExit2D(Collider2D enemy)
    {}

}
