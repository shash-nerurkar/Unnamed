using System.Threading.Tasks;
using UnityEngine;

public class TurnBasedEnemy : MonoBehaviour
{
    public CharacterPanel panel;
    public TurnBasedPlayer player;
    int health;
    int energy;
    readonly int maxHealth = 5;
    readonly int maxEnergy = 100;
    public int GetHealth() => health;
    public int GetEnergy() => energy;
    
    public async Task Init()
    {
        health = maxHealth;
        energy = maxEnergy;
        await panel.InitPanel(title: "Enemy", maxHealth: health, maxEnergy: energy);
    }

    public async Task Damage(int amount) {
        health = health <= amount ? 0 : health - amount;
        await panel.healthBar.Changevalue(value: health);
    }

    public async Task Attack(EnemyAttacks enemyAttacks) {
        switch(enemyAttacks) {
            case EnemyAttacks.Attack1:
                print("In Enemy: Attack 1");
                break;
            case EnemyAttacks.Attack2:
                print("In Enemy: Attack 2");
                break;
            case EnemyAttacks.Attack3:
                print("In Enemy: Attack 3");
                break;
            case EnemyAttacks.Attack4:
                print("In Enemy: Attack 4");
                break;
        }
        // AWAIT FOR ATTACK ANIMATION
        await Task.Delay(1500);
        // ON HIT, CALL ENEMY DAMAGE FUNCTION
        await player.Damage(5);
    }

    public async Task Dying() {
        // AWAIT FOR DEATH ANIMATION DURATION
        await Task.Delay(1000);
        print("ENEMY DIED");
    }

    public EnemyAttacks ChooseAttack(
        PlayerAttacks lastPlayerAttack
    ) {
        int playerHealth = player.GetHealth();
        int playerEnergy = player.GetEnergy();

        return (EnemyAttacks)Random.Range(0, 4);
    }
}
