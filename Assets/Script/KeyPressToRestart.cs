using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class KeyPressToRestart : MonoBehaviour
{
    [SerializeField] private InputAction changeKey;
    [SerializeField] private string scene;


    private void OnEnable()
    {
        changeKey.Enable();
        changeKey.performed += OnKeyPressed;
    }

    private void OnDisable()
    {
        changeKey.performed -= OnKeyPressed;
        changeKey.Disable();
    }

    private void OnKeyPressed(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.GoAndReset(scene);
    }

}
