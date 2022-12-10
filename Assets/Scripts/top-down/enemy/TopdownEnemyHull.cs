using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdownEnemyHull : MonoBehaviour
{
    public TopdownEnemy enemy;

    Vector3 currentPosition;

    void Awake() {
        currentPosition = transform.position;
    }

    public void MoveToDestination(Vector3 destination) {
        StartCoroutine(RotateToDestination(destination));
    }

    IEnumerator RotateToDestination(Vector3 destination) {
        Vector3 startPosition = transform.up;
        Vector3 endPosition = destination - transform.position;
        
        for(float i = 0; i <= 1; i += 0.001f) {
            transform.up = Vector3.Lerp(startPosition, endPosition, i);
            yield return null;
        }

        currentPosition = destination;
    }

    void Update() {
        enemy.transform.position = Vector3.MoveTowards(transform.position, currentPosition, Mathf.Clamp(3 * Time.deltaTime, 0, 1));
    }
}
