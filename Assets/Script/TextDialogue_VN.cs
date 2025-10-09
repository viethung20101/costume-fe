using UnityEngine;
using TMPro;
using System.Collections;

public class TextDialogue_VN : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TextComponent;
    [SerializeField] private float Speed = 0.03f;
    [TextArea(3, 100)]
    [SerializeField] private string[] Lines;
    private int currentIndex = -1;
    private Coroutine typingCoroutine;

    void Start()
    {
        TextComponent.text = string.Empty;
    }

    /// <summary>
    /// Cập nhật text theo chỉ số, hiển thị đúng dòng tương ứng.
    /// </summary>
    public void Update_Text(int newIndex)
    {
        // Nếu index vượt phạm vi thì bỏ qua
        if (newIndex < 0 || newIndex >= Lines.Length)
        {
            Debug.LogWarning("⚠️ Index ngoài phạm vi Lines: " + newIndex);
            return;
        }

        // Nếu đang chạy coroutine trước đó thì dừng
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        currentIndex = newIndex;

        // Xóa text cũ và bắt đầu đánh máy lại dòng mới
        TextComponent.text = string.Empty;
        typingCoroutine = StartCoroutine(TypeLines(Lines[currentIndex]));
    }

    private IEnumerator TypeLines(string line)
    {
        foreach (char c in line.ToCharArray())
        {
            TextComponent.text += c;
            yield return new WaitForSeconds(Speed);
        }
    }
}
