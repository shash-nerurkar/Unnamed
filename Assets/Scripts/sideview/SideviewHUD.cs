using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideviewHUD : MonoBehaviour
{
    public GameObject pauseOverlay;

    public void TooglePause() {
        if(Time.timeScale == 0) {
            Time.timeScale = 1;
            pauseOverlay.SetActive(false);
        }
        else {
            Time.timeScale = 0;
            pauseOverlay.SetActive(true);
        }
    }
}
