using UnityEngine;

public class FPSWeaponPickable : FPSInteractable
{
    private IFPSWeapon weapon;
    private FPSPlayer player;

    void Awake() {
        player = FindObjectOfType<FPSPlayer>();
        weapon = transform.GetComponentInParent<IFPSWeapon>();
    }

    public void SetWeaponPickupMessage(string dataString) {
        onInteractableSeenMessage = dataString;
    }

    protected override void Interact()
    {
        player.Fight.OnWeaponPickup(floorWeapon: weapon);
    }
}
