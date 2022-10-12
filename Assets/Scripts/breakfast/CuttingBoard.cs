using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : MonoBehaviour
{
    public Collider2D cd;
    public SpriteRenderer cuffs;
    Vegetable vegetable;

    public Vegetable GetVegetable() {
        return vegetable;
    }

    public void ToggleVegetableStatus(Vegetable vegetable = null) {
        this.vegetable = vegetable;
        if(vegetable != null) {
            cuffs.gameObject.SetActive(true);
            cuffs.transform.position = vegetable.transform.position;
            print(vegetable.spriteRenderer.bounds.size);
            print(cuffs.bounds.size);
            print(vegetable.lateralCollider.bounds.size);
            cuffs.transform.localScale = new Vector2(1, vegetable.spriteRenderer.bounds.size.y / cuffs.bounds.size.y);
        }
        else {
            cuffs.gameObject.SetActive(false);
            cuffs.transform.localScale = Vector3.one;
        }
    }
}
