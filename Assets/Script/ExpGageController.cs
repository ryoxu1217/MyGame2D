using UnityEngine;
using UnityEngine.UI;

public class ExpGageController : MonoBehaviour
{
    public Image ExpGageImage;
    public PlayerStatus ps; 

    private void Update()
    {
        UpdateGauge();
    }

    private void UpdateGauge()
    {
        float ratio = (float)ps.CurrentEXP / 100;
        ExpGageImage.fillAmount = ratio;
    }
}
