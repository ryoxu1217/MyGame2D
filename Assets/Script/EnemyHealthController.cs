using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthController : MonoBehaviour
{
    public Image healthGaugeImageMain;
    public Image healthGaugeImageBack;
    public EnemyHealth eh; 

    private void Update()
    {
        UpdateGauge();
    }

    private void UpdateGauge()
    {
        float ratio = (float)eh.CurrentHP / eh.MaxHP;
        healthGaugeImageMain.fillAmount = ratio;

        if (eh.isInvincible)
        {
            healthGaugeImageMain.enabled = true;
            healthGaugeImageBack.enabled = true;
        }
        else
        {
            healthGaugeImageMain.enabled = false;
            healthGaugeImageBack.enabled = false;
        }
    }
}
