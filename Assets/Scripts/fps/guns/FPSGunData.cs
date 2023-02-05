using UnityEngine;

public enum GunShootType {
    SemiAutomatic,
    BurstFire,
    FullyAutomatic,
}

[CreateAssetMenu(fileName = "GunData", menuName = "FPS/GunData")]
public class FPSGunData : ScriptableObject
{
    [Header("Info")]
    public new string name;

    [Header("Shooting")]
    public int maxDistance;
    public FPSBulletData bulletData;
    public GunShootType gunShootType;

    [Header("Reloading")]
    public int magSize;
    [HideInInspector] public int currentAmmo;

    [Header("Recoiling")]
    public float recoil;
    public float recoilDuration;
    public float recoverDuration;

    // [Header("Gun Shoot Type: Semi Automatic")]
    [HideInInspector] public bool canFire;

    [Header("Gun Shoot Type: Burst Fire")]
    public int burstFireCount;
    [HideInInspector] public int currentBurstFireCount;


    // [Header("Gun Shoot Type: Fully Automatic")]

    public void OnInit() {
        currentAmmo = magSize;
        canFire = true;
        currentBurstFireCount = burstFireCount;
    }
}
