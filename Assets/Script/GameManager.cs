using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string gameoverScene;

    private void Awake()
    {
        // ゲーム中に1つだけ存在する GameManager を登録
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnPlayerDead()
    {
        GoAndReset(gameoverScene);
    }

    public void GoAndReset(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
