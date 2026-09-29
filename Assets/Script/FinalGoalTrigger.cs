using UnityEngine;

public class FinalGoalTrigger : MonoBehaviour
{
    [SerializeField] private FadeController fc;
    [SerializeField] private string clearSceneName = "GameClear";
    [SerializeField] private LayerMask playerLayer;

    private void Awake()
    {
        // FadeControllerを探す
        fc = FindAnyObjectByType<FadeController>();

        if (fc == null)
        Debug.LogError("FadeController が見つかりません");
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // collision のlayer がplayerLayer に含まれているか判定
        if ((playerLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            fc.StartFadeToClearScene(clearSceneName, Color.white, 1);
        }
    }
}
