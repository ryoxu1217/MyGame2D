using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 5;
    public int CurrentHP { get; private set; }

    void Start()
    {
        CurrentHP = maxHP;
    }
    public void TakeDamage(int amount)
    {
        CurrentHP -= amount;
        Debug.Log(CurrentHP);

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            GameManager.Instance.OnPlayerDead();
        }
    }
}
