using System;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public static event Action GameUnpausedEvent;
    public static event Action GamePausedEvent;
    public GameObject pauseOverlay;

    public void TogglePause() {
        if(Time.timeScale == 0) {
            Time.timeScale = 1;
            pauseOverlay.SetActive(false);
            GameUnpausedEvent();
        }
        else {
            Time.timeScale = 0;
            pauseOverlay.SetActive(true);
            GamePausedEvent();
        }
    }
}
