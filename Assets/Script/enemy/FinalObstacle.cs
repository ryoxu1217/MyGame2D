using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class FinalObstacle : MonoBehaviour
{
    [SerializeField] private GameObject BreakParticlePrefab;

    [SerializeField] private PlayerStatus ps;

    public void TakeBreak()
    {
        if (ps.CurrentEXP < 100) return;

        Vector3 pos = transform.position;
        pos.z = -1f;
        Instantiate(BreakParticlePrefab, pos, Quaternion.identity);

        Destroy(gameObject);
    }
    
}