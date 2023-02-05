using UnityEngine;

public abstract class FPSInteractable : MonoBehaviour
{
    public bool useEvents;
    public string onInteractableSeenMessage;
    
    public void OnPlayerInteract() {
        if(useEvents)
            GetComponent<FPSInteractionEvent>().OnInteract.Invoke();
        Interact();
    }

    // Template to be overridden
    protected virtual void Interact() {}
}
