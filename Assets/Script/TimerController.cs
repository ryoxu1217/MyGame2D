using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timer;

    private void Update()
    {
        timer.text = "Time: " + GameManager.Instance.gameTimer;
    }
}
