using System.Collections;
using UnityEngine;

public class FPSPlayerLook : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private FPSPlayer player;
    [SerializeField] private float zoomedOutCameraValue;
    [SerializeField] private float zoomedInCameraValue;

    public Vector2 Sensitivity { get; private set; }
    private float xRotation = 0f;
    public bool IsZoomedIn { get; private set; }
    private Coroutine zoomCoroutine;
    private Coroutine positionWeaponCoroutine;
    private Coroutine rotateWeaponCoroutine;

    void Awake() {
        Sensitivity = new Vector2(60f, 30f);
        IsZoomedIn = false;
    }

    public void OnLook(Vector2 input) {
        xRotation = Mathf.Clamp(xRotation - input.y * Time.deltaTime * Sensitivity.y, -55f, 55f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.Rotate(Vector3.up * input.x * Time.deltaTime * Sensitivity.x);
    }

    public void OnZoomIn(bool zoomIn) {
        IsZoomedIn = zoomIn;
        
        if(zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(LerpZoomIn(IsZoomedIn ? zoomedInCameraValue : zoomedOutCameraValue));
        
        if(positionWeaponCoroutine != null) StopCoroutine(positionWeaponCoroutine);
        positionWeaponCoroutine = StartCoroutine(LerpPositionWeapon(
            IsZoomedIn 
                ? player.Fight.playerWeapons[player.Fight.currentWeaponIndex].ZoomedInTransform.localPosition
                : player.Fight.playerWeapons[player.Fight.currentWeaponIndex].EquippedTransform.localPosition
        ));
        
        if(rotateWeaponCoroutine != null) StopCoroutine(rotateWeaponCoroutine);
        rotateWeaponCoroutine = StartCoroutine(LerpRotateWeapon(
            IsZoomedIn 
                ? player.Fight.playerWeapons[player.Fight.currentWeaponIndex].ZoomedInTransform.localRotation
                : player.Fight.playerWeapons[player.Fight.currentWeaponIndex].EquippedTransform.localRotation
        ));
    } 

    IEnumerator LerpZoomIn(float targetCameraValue) {
        float time = 0, duration = 0.25f;
        float startCameraValue = cam.fieldOfView;
        while (time < duration)
        {
            cam.fieldOfView = Mathf.Lerp(startCameraValue, targetCameraValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = targetCameraValue;
    }

    IEnumerator LerpPositionWeapon(Vector3 targetPosition) {
        float time = 0, duration = 0.25f;
        Vector3 startPosition = (player.Fight.playerWeapons[player.Fight.currentWeaponIndex] as FPSGun).gameObject.transform.localPosition;
        while (time < duration)
        {
            (player.Fight.playerWeapons[player.Fight.currentWeaponIndex] as FPSGun).gameObject.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        (player.Fight.playerWeapons[player.Fight.currentWeaponIndex] as FPSGun).gameObject.transform.localPosition = targetPosition;
    }

    IEnumerator LerpRotateWeapon(Quaternion targetRotation) {
        float time = 0, duration = 0.25f;
        Quaternion startRotation = (player.Fight.playerWeapons[player.Fight.currentWeaponIndex] as FPSGun).gameObject.transform.localRotation;
        while (time < duration)
        {
            (player.Fight.playerWeapons[player.Fight.currentWeaponIndex] as FPSGun).gameObject.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        (player.Fight.playerWeapons[player.Fight.currentWeaponIndex] as FPSGun).gameObject.transform.localRotation = targetRotation;
    }
}
