using UnityEngine;

public class FPSDoorKeypad : FPSInteractable
{
    [SerializeField] private Animator doorAnimator;

    public bool IsOpen { get; private set; }

    void Awake() {
        IsOpen = false;
    }

    protected override void Interact()
    {
        IsOpen = !IsOpen;
        if(IsOpen) {
            doorAnimator.SetBool("IsOpen", true);
        }
        else {
            doorAnimator.SetBool("IsOpen", false);
        }
    }
}
