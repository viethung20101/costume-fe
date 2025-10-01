using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    public static LoadingUI Instance;
    [SerializeField] private GameObject loadingPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Show()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
    }

    public void Hide()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }
}
