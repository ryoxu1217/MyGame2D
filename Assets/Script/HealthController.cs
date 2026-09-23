using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public Image healthGaugeImage;
   public PlayerHealth playerHealth; 

    private void Update()
    {
        UpdateGauge();
    }

    private void UpdateGauge()
    {
        float ratio = (float)playerHealth.CurrentHP / playerHealth.MaxHP;
        healthGaugeImage.fillAmount = ratio;
    }
}
