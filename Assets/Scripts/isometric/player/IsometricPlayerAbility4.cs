using UnityEngine;

public class IsometricPlayerAbility4 : MonoBehaviour
{
    readonly float rayDefaultDistance = 100;
    public Transform firePoint;
    public LineRenderer lineRenderer;

    public void ShootLaser() {
        RaycastHit2D hit = Physics2D.Raycast(
            origin: firePoint.position, 
            direction: transform.up,
            distance: Mathf.Infinity,
            layerMask: 9
        );
        print(hit.collider.gameObject.name);
        if(hit) {
            Draw2DRay(firePoint.position, hit.point);
        }
        else {
            Draw2DRay(firePoint.position, firePoint.transform.right * rayDefaultDistance);
        }
    }

    public void ClearLaser() {
        Draw2DRay(Vector2.zero, Vector2.zero);
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos) {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }
}
