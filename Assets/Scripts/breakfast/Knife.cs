using UnityEngine;

public class Knife : MonoBehaviour
{
    public Collider2D cd;
    Vegetable vegetableToCut;

    public void Disable() {
        cd.enabled = false;
    }

    public void Enable() {
        cd.enabled = true;
    }

    public void RemoveVegetable() {
        vegetableToCut = null;
    }
    
    public void CutVegetable() {
        if(vegetableToCut != null) {
            vegetableToCut.Cut();
        }
    }

    void OnCollisionEnter2D(Collision2D vegetableCollision) {
        Vegetable vegetable = vegetableCollision.collider.GetComponent<Vegetable>();

        if(vegetable.isCuffed) {
            vegetableToCut = vegetable;
        }
    }

    void OnCollisionExit2D(Collision2D vegetableCollision) {
        Vegetable vegetable = vegetableCollision.collider.GetComponent<Vegetable>();

        if(vegetableToCut == vegetable) {
            vegetableToCut = null;
        }
    }

}
