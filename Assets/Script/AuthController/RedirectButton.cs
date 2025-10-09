using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RedirectButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string sceneName;
    public bool loginRequired = false;
    public LoginController loginController;
    public TMP_Text ErrorText;
    void Start()
    {

        var button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            if (loginRequired)
            {
                var isAuthenticated = loginController.IsAuthenticated;
                if (!isAuthenticated)
                {
                    ErrorText.text = "Vui lòng đăng nhập để tiếp tục.";
                    Debug.Log("User not authenticated. Redirecting to Login scene.");
                    return;
                }
            }
            ErrorText.text = "";
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        });
    }

  
}
