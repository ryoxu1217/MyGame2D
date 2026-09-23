using UnityEngine;

public class ExplosionParticleEnd : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        GameManager.Instance.OnPlayerDead();
    }
}
