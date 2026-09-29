using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthController : MonoBehaviour
{
    [SerializeField] private Image healthGaugeImageMain;
    [SerializeField] private Image healthGaugeImageBack;
    [SerializeField] private bool isAlwaysShown = false;
    [SerializeField] private EnemyHealth eh; 

    private void Update()
    {
        UpdateGauge();
    }

    private void UpdateGauge()
    {
        float ratio = (float)eh.CurrentHP / eh.MaxHP;
        healthGaugeImageMain.fillAmount = ratio;

        if (isAlwaysShown)
        {
            healthGaugeImageMain.enabled = true;
            healthGaugeImageBack.enabled = true;
        } else {
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
}
