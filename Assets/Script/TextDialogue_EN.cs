using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
public class TextDialogue_EN : MonoBehaviour
{
  [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI TextComponent;
    [SerializeField] private float Speed = 0.03f;

    private Coroutine typingCoroutine;

    void Start()
    {
        if (TextComponent != null)
            TextComponent.text = string.Empty;
    }

    // ✅ Hiển thị văn bản động (có thể truyền từ chatbot)
    public void ShowChatbotResponse(string message)
    {
        // Dừng gõ nếu đang gõ dở
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeMessage(message));
    }

    // Gõ từng ký tự một, hiệu ứng chatbot
    private IEnumerator TypeMessage(string message)
    {
        TextComponent.text = string.Empty;

        foreach (char c in message)
        {
            TextComponent.text += c;
            yield return new WaitForSeconds(Speed);
        }

        typingCoroutine = null;
    }

    // ⚙️ Hàm tùy chọn nếu bạn muốn gán tốc độ hoặc reset text
    public void ClearDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        TextComponent.text = string.Empty;
    }

    public void SetSpeed(float newSpeed)
    {
        Speed = Mathf.Max(0.001f, newSpeed);
    }
}
