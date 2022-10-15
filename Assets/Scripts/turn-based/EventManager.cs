using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public class EventManager : MonoBehaviour
{
    public InputAction generalAction;
    public InputAction selectAttackAction;

    public BottomTextPanel bottomTextPanel;
    public Player player;
    public Enemy enemy;

    int selectedAttack = 0;
    GameStates state;
    PlayerAttacks currentPlayerAttack;
    EnemyAttacks currentEnemyAttack;
    
    async void Start()
    {
        await player.Init();
        await enemy.Init();
        _ = ChangeState(GameStates.PlayerSelectingAttack);
        generalAction.Enable();
        generalAction.started += context => OnGeneralAction();
        selectAttackAction.started += context => {
            switch(selectAttackAction.ReadValue<Vector2>()) {
                case Vector2 when selectAttackAction.ReadValue<Vector2>().Equals(Vector2.right):
                    if(selectedAttack == 0) selectedAttack = 1;
                    else if(selectedAttack == 2) selectedAttack = 3;
                    break;
                case Vector2 when selectAttackAction.ReadValue<Vector2>().Equals(Vector2.down):
                    if(selectedAttack == 0) selectedAttack = 2;
                    else if(selectedAttack == 1) selectedAttack = 3;
                    break;
                case Vector2 when selectAttackAction.ReadValue<Vector2>().Equals(Vector2.left):
                    if(selectedAttack == 1) selectedAttack = 0;
                    else if(selectedAttack == 3) selectedAttack = 2;
                    break;
                case Vector2 when selectAttackAction.ReadValue<Vector2>().Equals(Vector2.up):
                    if(selectedAttack == 2) selectedAttack = 0;
                    else if(selectedAttack == 3) selectedAttack = 1;
                    break;
            }
            bottomTextPanel.SelectAttack((PlayerAttacks)selectedAttack);
        };
    }

    async void OnGeneralAction() {
        generalAction.Disable();
        switch(state) {
            case GameStates.PlayerSelectingAttack:
                await ChangeState(GameStates.PlayerAttackingText);
                break;
            case GameStates.PlayerAttackingText:
                await ChangeState(GameStates.PlayerAttacking);
                break;
            case GameStates.PlayerAttacking:
                await ChangeState(enemy.GetHealth() == 0 ? GameStates.EnemyDyingText : GameStates.EnemyAttackingText);
                break;
            case GameStates.EnemyDyingText:
                await ChangeState(GameStates.EnemyDying);
                break;
            case GameStates.EnemyAttackingText:
                await ChangeState(GameStates.EnemyAttacking);
                break;
            case GameStates.EnemyAttacking:
                await ChangeState(player.GetHealth() == 0 ? GameStates.PlayerDyingText : GameStates.PlayerSelectingAttack);
                break;
            case GameStates.PlayerDyingText:
                await ChangeState(GameStates.PlayerDying);
                break;
            case GameStates.PlayerDying:
            case GameStates.EnemyDying:
                await ChangeState(GameStates.GameEnd);
                break;
            case GameStates.GameEnd:
                // EVENT END HERE
                break;
        }
        generalAction.Enable();
    }

    async Task ChangeState(GameStates state) {
        this.state = state;
        switch(this.state) {
            case GameStates.PlayerSelectingAttack:
                selectAttackAction.Enable();
                await bottomTextPanel.ShowFightPanelAsync("What would you like to do?");
                break;
            
            case GameStates.PlayerAttackingText:
                selectAttackAction.Disable();
                currentPlayerAttack = (PlayerAttacks)selectedAttack; 
                await bottomTextPanel.ShowTextPanelAsync(PromptStrings.playerAttackPrompts[currentPlayerAttack]);
                break;
            case GameStates.PlayerAttacking:
                await player.Attack(currentPlayerAttack);
                await bottomTextPanel.ShowTextPanelAsync("Its super effective!");
                break;
            
            case GameStates.EnemyDyingText:
                await bottomTextPanel.ShowTextPanelAsync("Your demons are collapsing");
                break;
            case GameStates.EnemyDying:
                await enemy.Dying();
                break;
            
            case GameStates.EnemyAttackingText:
                currentEnemyAttack = enemy.ChooseAttack(currentPlayerAttack);
                await bottomTextPanel.ShowTextPanelAsync("Dodge? u can't. The enemy is yourself.");
                break;
            case GameStates.EnemyAttacking:
                await enemy.Attack(currentEnemyAttack);
                await bottomTextPanel.ShowTextPanelAsync("Its super effective!");
                break;
            
            case GameStates.PlayerDyingText:
                await bottomTextPanel.ShowTextPanelAsync("You are losing");
                break;
            case GameStates.PlayerDying:
                await player.Dying();
                break;
            
            case GameStates.GameEnd:
                bottomTextPanel.gameObject.SetActive(false);
                if(enemy.GetHealth() == 0) {
                    enemy.panel.gameObject.SetActive(false);
                    enemy.gameObject.SetActive(false);
                }
                else {
                    player.panel.gameObject.SetActive(false);
                    player.gameObject.SetActive(false);
                }
                break;
        }
    }

    public void SelectAttack(int selectedAttack) {
        this.selectedAttack = selectedAttack;
        _ = ChangeState(GameStates.PlayerAttackingText);
    }
}
