using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int CurrentEXP{ get; private set; }
    public int AttackDamage = 1;
    public float speed = 5f;
    public float jumpPower = 6f;

    private void Start()
    {
        CurrentEXP = 0;
    }


    public void TakeEXP(int EXP)
    {
        CurrentEXP += EXP;

        Debug.Log("現在のEXP: " + CurrentEXP);
        CurrentEXP = Mathf.Clamp(CurrentEXP, 0, 100);
    }
}
