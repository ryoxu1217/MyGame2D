using UnityEngine;

public class ExplosionParticleEnd : MonoBehaviour
{
    [SerializeField] private string gameOverScene = "GameOver";
    private FadeController fc;
    
    private void Awake()
    {
        // FadeControllerを探す
        fc = FindAnyObjectByType<FadeController>();
    }

    private void OnParticleSystemStopped()
    {
        if (GameManager.Instance != null)
        {
            fc.StartFadeToClearScene(gameOverScene, Color.black, 0.5f);
        }
        else
        {
            Debug.LogWarning("ExplosionParticleEnd: GameManager.Instance が null のため OnPlayerDead を呼べませんでした");
        }
    }
}