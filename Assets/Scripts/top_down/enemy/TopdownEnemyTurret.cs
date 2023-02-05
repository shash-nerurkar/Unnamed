using System.Collections;
using UnityEngine;

public class TopdownEnemyTurret : MonoBehaviour
{
    public Transform crosshair;
    
    Vector3 targetPosition;
    Coroutine rotateToTargetCoroutine;

    public void UpdateTarget(Vector3 targetPosition) {
        if(this.targetPosition == targetPosition) return;

        this.targetPosition = targetPosition;

        if(rotateToTargetCoroutine != null) StopCoroutine(rotateToTargetCoroutine);
        rotateToTargetCoroutine = StartCoroutine(RotateToTarget(this.targetPosition));
    }

    IEnumerator RotateToTarget(Vector3 endPosition) {
        Vector3 startPosition = transform.up;
        
        for(float i = 0; i <= 1; i += 0.001f) {
            transform.up = Vector3.Lerp(startPosition, endPosition, i);
            yield return null;
        }
    }
}
