using UnityEngine;

public class IsometricEnemyWeapon : MonoBehaviour
{
    public IsometricEnemy enemy;
    public Collider2D cd;

    public void ActivateWeapon() {
        gameObject.SetActive(true);
        cd.enabled = true;
    }

    public void DeactivateWeapon() {
        gameObject.SetActive(false);
        cd.enabled = false;
    }

    // UPON ENTERING COLLISION
    void OnTriggerEnter2D(Collider2D player)
    {
        cd.enabled = false;
        player.GetComponent<IsometricPlayer>().Damage(
            damage: enemy.attackDamage,
            hitterPosition: player.transform.position,
            hitKnockback: enemy.attackKnockback
        );
    }

    // UPON EXITING COLLISION
    void OnTriggerExit2D(Collider2D other)
    {}
}
