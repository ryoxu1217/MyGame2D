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
    }

    public void OnPlayerDead()
    {
        // GAMEOVER シーンへ移動
        SceneManager.LoadScene(gameoverScene);
    }
}
