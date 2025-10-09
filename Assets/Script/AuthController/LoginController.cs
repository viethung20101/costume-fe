using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
public class LoginController : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_Text errorText;
    public Button loginButton;
    public TMP_Text emailErrorText;
    public TMP_Text passwordErrorText;
    public GameObject loginPanel;
    public GameObject userPanel;
    public bool IsAuthenticated => _isAuthenticated; //Read-only property
    private bool _isAuthenticated = false;

    void Start()
    {
        CheckAuthentication();
        loginButton.onClick.AddListener(OnLoginClicked);
        emailErrorText.text = "";
        passwordErrorText.text = "";
    }

    void CheckAuthentication()
    {
        string accessToken = PlayerPrefs.GetString("accessToken", "");
        string userProfile = PlayerPrefs.GetString("userProfile", "");
        if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(userProfile))
        {
            _isAuthenticated = true;
        }
        else
        {
            _isAuthenticated = false;
        }

        loginPanel.SetActive(!_isAuthenticated);
        userPanel.SetActive(_isAuthenticated);

    }

    public void Logout()
    {
        //Reset all fields and states
        _isAuthenticated = false;
        emailInputField.text = "";
        passwordInputField.text = "";
        errorText.text = "";
        PlayerPrefs.DeleteKey("accessToken");
        PlayerPrefs.DeleteKey("refreshToken");
        PlayerPrefs.DeleteKey("userProfile");
        CheckAuthentication();
    }

    void OnLoginClicked()
    {
        // Clear Error Texts
        emailErrorText.text = "";
        passwordErrorText.text = "";
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            emailErrorText.text = "Email không được để trống!";
            return;
        }
        if (!IsValidEmail(email))
        {
            emailErrorText.text = "Email không hợp lệ!";
            return;
        }
        if (string.IsNullOrEmpty(password))
        {
            passwordErrorText.text = "Mật khẩu không được để trống!";
            return;
        }

        LoadingUI.Instance.Show();
        StartCoroutine(new AuthAPIs().LoginRequest(
            email,
            password,
            (result) =>
            {
                StartCoroutine(new AuthAPIs().GetProfileRequest(
                    (profile) =>
                    {
                        LoadingUI.Instance.Hide();
                        _isAuthenticated = true;
                        loginPanel.SetActive(false);
                        userPanel.SetActive(true);

                    },
                    (error) =>
                    {
                        LoadingUI.Instance.Hide();
                        Debug.LogError("Get profile lỗi: " + error);
                    }
                ));
            },
            (error) =>
            {
                LoadingUI.Instance.Hide();
                Debug.LogError("Login thất bại: " + error);
                passwordErrorText.text = "Sai email hoặc mật khẩu!";
            }
        ));
    }

    bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }
    bool IsValidPassword(string password)
    {
        string pattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,}$"; // Ít nhất 6 ký tự, 1 chữ hoa, 1 số, 1 ký tự đặc biệt
        return Regex.IsMatch(password, pattern);
    }

}
