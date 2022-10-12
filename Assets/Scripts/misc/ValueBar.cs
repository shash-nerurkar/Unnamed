using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ValueBar : MonoBehaviour
{
    public Slider bar;
    public TextMeshProUGUI textDisplay;
    Coroutine animatedValueChange;

    public void InitBar(float maxValue) {
        bar.maxValue = maxValue;
        if(textDisplay != null) textDisplay.text = maxValue + " / " + maxValue;
        Changevalue(maxValue);
    }

    public void Changevalue(float value) {
        if(animatedValueChange != null) StopCoroutine(animatedValueChange);
        animatedValueChange = StartCoroutine(AnimatedValueChange(value));
    }
    
    IEnumerator AnimatedValueChange(float destinationValue)
    {
        if(bar.value < destinationValue) {
            while(bar.value < destinationValue) {
                bar.value += 0.1f;
                if(textDisplay != null) textDisplay.text = Mathf.Round(bar.value*10)/10 + " / " + bar.maxValue;
                yield return null;
            }
        }
        else {
            while(bar.value > destinationValue) {
                bar.value -= 0.1f;
                if(textDisplay != null) textDisplay.text = Mathf.Round(bar.value*10)/10 + " / " + bar.maxValue;
                yield return null;
            }
        }
    }
}
