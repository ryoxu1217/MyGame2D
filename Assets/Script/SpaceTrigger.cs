using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceTrigger : MonoBehaviour
{
    public Animator anim;
    public InputAction spaceAction;

    void OnEnable()
    {
        spaceAction.Enable();
        spaceAction.performed += OnSpace;
    }

    void OnDisable()
    {
        spaceAction.performed -= OnSpace;
        spaceAction.Disable();
    }

    private void OnSpace(InputAction.CallbackContext ctx)
    {
        
    }
}
