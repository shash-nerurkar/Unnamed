using UnityEditor;

[CustomEditor(typeof(FPSInteractable), true)]
public class FPSInteractableEditor : Editor
{
    public override void OnInspectorGUI() {
        FPSInteractable interactable = (FPSInteractable)target;
        if(target.GetType() == typeof(FPSInteractableEventOnly)) {
            interactable.onInteractableSeenMessage = EditorGUILayout.TextField("On Interactable Seen Message", interactable.onInteractableSeenMessage);
            EditorGUILayout.HelpBox("FPSInteractableEventOnly can ONLY use UnityEvents.", MessageType.Info);
            if(interactable.GetComponent<FPSInteractionEvent>() == null) {
                interactable.useEvents = true;
                interactable.gameObject.AddComponent<FPSInteractionEvent>();
            }
        }
        else {
            base.OnInspectorGUI();
            if(interactable.useEvents) {
                if(interactable.GetComponent<FPSInteractionEvent>() == null)
                    interactable.gameObject.AddComponent<FPSInteractionEvent>();
            }
            else {
                if(interactable.GetComponent<FPSInteractionEvent>() != null)
                    DestroyImmediate(interactable.GetComponent<FPSInteractionEvent>());
            }
        }
    }
}
