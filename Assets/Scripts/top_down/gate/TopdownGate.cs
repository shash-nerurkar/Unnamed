using UnityEngine;

public class TopdownGate : MonoBehaviour
{
    public Transform spawn;
    public Animator animator;
    public Collider2D cd;
    public TopdownEnemy enemy;

    public void Open() {
        animator.SetBool("isOpen", true);
    }

    void OnOpened() {
        cd.enabled = false;
        enemy.ActivateAndMoveTo(transform.position);
    }

    public void Close() {
        animator.SetBool("isOpen", false);
    }

    void OnClosed() {
        cd.enabled = true;
    }
}
