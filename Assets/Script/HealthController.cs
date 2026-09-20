using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HealthController : MonoBehaviour
{
    public Image healthGaugeImage;
   public PlayerHealth playerHealth; 

    public InputAction damageAction; // InputSystemの入力

    private void Update()
    {
        UpdateGauge();
    }

    private void OnEnable()
    {
        damageAction.Enable();
        damageAction.performed += OnDamageInput;
    }

    private void OnDisable()
    {
        damageAction.performed -= OnDamageInput;
        damageAction.Disable();
    }

    private void OnDamageInput(InputAction.CallbackContext ctx)
    {
        playerHealth.TakeDamage(1);
    }

    private void UpdateGauge()
    {
        float ratio = (float)playerHealth.CurrentHP / playerHealth.MaxHP;
        healthGaugeImage.fillAmount = ratio;
    }

}
