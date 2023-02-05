using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSInputManager : MonoBehaviour
{
    private FPSPlayerInputActions fpsPlayerInputActions;
    public FPSPlayerInputActions.OnFootActions OnFootActions { get; private set; }
    private FPSPlayer player;

    private Timer weaponSwitchDebounceTimer;

    void Awake()
    {
        player = FindObjectOfType<FPSPlayer>();
        weaponSwitchDebounceTimer = gameObject.AddComponent<Timer>();
        fpsPlayerInputActions = new FPSPlayerInputActions();
        OnFootActions = fpsPlayerInputActions.OnFoot;

        OnFootActions.Jump.started += ctx => player.Movement.OnJump();

        OnFootActions.Sprint.started += ctx => player.Movement.OnSprint(IsSprinting: true);
        OnFootActions.Sprint.canceled += ctx => player.Movement.OnSprint(IsSprinting: false);

        OnFootActions.Crouch.started += ctx => player.Movement.OnCrouch(IsCrouching: true);
        OnFootActions.Crouch.canceled += ctx => player.Movement.OnCrouch(IsCrouching: false);

        OnFootActions.ZoomIn.started += ctx => player.Look.OnZoomIn(zoomIn: true);
        OnFootActions.ZoomIn.canceled += ctx => player.Look.OnZoomIn(zoomIn: false);

        OnFootActions.WeaponAttack.started += ctx => player.Fight.OnWeaponAttack(IsAttacking: true);
        OnFootActions.WeaponAttack.canceled += ctx => player.Fight.OnWeaponAttack(IsAttacking: false);

        OnFootActions.WeaponReload.started += ctx => player.Fight.OnWeaponReload();
        
        weaponSwitchDebounceTimer.StartTimer(maxTime: 0.7f, onTimerFinish: OnWeaponSwitchingDebounceTimerFinished);
        OnFootActions.WeaponSwitch.started += ctx => {
            player.Fight.OnWeaponSwitch(switchDirection: Mathf.Sign(OnFootActions.WeaponSwitch.ReadValue<float>()));
            OnFootActions.WeaponSwitch.Disable();
            weaponSwitchDebounceTimer.StartTimer(maxTime: 0.7f, onTimerFinish: OnWeaponSwitchingDebounceTimerFinished);
        };
    }

    void FixedUpdate() {
        player.Movement.OnMove(input: OnFootActions.Movement.ReadValue<Vector2>());
    }

    void LateUpdate() {
        player.Look.OnLook(input: OnFootActions.Look.ReadValue<Vector2>());
    }

    public void OnWeaponSwitchingDebounceTimerFinished() {
        OnFootActions.WeaponSwitch.Enable();
    }

    void OnEnable() {
        OnFootActions.Enable();
        OnFootActions.WeaponSwitch.Disable();
    }

    void OnDisable() {
        OnFootActions.Disable();
    }
}
