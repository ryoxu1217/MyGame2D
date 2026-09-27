using UnityEngine;

public class ExplosionParticleEnd : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDead();
        }
        else
        {
            Debug.LogWarning("ExplosionParticleEnd: GameManager.Instance が null のため OnPlayerDead を呼べませんでした");
        }
    }
}
