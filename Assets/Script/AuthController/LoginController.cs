using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
public class LoginController : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public Button loginButton;
    public TMP_Text emailErrorText;
    public TMP_Text passwordErrorText;



    void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        emailErrorText.text = "";
        passwordErrorText.text = "";
    }

    void OnLoginClicked()
    {
        // Clear Error Texts
        emailErrorText.text = "";
        passwordErrorText.text = "";
        
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        //Email validation
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
        //Password validation
        if (string.IsNullOrEmpty(password))
        {
            passwordErrorText.text = "Mật khẩu không được để trống!";
            return;
        }

        StartCoroutine(new AuthAPIs().LoginRequest(
            email,
            password,
            (result) =>
            {
                StartCoroutine(new AuthAPIs().GetProfileRequest(
                    (profile) =>
                    {
                        Debug.Log("Profile " + profile.ToString());
                        // Chuyển scene hoặc thực hiện hành động khác sau khi lấy profile thành công
                    },
                    (error) =>
                    {
                        Debug.LogError("Get profile lỗi: " + error);
                    }
                ));
            },
            (error) =>
            {
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
        string pattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,}$";
        return Regex.IsMatch(password, pattern);
    }

}
