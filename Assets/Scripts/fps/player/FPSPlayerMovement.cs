using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;

    // MOVEMENT
    private Vector3 velocity;
    public float Speed { get; private set; }
    public float Gravity { get; private set; }
    // JUMP
    public float JumpHeight { get; private set; }
    // SPRINT
    public bool IsSprinting { get; private set; }
    // CROUCH
    public bool IsCrouching { get; private set; }
    private float crouchTimer = 1;
    private bool lerpCrouch = false;

    void Awake() {
        Speed = 3.0f;
        Gravity = -9.8f;
        
        JumpHeight = 4f;

        IsSprinting = false;

        IsCrouching = false;
    }

    // FROM FPSInputManager.cs
    public void OnMove(Vector2 input) {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        characterController.Move(transform.TransformDirection(moveDirection) * Speed * Time.deltaTime);
    }

    // FROM FPSInputManager.cs
    public void OnJump() {
        if(!characterController.isGrounded) return;

        velocity.y = Mathf.Sqrt(JumpHeight * -1.0f * Gravity);
    }

    // FROM FPSInputManager.cs
    public void OnSprint(bool IsSprinting) {
        this.IsSprinting = IsSprinting;
        Speed = IsSprinting ? 7 : 3;
    }

    // FROM FPSInputManager.cs
    public void OnCrouch(bool IsCrouching) {
        OnSprint(IsSprinting: false);
        this.IsCrouching = IsCrouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }

    void Update() {
        // Debug.Log("Player velocity: " + velocity);

        if(characterController.isGrounded && velocity.y < 0) velocity.y = -2f;
        else velocity.y += Gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if(lerpCrouch) {
            crouchTimer += Time.deltaTime;
            characterController.height = Mathf.Lerp(characterController.height, IsCrouching ? 1 : 2, crouchTimer);
            if(crouchTimer > 1) {
                lerpCrouch = false;
                crouchTimer = 0;
            } 
        }
    }
}

