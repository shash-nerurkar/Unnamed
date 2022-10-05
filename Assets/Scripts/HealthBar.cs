using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    Coroutine animatedHealthChange;

    public void InitBar(float maxHealth) {
        healthBar.maxValue = maxHealth;
    }

    public void ChangeHealth(float health) {
        if(animatedHealthChange != null) StopCoroutine(animatedHealthChange);
        animatedHealthChange = StartCoroutine(AnimateHealthChange(health));
    }
    
    IEnumerator AnimateHealthChange(float destinationHealth)
    {
        if(healthBar.value < destinationHealth) {
            while(healthBar.value < destinationHealth) {
                healthBar.value += 0.1f;
                yield return null;
            }
        }
        else {
            while(healthBar.value > destinationHealth) {
                healthBar.value -= 0.1f;
                yield return null;
            }
        }
    }
}
