using UnityEngine;
using System.Threading.Tasks;

public class BottomTextPanel : MonoBehaviour
{
    public GameObject textPanel;
    public TextDisplay textPanelLabel;

    public GameObject fightPanel;
    public TextDisplay fightPanelLabel;

    
    public Transform selectedAttackIndicatorTransform;
    public Transform attack1IndicatorTransform;
    public Transform attack2IndicatorTransform;
    public Transform attack3IndicatorTransform;
    public Transform attack4IndicatorTransform;

    public async Task ShowFightPanelAsync(string text) {
        fightPanel.SetActive(true);
        textPanel.SetActive(false);
        await fightPanelLabel.ShowTextAsync(text);
    }

    public async Task ShowTextPanelAsync(string text) {
        textPanel.SetActive(true);
        fightPanel.SetActive(false);
        await textPanelLabel.ShowTextAsync(text);
    }
    
    public void SelectAttack(PlayerAttacks playerAttack) {
        switch(playerAttack) {
            case PlayerAttacks.Attack1:
                selectedAttackIndicatorTransform.position = attack1IndicatorTransform.position;
                break;
            case PlayerAttacks.Attack2:
                selectedAttackIndicatorTransform.position = attack2IndicatorTransform.position;
                break;
            case PlayerAttacks.Attack3:
                selectedAttackIndicatorTransform.position = attack3IndicatorTransform.position;
                break;
            case PlayerAttacks.Attack4:
                selectedAttackIndicatorTransform.position = attack4IndicatorTransform.position;
                break;
        }
    }
}
