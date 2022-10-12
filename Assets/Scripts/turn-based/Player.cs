using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterPanel panel;
    readonly int maxHealth = 20;
    readonly int maxEnergy = 100;
    
    void Start()
    {
        panel.InitPanel(title: "Player", maxHealth: maxHealth, maxEnergy: maxEnergy);
    }

    void Update()
    {
        
    }
}
