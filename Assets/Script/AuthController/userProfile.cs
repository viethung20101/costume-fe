using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class userProfile : MonoBehaviour
{
    public TMP_Text TitleText;
    void OnEnable()
    {
        CheckAuthentication();
    }
    void CheckAuthentication()
    {
        string accessToken = PlayerPrefs.GetString("accessToken", "");
        string userProfileJson = PlayerPrefs.GetString("userProfile", "");
        if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(userProfileJson))
        {
            UserProfile userProfile = JsonUtility.FromJson<UserProfile>(userProfileJson);
            TitleText.text = "Welcome, " + userProfile.name + "!";
        }
        else
        {
            TitleText.text = "";
        }
    }
}
