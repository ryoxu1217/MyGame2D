using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToCurrentScene : MonoBehaviour
{
    private void Start()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.MoveGameObjectToScene(gameObject, current);

        Debug.Log($"{gameObject.name} を {current.name} に移動しました");
    }
}
