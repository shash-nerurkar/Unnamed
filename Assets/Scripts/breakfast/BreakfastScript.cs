using UnityEngine;
using UnityEngine.InputSystem;

public class BreakfastScript : MonoBehaviour
{
    [SerializeField] private LayerMask inputLayerMask;
    [SerializeField] private InputAction changeArmAction;

    [SerializeField] private Arm[] arms;

    void OnEnable()
    {
        changeArmAction.Enable();
    }

    void OnDisable()
    {
        changeArmAction.Disable();
    }
 
    private void Awake()
    {
        Camera.main.eventMask = inputLayerMask;

        Cursor.visible = false;

        arms[0].SelectArm();
        
        changeArmAction.started += context => {
            if(arms[0].isSelected) arms[1].SelectArm();
            else arms[0].SelectArm();
        };
    }
}
