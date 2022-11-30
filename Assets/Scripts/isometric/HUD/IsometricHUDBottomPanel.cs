using UnityEngine;

public class IsometricHUDBottomPanel : MonoBehaviour
{
    public ValueBar healthBar;
    public IsometricHUDBottomPanelAbilityButton[] abilityButtons;
    public ValueBar abilityCastTimeBar;
    Timer[] abilityCooldownTimers;
    float[] abilityCooldownTimes;
    Timer abilityTimer;
    float totalCastTime;

    public void InitBottomPanel(float maxHealth, int[] abilityCooldownTimes, Timer[] abilityCooldownTimers) {
        healthBar.InitBar(maxValue: maxHealth, setValue: true);
        this.abilityCooldownTimers = abilityCooldownTimers;
        this.abilityCooldownTimes = new float[5];
        for(int i = 0; i < 5; i++)
            this.abilityCooldownTimes[i] = abilityCooldownTimes[i]/1000.0f;
    }

    public void OnAbilityStart(float castTime, Timer abilityTimer) {
        abilityCastTimeBar.gameObject.SetActive(true);
        abilityCastTimeBar.RestartBar();
        abilityCastTimeBar.InitBarWithoutStarting(castTime/1000.0f);
        this.abilityTimer = abilityTimer;
        totalCastTime = castTime/1000.0f;
    }

    public void OnAbilityFinish() {
        abilityCastTimeBar.gameObject.SetActive(false);
    }

    public void OnAbilityCooldownStart(int abilityNum) {
        if(abilityNum > 0) {
            abilityButtons[abilityNum-1].abilityIcon.color = new Color(200, 200, 200, 255);
            abilityButtons[abilityNum-1].cooldownTimerImage.fillAmount = 1;
            abilityButtons[abilityNum-1].cooldownTimerImage.gameObject.SetActive(true);
        }
        else {

        }
    }

    public void OnAbilityCooldownFinish(int abilityNum) {
        if(abilityNum > 0) {
            abilityButtons[abilityNum-1].abilityIcon.color = new Color(255, 255, 255, 255);
            abilityButtons[abilityNum-1].cooldownTimerImage.fillAmount = 0;
            abilityButtons[abilityNum-1].cooldownTimerImage.gameObject.SetActive(false);
        }
        else {
            
        }
    }

    public void Update() {
        if(abilityTimer != null && abilityTimer.IsRunning)
            abilityCastTimeBar.SetValue(totalCastTime - abilityTimer.TimeRemaining);

        if(abilityCooldownTimers[1].IsRunning){
            abilityButtons[0].cooldownTimerText.text = Mathf.CeilToInt(abilityCooldownTimers[1].TimeRemaining).ToString();
            abilityButtons[0].cooldownTimerImage.fillAmount = abilityCooldownTimers[1].TimeRemaining/abilityCooldownTimes[1];
        }
        if(abilityCooldownTimers[2].IsRunning){
            abilityButtons[1].cooldownTimerText.text = Mathf.CeilToInt(abilityCooldownTimers[2].TimeRemaining).ToString();
            abilityButtons[1].cooldownTimerImage.fillAmount = abilityCooldownTimers[2].TimeRemaining/abilityCooldownTimes[2];
        }
        if(abilityCooldownTimers[3].IsRunning){
            abilityButtons[2].cooldownTimerText.text = Mathf.CeilToInt(abilityCooldownTimers[3].TimeRemaining).ToString();
            abilityButtons[2].cooldownTimerImage.fillAmount = abilityCooldownTimers[3].TimeRemaining/abilityCooldownTimes[3];
        }
        if(abilityCooldownTimers[4].IsRunning) {
            abilityButtons[3].cooldownTimerText.text = Mathf.CeilToInt(abilityCooldownTimers[4].TimeRemaining).ToString();
            abilityButtons[3].cooldownTimerImage.fillAmount = abilityCooldownTimers[4].TimeRemaining/abilityCooldownTimes[4];
        }
    }
}
