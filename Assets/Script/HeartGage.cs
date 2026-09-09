using UnityEngine;
using UnityEngine.UI;

public class HeartHPUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public GameObject heartPrefab; // Image を持つプレハブ
    public Transform heartContainer; // ハートを並べる親オブジェクト

    private Image[] hearts;

    void Start()
    {
        GenerateHearts();
    }

    void Update()
    {
        UpdateHearts();
    }

    void GenerateHearts()
    {
        // 既存のハートを削除
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }

        // 新しいハートを maxHP の数だけ生成
        hearts = new Image[playerHealth.maxHP];

        for (int i = 0; i < playerHealth.maxHP; i++)
        {
            GameObject h = Instantiate(heartPrefab, heartContainer);
            hearts[i] = h.GetComponent<Image>();
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < playerHealth.CurrentHP)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}
