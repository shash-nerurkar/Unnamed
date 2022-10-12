using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterPanel : MonoBehaviour
{
    public TextMeshProUGUI titleTextLabel;
    public ValueBar healthBar;
    public ValueBar energyBar;

    public void InitPanel(
        string title,
        int maxHealth,
        int maxEnergy
    ) {
        titleTextLabel.text = title;
        healthBar.InitBar(maxHealth);
        energyBar.InitBar(maxEnergy);
    }
}
