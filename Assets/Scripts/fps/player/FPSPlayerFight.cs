using System.Collections.Generic;
using UnityEngine;

public class FPSPlayerFight : MonoBehaviour, IFPSWeaponUser
{
    void Awake() {
        showHUDDamageEffectTimer = gameObject.AddComponent<Timer>();
        showHUDHealEffectTimer = gameObject.AddComponent<Timer>();
        player.UI.HealthBar.InitBar(maxValue: health, setValue: true);
        IsAttacking = false;
        playerWeapons = new List<IFPSWeapon>();
        foreach(IFPSWeapon weapon in gameObject.GetComponentsInChildren<IFPSWeapon>()) {
            playerWeapons.Add(weapon);
        }
        currentWeaponIndex = 1;
        List<Sprite> weaponSprites = new();
        foreach(IFPSWeapon weapon in playerWeapons) {
            player.UI.SetWeaponPanel(weaponSprite: weapon.Image, index: playerWeapons.IndexOf(weapon));
        }
        OnWeaponSwitch(switchDirection: -1);
    }

    void Update() {
        playerWeapons[currentWeaponIndex].Attack(IsAttacking: IsAttacking);
    }

    [Header("Health")]
    [SerializeField] private FPSPlayer player;
    [SerializeField] private Animator damageVolumeAnimator;
    [SerializeField] private Animator healVolumeAnimator;

    private Timer showHUDDamageEffectTimer;
    private Timer showHUDHealEffectTimer;
    int health = 100;

    public void TakeDamage(int damage) {
        if(damage > 0) {
            damageVolumeAnimator.Play("show_damage");
            showHUDDamageEffectTimer.StartTimer(maxTime: 2f, onTimerFinish: OnShowHUDDamageEffectTimerFinish);
        }
        else {
            healVolumeAnimator.Play("show_heal");
            showHUDHealEffectTimer.StartTimer(maxTime: 1f, onTimerFinish: OnShowHUDHealEffectTimerFinish);
        }
        health = Mathf.Clamp(health - damage, 0, health - damage);
        player.UI.HealthBar.ChangeValue(value: health);
    }

    void OnShowHUDDamageEffectTimerFinish() {
        damageVolumeAnimator.Play("hide_damage");
    }

    void OnShowHUDHealEffectTimerFinish() {
        healVolumeAnimator.Play("hide_heal");
    }


    [Header("Weapon")]
    [SerializeField] private Camera cam;

    public List<IFPSWeapon> playerWeapons { get; private set;}
    public int currentWeaponIndex { get; private set;}
    [HideInInspector] public bool IsAttacking { get; set; }

    public void OnWeaponAttack(bool IsAttacking) {
        this.IsAttacking = IsAttacking;
    }

    public void OnWeaponReload() {
        if(playerWeapons[currentWeaponIndex] is FPSGun) 
            (playerWeapons[currentWeaponIndex] as FPSGun).Reload();
    }

    public void OnWeaponSwitch(float switchDirection) {
        if(switchDirection == -1)
            currentWeaponIndex = currentWeaponIndex == 0 ? playerWeapons.Count - 1 : currentWeaponIndex - 1;
        else 
            currentWeaponIndex = currentWeaponIndex == playerWeapons.Count - 1 ? 0 : currentWeaponIndex + 1;
        ChooseCurrentWeapon();
    }

    public void OnWeaponPickup(IFPSWeapon floorWeapon) {
        if(playerWeapons[currentWeaponIndex] is FPSGun) 
            if(!(playerWeapons[currentWeaponIndex] as FPSGun).CanDrop()) 
                return;
        
        playerWeapons[currentWeaponIndex].Drop(floorWeaponTransform: floorWeapon.WeaponTransform);
        floorWeapon.PickUp();
        player.UI.SetWeaponPanel(weaponSprite: floorWeapon.Image, index: currentWeaponIndex);
        playerWeapons[currentWeaponIndex] = floorWeapon;
    }

    private void ChooseCurrentWeapon() {
        for(int i = 0; i < playerWeapons.Count; i++) {
            if(i == currentWeaponIndex) playerWeapons[i].SwitchIn();
            else playerWeapons[i].SwitchOut();
        }

        player.UI.SwitchWeapon(currentWeaponIndex: currentWeaponIndex);
    }
}
