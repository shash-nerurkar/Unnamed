using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class ValueBarAsync : MonoBehaviour
{
    public Slider bar;
    public TextMeshProUGUI textDisplay;

    public async Task InitBar(float maxValue) {
        bar.maxValue = maxValue;
        textDisplay.text = "0 / " + maxValue;
        await Changevalue(maxValue);
    }

    public async Task Changevalue(float value) {
        float valuePerFrame;
        if(bar.value < value) {
            valuePerFrame = 11f - Mathf.Clamp((value - bar.value)/10, 0.5f, 10);
            while(bar.value < value) {
                bar.value += 0.1f;
                textDisplay.text = Mathf.Round(bar.value*10)/10 + " / " + bar.maxValue;
                await Task.Delay(Mathf.RoundToInt(valuePerFrame));
            }
        }
        else {
            valuePerFrame = 10.5f - Mathf.Clamp((bar.value - value)/10, 0.5f, 10);
            while(bar.value > value) {
                bar.value -= 0.1f;
                textDisplay.text = Mathf.Round(bar.value*10)/10 + " / " + bar.maxValue;
                await Task.Delay(Mathf.RoundToInt(valuePerFrame));
            }
        }
    }
}
