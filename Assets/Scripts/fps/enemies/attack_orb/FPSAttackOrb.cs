using UnityEngine;

public class FPSAttackOrb : MonoBehaviour, IFPSDamageable
{
    [SerializeField] private ValueBar healthBar;
    private int health;
    
    void Awake() {
        health = 100;
        healthBar.InitBar(maxValue: health, setValue: true);
    }

    public void OnDamage(int value) {
        health = health - value < 0 ? 0 : health - value;
        healthBar.ChangeValue(value: health);
        if(health == 0) {
            Destroy(gameObject);
        }
        else {

        }
    }
}
