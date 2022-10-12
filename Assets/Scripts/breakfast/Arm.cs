using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Arm : MonoBehaviour
{
    public Arm otherArm;
    public Hand hand;
    public Knife knife;

    Vector3 basePosition;
    [SerializeField] Transform topLeftTransform;
    [SerializeField] Transform bottomRightTransform;

    [HideInInspector] public bool isSelected;

    Vector3 mousePositionOffset;

    Coroutine moveMouseCoroutine;

    Vector3 GetMouseWorldPosition() => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    void Awake() {
        basePosition = transform.position;
    }

    void ResetArm() {
        isSelected = false;
        if(moveMouseCoroutine != null) StopCoroutine(moveMouseCoroutine);
        moveMouseCoroutine = StartCoroutine(MoveMouse(basePosition));
        if(gameObject.name == "Free arm") {
            hand.InteractWithVegetable();
        }
    }

    IEnumerator MoveMouse(Vector3 destinationPosition) {
        for(float i = 0; i <= 0.95; i+=0.001f) {
            transform.position = Vector3.Lerp(transform.position, destinationPosition, i);
            yield return null;
        }
    }

    public async void SelectArm() {
        otherArm.ResetArm();
        if(gameObject.name == "Free arm") {
            knife.Disable();
            hand.Enable();
        }
        else {
            hand.Disable();
            knife.Enable();
        }
        Mouse.current.WarpCursorPosition(Camera.main.WorldToScreenPoint(gameObject.name == "Free arm" ? hand.transform.position : knife.transform.position));
        await Task.Delay(100);
        if(moveMouseCoroutine != null) StopCoroutine(moveMouseCoroutine);
        isSelected = true;
        mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
    }

    void OnMouseDown() {
        if(isSelected) {
            if(gameObject.name == "Free arm") {
                hand.InteractWithVegetable();
            }
            else {
                knife.CutVegetable();
            }
        }
    }

    void Update() {
        if(isSelected) {
            Vector2 desiredPosition = GetMouseWorldPosition() + mousePositionOffset;
            transform.position = new(
                Mathf.Clamp(desiredPosition.x, topLeftTransform.position.x - (name == "Free arm" ? hand.transform.localPosition.x : knife.transform.localPosition.x), bottomRightTransform.position.x - (name == "Free arm" ? hand.transform.localPosition.x : knife.transform.localPosition.x)), 
                Mathf.Clamp(desiredPosition.y, bottomRightTransform.position.y - (name == "Free arm" ? hand.transform.localPosition.y : knife.transform.localPosition.y), topLeftTransform.position.y - (name == "Free arm" ? hand.transform.localPosition.y : knife.transform.localPosition.y))
            );
        }
    }
}
