using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private InputActionReference nextActionRef;
    [SerializeField] private SpriteRenderer targetSprite;   // ← 変更したいオブジェクト
    [SerializeField] private Sprite[] tutorialSprites;      // Tutorial1〜Tutorial7 を入れる

    [SerializeField] private TMP_Text targetText;       // ← 追加（変更したいテキスト）
    [SerializeField] private string[] tutorialTexts;    // ← "Tutorial1"〜"Tutorial7" を入れる

    private int value = 1;

    private void Start()
    {
        // Sprite変更
        int index = value - 1;    // 配列は0始まり
        if (index >= 0 && index < tutorialSprites.Length)
        {
            targetSprite.sprite = tutorialSprites[index];
        }

        // TextMeshPro変更
        if (index >= 0 && index < tutorialTexts.Length)
        {
            targetText.text = tutorialTexts[index];
        }
    }
    
    private void OnEnable()
    {
        if (nextActionRef != null)
        {
            nextActionRef.action.Enable();
            nextActionRef.action.performed += OnNext;
        }
    }

    private void OnDisable()
    {
        if (nextActionRef != null)
        {
            nextActionRef.action.performed -= OnNext;
            nextActionRef.action.Disable();
        }
    }

    private void OnNext(InputAction.CallbackContext ctx)
    {
        value = (value % 7) + 1;

        // Sprite変更
        int index = value - 1;    // 配列は0始まり
        if (index >= 0 && index < tutorialSprites.Length)
        {
            targetSprite.sprite = tutorialSprites[index];
        }

        if (index >= 0 && index < tutorialTexts.Length)
        {
            targetText.text = tutorialTexts[index];
        }
        
        Debug.Log("value = " + value);
    }
}
