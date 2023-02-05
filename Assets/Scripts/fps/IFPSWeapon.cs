using UnityEngine;

public interface IFPSWeapon
{
    public Sprite Image { get; set; }
    public Transform WeaponTransform { get; set; }
    public Transform EquippedTransform { get; set; }
    public Transform UnequippedTransform { get; set; }
    public Transform ZoomedInTransform { get; set; }

    public void SwitchIn();
    public void SwitchOut();    
    public void Drop(Transform floorWeaponTransform);
    public void PickUp();    
    public void Attack(bool IsAttacking);
    public void SetDataString();
}
