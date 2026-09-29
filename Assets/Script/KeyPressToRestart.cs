using UnityEngine;
using UnityEngine.InputSystem;

public class KeyPressToRestart : MonoBehaviour
{
    [SerializeField] private string scene;
    [SerializeField] private InputActionReference changeKeyRef;   // ← 安定版
    private FadeController fc;

    private void Awake()
    {
        fc = FindAnyObjectByType<FadeController>();
    }

    private void OnEnable()
    {
        changeKeyRef.action.Enable();
        changeKeyRef.action.performed += OnKeyPressed;
    }

    private void OnDisable()
    {
        changeKeyRef.action.performed -= OnKeyPressed;
        changeKeyRef.action.Disable();
    }

    private void OnKeyPressed(InputAction.CallbackContext ctx)
    {
        if (fc.IsFading) return;

        fc.StartFadeToClearScene(scene, Color.black, 1f);
    }
}
