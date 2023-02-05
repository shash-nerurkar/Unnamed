using UnityEngine;

public class TopdownBulletBasic : MonoBehaviour
{
    public Collider2D cd;
    public Rigidbody2D rb;
    public Animator animator;

    readonly float moveSpeed = 2;
    readonly int damage = 3;
    readonly float knockback = 5;

    // Start is called before the first frame update
    public void Initialize(Vector3 direction, Transform parent)
    {
        transform.parent = parent;
        transform.up = direction;
        rb.velocity = direction * moveSpeed;
    }

    // UPON ENTERING COLLISION
    async void OnTriggerEnter2D(Collider2D collided)
    {
        rb.velocity = Vector2.zero;
        cd.enabled = false;
        collided.GetComponent<TopdownPlayer>()?.Damage(
            damage: damage,
            hitterPosition: transform.position,
            hitKnockback: knockback
        );
        await System.Threading.Tasks.Task.Delay(1000);
        Destroy(gameObject);
    }
}
