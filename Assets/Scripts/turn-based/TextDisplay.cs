using System.Collections;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class TextDisplay : MonoBehaviour
{
    public TextMeshProUGUI textLabel;
    string destinationText;

    public async Task ShowTextAsync(string text) {
        textLabel.text = "";
        destinationText = text;
        await ShowTextAsyncRoutine();
    }

    public async Task ShowTextAsyncRoutine() {
        int i = 0;
        while(i < destinationText.Length) {
            textLabel.text += destinationText[i++];
            await Task.Delay(30);
        }
    }

    public void ShowFullText(string text = null) {
        textLabel.text = text ?? destinationText;
    }
}
