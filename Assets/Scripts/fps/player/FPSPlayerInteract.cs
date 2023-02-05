using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayerInteract : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private FPSPlayer player;
    private FPSInputManager inputManager;

    private float rayLength;

    void Awake() {
        inputManager = FindObjectOfType<FPSInputManager>();
        rayLength = 3.0f;
    }

    void Update() {
        Ray ray = new(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(
            ray: ray,
            hitInfo: out RaycastHit hitInfo,
            maxDistance: rayLength,
            layerMask: LayerMask.GetMask("Interactable")
        ))
        {
            FPSInteractable interactable = hitInfo.collider.GetComponent<FPSInteractable>();
            player.UI.InteractablePromptPanel.UpdateText(interactable.onInteractableSeenMessage);
            if(inputManager.OnFootActions.Interact.triggered) {
                interactable.OnPlayerInteract();
            }
        }
    }
}
