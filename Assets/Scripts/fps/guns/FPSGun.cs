using System.Collections;
using UnityEngine;

public abstract class FPSGun : MonoBehaviour, IFPSWeapon
{
    public FPSGunData gunData;
    [SerializeField] protected Sprite gunImage;
    [SerializeField] protected Transform muzzle;
    [SerializeField] protected Animator animator;
    [SerializeField] protected FPSWeaponPickable WeaponPickable;

    public Sprite Image { get; set; }
    public Transform WeaponTransform { get; set; }
    public Transform EquippedTransform { get; set; }
    public Transform UnequippedTransform { get; set; }
    public Transform ZoomedInTransform { get; set; }
    private Transform shootParent;
    private FPSPlayer player;
    private Transform playerCamera;
    private GameObject floorWeaponsContainer;

    public bool CanShoot() => !IsShooting && !animator.GetBool("IsReloading") && !animator.GetBool("IsInitializing");
    public bool CanDrop() => !IsShooting && !animator.GetBool("IsInitializing");
    protected bool IsShooting;
    protected Coroutine jerkGunCoroutine;

    void Awake() {
        player = FindObjectOfType<FPSPlayer>();
        playerCamera = GameObject.FindGameObjectWithTag("Player Camera").transform;
        floorWeaponsContainer = GameObject.FindGameObjectWithTag("FPS Weapon Container");
        WeaponTransform = transform;
        EquippedTransform = transform.GetChild(2).transform;
        UnequippedTransform = transform.GetChild(3).transform;
        ZoomedInTransform = transform.GetChild(4).transform;
        Image = gunImage;

        gunData.OnInit();
        SetDataString();
        IsShooting = false;
    }

    public void SwitchIn() {
        transform.SetParent(playerCamera);
        shootParent = playerCamera;

        transform.localPosition = EquippedTransform.localPosition;
        transform.localRotation = EquippedTransform.localRotation;

        animator.enabled = true;
        // if(gunData.currentAmmo == 0)
        //     animator.SetBool("IsReloading", true);
        // else 
        animator.SetBool("IsInitializing", true);
        player.UI.AmmoPanel.UpdatePanel(currentWeaponAmmo: gunData.currentAmmo, totalWeaponAmmo: gunData.currentAmmo);
    }

    public void OnInitializationFinished() {
        SetBasePosition();
    }

    public void SwitchOut() {
        transform.SetParent(player.transform);

        SetBasePosition();
    }

    public void PickUp() {
        WeaponPickable.gameObject.SetActive(false);

        SwitchIn();
    }

    public void Drop(Transform floorWeaponTransform) {
        WeaponPickable.gameObject.SetActive(true);
        
        transform.SetParent(floorWeaponsContainer.transform);
        if(animator.enabled) SetBasePosition();
        SetDataString();
        transform.localPosition = floorWeaponTransform.localPosition;
        transform.localRotation = floorWeaponTransform.localRotation;
    }

    private void SetBasePosition() {
        animator.SetBool("IsInitializing", false);
        animator.SetBool("IsReloading", false);
        animator.enabled = false;
        transform.localPosition = UnequippedTransform.localPosition;
        transform.localRotation = UnequippedTransform.localRotation;
    }

    public void SetDataString() {
        WeaponPickable.SetWeaponPickupMessage(
            dataString: "Name: " + gunData.name + " | " + "Damage: " + gunData.bulletData.damage + " | " + 
                        "Recoil : " + gunData.recoil + " | " + "Ammo : " + gunData.currentAmmo
        );
    }

    public void SetShootDirection(Transform shootParent) {
        this.shootParent = shootParent;
    }

    public virtual void Attack(bool IsAttacking) {
        switch(gunData.gunShootType) {
            case GunShootType.SemiAutomatic:
                if(IsAttacking) {
                    if(!gunData.canFire) return;
                    
                    gunData.canFire = false;
                    ShootGun();
                }
                else {
                    gunData.canFire = true;
                }
                break;
                
            case GunShootType.BurstFire:
                if(IsAttacking) {
                    if(gunData.currentBurstFireCount == 0) return;

                    gunData.currentBurstFireCount--;
                    ShootGun();
                }
                else {
                    gunData.currentBurstFireCount = gunData.burstFireCount;
                }
                break;
                
            case GunShootType.FullyAutomatic:
                if(!IsAttacking) return;

                ShootGun();
                break;
        }
    }

    public void ShootGun() {
        if(gunData.currentAmmo <= 0) return;
        if(!CanShoot()) return;
        
        if(jerkGunCoroutine != null) StopCoroutine(jerkGunCoroutine);
        jerkGunCoroutine = StartCoroutine(JerkGun());
        if(Physics.Raycast(
            shootParent.transform.position, 
            shootParent.transform.forward, 
            out RaycastHit hitInfo, 
            gunData.maxDistance, 
            layerMask: LayerMask.GetMask("Enemy hitbox") | LayerMask.GetMask("Wall"))) {
                IFPSDamageable damageable = hitInfo.collider.GetComponent<IFPSDamageable>();
                if(damageable != null) {
                    damageable.OnDamage(damage: gunData.bulletData.damage);
                }
                else {
                    // show bullet hole
                }
        }
        gunData.currentAmmo--;
        player.UI.AmmoPanel.UpdatePanel(currentWeaponAmmo: gunData.currentAmmo, totalWeaponAmmo: gunData.currentAmmo);
        if(gunData.currentAmmo == 0) Reload();
    }

    protected IEnumerator JerkGun() {
        IsShooting = true;

        float time = 0, duration = gunData.recoilDuration;
        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = transform.localPosition + 
                                new Vector3(
                                    Random.Range(-gunData.recoil, gunData.recoil), 
                                    Random.Range(-gunData.recoil, gunData.recoil), 
                                    0
                                );
        while (time < duration) {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = endPosition;
        
        time = 0; 
        duration = gunData.recoverDuration;
        endPosition = startPosition;
        startPosition = transform.localPosition;
        while (time < duration) {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = endPosition;
        
        IsShooting = false;
    }

    public void Reload() {
        if(gunData.currentAmmo >= gunData.magSize) return;

        animator.enabled = true;
        animator.SetBool("IsReloading", true);
    }

    public void OnReloadFinish() {
        animator.SetBool("IsReloading", false);
        animator.enabled = false;
        gunData.currentAmmo = gunData.magSize;
        player.UI.AmmoPanel.UpdatePanel(currentWeaponAmmo: gunData.currentAmmo, totalWeaponAmmo: gunData.currentAmmo);
    }
}
