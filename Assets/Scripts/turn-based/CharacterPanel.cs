using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class CharacterPanel : MonoBehaviour
{
    public TextMeshProUGUI titleTextLabel;
    public ValueBarAsync healthBar;
    public ValueBarAsync energyBar;

    public async Task InitPanel(
        string title,
        int maxHealth,
        int maxEnergy
    ) {
        titleTextLabel.text = title;
        _ = healthBar.InitBar(maxHealth);
        await energyBar.InitBar(maxEnergy);
    }
}
