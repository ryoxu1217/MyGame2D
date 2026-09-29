using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float gameTimer {get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);   // 既にあるなら自分を消す
            return;
        };

        Instance = this;
        DontDestroyOnLoad(gameObject);

        gameTimer = 0.0f;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Level"))
        {
            gameTimer += Time.deltaTime;
        }
    }

    public void GoToScene(string sceneName)
    {
        if (sceneName.StartsWith("Level")) gameTimer = 0.0f;
        SceneManager.LoadScene(sceneName);
    }
}
