using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public Collider2D cd;
    public CuttingBoard cuttingBoard;
    readonly List<Vegetable> hoveringVegetables = new();
    Vegetable grabbedVegetable;

    public void Disable() {
        cd.enabled = false;
    }

    public void Enable() {
        cd.enabled = true;
    }
    
    public void InteractWithVegetable() {
        if(grabbedVegetable == null) {
            float closestVegetableDistance = 999;
            foreach(Vegetable vegetable in hoveringVegetables) {
                float vegetableDistance = Vector3.Distance(transform.position, vegetable.transform.position);
                if(vegetableDistance <= closestVegetableDistance) {
                    closestVegetableDistance = vegetableDistance;
                    grabbedVegetable = vegetable;
                }
            }
            if(grabbedVegetable != null) grabbedVegetable.ChangeState(Vegetable.States.Caught);
        }
        else {
            grabbedVegetable.ChangeState(Vegetable.States.Free);
            grabbedVegetable = null;
        }

    }
    
    void OnCollisionEnter2D(Collision2D vegetableCollision) {
        Vegetable vegetable = vegetableCollision.collider.GetComponent<Vegetable>();

        if(!hoveringVegetables.Contains(vegetable) && 
            !vegetable.isCuffed &&
            (cuttingBoard.GetVegetable() == null || !vegetable.isInBowl)) {
            hoveringVegetables.Add(vegetable);
        }
    }

    void OnCollisionExit2D(Collision2D vegetableCollision) {
        Vegetable vegetable = vegetableCollision.collider.GetComponent<Vegetable>();
        hoveringVegetables.Remove(vegetable);
    }
}
