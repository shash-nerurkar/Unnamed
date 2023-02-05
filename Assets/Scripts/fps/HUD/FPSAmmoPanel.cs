using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSAmmoPanel : MonoBehaviour
{
    [SerializeField] private GameObject ammoContainer;
    [SerializeField] private GameObject ammoIndicatorElement;
    [SerializeField] private TextMeshProUGUI ammoCountLabel;

    public Vector2 ammoContainerPadding;
    public Vector2 ammoContainerSpaceBetween;

    public void UpdatePanel(int currentWeaponAmmo, int totalWeaponAmmo) {
        if(currentWeaponAmmo < ammoContainer.transform.childCount) {
            for(int i = currentWeaponAmmo; i < ammoContainer.transform.childCount; i++) Destroy(ammoContainer.transform.GetChild(i).gameObject);
        }
        else {
            for(int i = ammoContainer.transform.childCount; i < currentWeaponAmmo; i++) {
                Vector2 pos = ammoContainerPadding;
                if(i >= 60) pos.y += ammoIndicatorElement.GetComponent<RectTransform>().rect.size.y + ammoContainerSpaceBetween.y;
                pos.x += (ammoIndicatorElement.GetComponent<RectTransform>().rect.size.x + ammoContainerSpaceBetween.x) * (i - 60*(i/60));
                GameObject element =  Instantiate(original: ammoIndicatorElement, position: Vector3.one, rotation: Quaternion.identity);
                element.transform.SetParent(ammoContainer.transform);
                element.transform.localScale = Vector3.one;
                element.GetComponent<RectTransform>().localPosition = pos;
            }
        }

        ammoCountLabel.text = currentWeaponAmmo.ToString() + " / " + totalWeaponAmmo.ToString();
    }
}
