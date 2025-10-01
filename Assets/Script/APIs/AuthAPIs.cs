using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Newtonsoft.Json;

public class AuthAPIs
{
    private string baseUrl = ApiConfig.BaseUrl;

    // ===== LOGIN =====
    public IEnumerator LoginRequest(string email, string password, Action<string> onSuccess, Action<string> onError)
    {
        string url = baseUrl + "/auth/login";

        string jsonBody = JsonUtility.ToJson(new LoginData(email, password));
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(www.error);
        }
        else
        {
            string result = www.downloadHandler.text;
            var json = JsonConvert.DeserializeObject<dynamic>(result);

            string accessToken = json["data"]["accessToken"];
            string refreshToken = json["data"]["refreshToken"];

            Debug.Log("Access Token: " + accessToken);
            Debug.Log("Refresh Token: " + refreshToken);
            // Lưu Token và Refresh Token vào PlayerPrefs
            PlayerPrefs.SetString("accessToken", accessToken);
            PlayerPrefs.SetString("refreshToken", refreshToken);
            PlayerPrefs.Save();

            onSuccess?.Invoke(result);
        }
    }

    // ===== GET PROFILE =====
    public IEnumerator GetProfileRequest(Action<string> onSuccess, Action<string> onError)
    {
        string url = baseUrl + "/profiles/me";
        string accessToken = PlayerPrefs.GetString("accessToken", "");

        if (string.IsNullOrEmpty(accessToken))
        {
            onError?.Invoke("Chưa có token, hãy login trước.");
            yield break;
        }

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(www.error);
        }
        else
        {
            string result = www.downloadHandler.text;
            var json = JsonConvert.DeserializeObject<dynamic>(result);
            PlayerPrefs.SetString("userProfile", json.data.ToString());
            PlayerPrefs.Save();

            onSuccess?.Invoke(result);
        }
    }

}


[System.Serializable]
public class LoginData
{
    public string email;
    public string password;

    public LoginData(string email, string password)
    {
        this.email = email;
        this.password = password;
    }
}

[System.Serializable]
public class LoginResponse
{
    public LoginDataResponse data;
    public int statusCode;
}

[System.Serializable]
public class LoginDataResponse
{
    public string role;
    public string expiresIn;
    public string accessToken;
    public string refreshToken;
}

[System.Serializable]
public class ProfileResponse
{
    public ProfileData data;
    public int statusCode;
}

[System.Serializable]
public class ProfileData
{
    public string id;
    public string email;
    public string name;
    public string role;
}