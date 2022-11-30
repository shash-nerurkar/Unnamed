using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricShadow : MonoBehaviour
{
    public List<Transform> transforms;

    public void FaceDirection(Vector3 moveDirection) {
        if(moveDirection.x == 0 && moveDirection.y > 0) {
            transform.position = transforms[0].position;
        }
        else if(moveDirection.x > 0 && moveDirection.y > 0) {
            transform.position = transforms[1].position;
        }
        else if(moveDirection.x > 0 && moveDirection.y == 0) {
            transform.position = transforms[2].position;
        }
        else if(moveDirection.x > 0 && moveDirection.y < 0) {
            transform.position = transforms[3].position;
        }
        else if(moveDirection.x == 0 && moveDirection.y < 0) {
            transform.position = transforms[0].position;
        }
        else if(moveDirection.x < 0 && moveDirection.y < 0) {
            transform.position = transforms[3].position;
        }
        else if(moveDirection.x < 0 && moveDirection.y == 0) {
            transform.position = transforms[2].position;
        }
        else if(moveDirection.x < 0 && moveDirection.y > 0) {
            transform.position = transforms[1].position;
        }
    }

}
