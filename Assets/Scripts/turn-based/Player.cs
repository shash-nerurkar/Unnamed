using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterPanel panel;
    public Enemy enemy;
    int health;
    int energy;
    readonly int maxHealth = 20;
    readonly int maxEnergy = 100;
    public int GetHealth() => health;
    public int GetEnergy() => energy;
    
    public async Task Init()
    {
        health = maxHealth;
        energy = maxEnergy;
        await panel.InitPanel(title: "Player", maxHealth: health, maxEnergy: energy);
    }

    public async Task Damage(int amount) {
        health = health <= amount ? 0 : health - amount;
        await panel.healthBar.Changevalue(value: health);
    }

    public async Task Attack(PlayerAttacks playerAttack) {
        switch(playerAttack) {
            case PlayerAttacks.Attack1:
                print("In Player: Attack 1");
                break;
            case PlayerAttacks.Attack2:
                print("In Player: Attack 2");
                break;
            case PlayerAttacks.Attack3:
                print("In Player: Attack 3");
                break;
            case PlayerAttacks.Attack4:
                print("In Player: Attack 4");
                break;
        }
        // AWAIT FOR ATTACK ANIMATION
        await Task.Delay(1500);
        // ON HIT, CALL ENEMY DAMAGE FUNCTION
        await enemy.Damage(5);
    }

    public async Task Dying() {
        // AWAIT FOR DEATH ANIMATION DURATION
        await Task.Delay(1000);
        print("PLAYER DIED");
    }
}
