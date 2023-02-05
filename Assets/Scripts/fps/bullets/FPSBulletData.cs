using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "FPS/BulletData")]
public class FPSBulletData : ScriptableObject
{
    [Header("Info")]
    public new string name;

    [Header("Shooting")]
    public int damage;
    public int bulletMoveSpeed;
}
