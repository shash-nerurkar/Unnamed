using System.Collections.Generic;
using UnityEngine;

public class FPSPlayerUI : MonoBehaviour
{
    void Awake() {}

    [Header("Interactions")]
    public FPSInteractablePromptPanel InteractablePromptPanel;

    [Header("Stats")]
    public ValueBar HealthBar;

    [Header("Ammo")]
    public FPSAmmoPanel AmmoPanel;

    [Header("Weapons")]
    [SerializeField] private List<FPSWeaponPanel> weaponPanels;
    [SerializeField] private RectTransform selectedWeaponIndicator;

    public void SetWeaponPanel(Sprite weaponSprite, int index) {
        weaponPanels[index].WeaponImage.sprite = weaponSprite;
    }

    public void SwitchWeapon(int currentWeaponIndex) {
        selectedWeaponIndicator.transform.position = weaponPanels[currentWeaponIndex].RectTransform.position;
    }
}
