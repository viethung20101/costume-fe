using UnityEngine;
using UnityEngine.UI;

public class RedirectButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string sceneName;
    public bool loginRequired = false;
    public LoginController loginController;
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
                    Debug.Log("User not authenticated. Redirecting to Login scene.");
                    return;
                }
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        });
    }

  
}
