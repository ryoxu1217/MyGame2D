using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusUIController : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private Image kickImage;
    [SerializeField] private Sprite KickReadySprite;
	[SerializeField] private Sprite kickCooldownSprite;
    [SerializeField]private Image rollImage;
    [SerializeField] private Sprite rollReadySprite;
	[SerializeField] private Sprite rollCooldownSprite;

    [Header("TextMeshProUGUI")]
    [SerializeField] private TextMeshProUGUI kick;
    [SerializeField] private TextMeshProUGUI roll;
    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI maxHP;

    [Header("Component")]
    [SerializeField] private PlayerHealth ph; 
    [SerializeField] private PlayerStatus ps;
    [SerializeField] private PlayerRollingAttack pra; 
    [SerializeField] private PlayerKickAttack pka; 

    private void Update()
    {
        UpdateText();
        UpdateImage(); 
    }

    private void UpdateText()
    {
        kick.text = "KickCT: " + Mathf.Floor(pka.cooldownTimer * 10) + " / " + pka.cooldown * 10;
        roll.text = "RollCT: " + Mathf.Floor(pra.cooldownTimer * 10) + " / " + pra.cooldown * 10;
        attack.text = "ATK: " + (ps.AttackDamage);
        maxHP.text = "HP: " + (ph.CurrentHP) + " / " + ph.MaxHP;
    }
    private void UpdateImage()
    {
        if (pka.isCooldown)  kickImage.sprite = kickCooldownSprite; else kickImage.sprite = KickReadySprite;
        if (pra.isCooldown)  rollImage.sprite = rollCooldownSprite; else rollImage.sprite = rollReadySprite;
    }
}
