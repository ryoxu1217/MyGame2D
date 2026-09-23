using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SpaceChangeScene : MonoBehaviour
{
    [SerializeField] private InputAction sceneChangeAction;
    [SerializeField] private string nextSceneName = "GameOver";

    private void OnEnable()
    {
        sceneChangeAction.Enable();
        sceneChangeAction.performed += OnSceneChange;
    }

    private void OnDisable()
    {
        sceneChangeAction.performed -= OnSceneChange;
        sceneChangeAction.Disable();
    }

    private void OnSceneChange(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
