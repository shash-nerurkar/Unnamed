using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public CharacterPanel panel;
    readonly int maxHealth = 20;
    readonly int maxEnergy = 100;
    
    void Start()
    {
        panel.InitPanel(title: "Enemy", maxHealth: maxHealth, maxEnergy: maxEnergy);
    }

    void Update()
    {
        
    }
}
