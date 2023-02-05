using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSBullet : MonoBehaviour
{
    [SerializeField] private Collider cd;
    [SerializeField] private Rigidbody rb;

    private int damage;

    public void InitBullet(Vector3 direction, int damage, int moveSpeed) {
        transform.up = direction;
        rb.velocity = direction * moveSpeed;
        this.damage = damage;
    }

    // UPON ENTERING COLLISION
    async void OnTriggerEnter(Collider collided)
    {
        rb.velocity = Vector2.zero;
        cd.enabled = false;
        print(collided.name);
        collided.GetComponent<IFPSDamageable>()?.OnDamage(damage: damage);
        await System.Threading.Tasks.Task.Delay(1000);
        Destroy(gameObject);
    }
}
