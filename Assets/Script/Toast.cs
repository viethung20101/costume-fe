using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToastManager : MonoBehaviour
{
    public GameObject toastPrefab;
    public Transform toastParent;

    public void ShowToast(string message, Color bgColor)
    {
        var toast = Instantiate(toastPrefab, toastParent);
        var text = toast.GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;
        toast.GetComponent<Image>().color = bgColor;

        // Animate (LeanTween or DOTween)
        LeanTween.alphaCanvas(toast.GetComponent<CanvasGroup>(), 1, 0.3f)
            .setOnComplete(() =>
            {
                LeanTween.delayedCall(2f, () =>
                {
                    LeanTween.alphaCanvas(toast.GetComponent<CanvasGroup>(), 0, 0.3f)
                        .setOnComplete(() => Destroy(toast));
                });
            });
    }
}
