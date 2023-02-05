using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSInteractablePromptPanel : MonoBehaviour
{
    [SerializeField] private Animator interactablePromptAnimator;
    [SerializeField] private TextMeshProUGUI interactablePromptMessage;

    private Timer showInteractablePromptTimer;
    private float showInteractablePromptTime;

    void Awake() {
        showInteractablePromptTimer = gameObject.AddComponent<Timer>();
        showInteractablePromptTime = 0.1f;
    }

    public void UpdateText(string text) {
        gameObject.SetActive(true);
        interactablePromptMessage.text = text;
        interactablePromptAnimator.Play("fade_in");
        showInteractablePromptTimer.StartTimer(maxTime: showInteractablePromptTime, onTimerFinish: OnShowInteractablePromptTimerFinished);
    }

    void OnShowInteractablePromptTimerFinished() {
        interactablePromptAnimator.Play("fade_out");
    }

    void OnInteractablePromptPanelFadeOut() {
        gameObject.SetActive(false);
    }
}
