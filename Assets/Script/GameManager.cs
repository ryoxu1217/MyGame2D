using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void OnPlayerDead()
    {
        Debug.Log("Game Over");
    }
}