using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class IsometricPlayerWeapon : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public IsometricPlayerAbility1 isometricPlayerAbility1;
    public IsometricPlayerAbility2 isometricPlayerAbility2;
    public IsometricPlayerAbility3 isometricPlayerAbility3;
    public IsometricPlayerAbility4 isometricPlayerAbility4;

    public IsometricPlayer player;

    public List<Transform> transforms;

    public void FaceDirection(Vector3 moveDirection) {
        if(moveDirection.x == 0 && moveDirection.y > 0) {
            spriteRenderer.sortingOrder = 0;
            animator.SetInteger("faceDirection", 0);
            transform.position = transforms[0].position;
        }
        else if(moveDirection.x > 0 && moveDirection.y > 0) {
            spriteRenderer.sortingOrder = 0;
            animator.SetInteger("faceDirection", 1);
            transform.position = transforms[1].position;
        }
        else if(moveDirection.x > 0 && moveDirection.y == 0) {
            spriteRenderer.sortingOrder = 0;
            animator.SetInteger("faceDirection", 2);
            transform.position = transforms[2].position;
        }
        else if(moveDirection.x > 0 && moveDirection.y < 0) {
            spriteRenderer.sortingOrder = 1;
            animator.SetInteger("faceDirection", 3);
            transform.position = transforms[3].position;
        }
        else if(moveDirection.x == 0 && moveDirection.y < 0) {
            spriteRenderer.sortingOrder = 1;
            animator.SetInteger("faceDirection", 4);
            transform.position = transforms[4].position;
        }
        else if(moveDirection.x < 0 && moveDirection.y < 0) {
            spriteRenderer.sortingOrder = 1;
            animator.SetInteger("faceDirection", 3);
            transform.position = transforms[3].position;
        }
        else if(moveDirection.x < 0 && moveDirection.y == 0) {
            spriteRenderer.sortingOrder = 0;
            animator.SetInteger("faceDirection", 2);
            transform.position = transforms[2].position;
        }
        else if(moveDirection.x < 0 && moveDirection.y > 0) {
            spriteRenderer.sortingOrder = 0;
            animator.SetInteger("faceDirection", 1);
            transform.position = transforms[1].position;
        }
    }

    public void Attack(int abilityNum) {
        animator.SetBool("isAbility" + abilityNum, true);
        switch(abilityNum) {
            case 1:
                break;
                
            case 2:
                break;
                
            case 3:
                break;
                
            case 4:
                isometricPlayerAbility4.ShootLaser();
                break;
                
            default:
                break;
        }
    }

    public void OnAbilityFinish(int abilityNum) {
        animator.SetBool("isAbility" + abilityNum, false);
        switch(abilityNum) {
            case 1:
                break;
                
            case 2:
                break;
                
            case 3:
                break;
                
            case 4:
                isometricPlayerAbility4.ClearLaser();
                break;
                
            default:
                break;
        }
    }
}
