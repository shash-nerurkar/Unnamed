using UnityEngine;
using UnityEngine.InputSystem;

public class TopdownPlayerShield : MonoBehaviour
{
    public InputAction rotateShieldClockwise;
    public InputAction rotateShieldCounterClockwise;

    bool rotateClockwise = false;
    bool rotateCounterClockwise = false;
    Vector3 rotationAngleDegrees = Vector3.zero;
    
    void OnEnable() {
        rotateShieldClockwise.Enable();
        rotateShieldCounterClockwise.Enable();
    }

    void OnDisable() {
        rotateShieldClockwise.Disable();
        rotateShieldCounterClockwise.Disable();
    }

    void Start() {
        rotateShieldClockwise.started += ctx => {
            rotateClockwise = true;
        };
        rotateShieldClockwise.canceled += ctx => {
            rotateClockwise = false;
        };
        rotateShieldCounterClockwise.started += ctx => {
            rotateCounterClockwise = true;
        };
        rotateShieldCounterClockwise.canceled += ctx => {
            rotateCounterClockwise = false;
        };
    }

    void Update() {
        if(rotateClockwise) {
            rotationAngleDegrees.z += 1f;
            transform.eulerAngles = rotationAngleDegrees;
        }
        else if(rotateCounterClockwise) {
            rotationAngleDegrees.z -= 1f;
            transform.eulerAngles = rotationAngleDegrees;
        }
    }
}
