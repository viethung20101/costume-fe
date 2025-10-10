using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Newtonsoft.Json;


public class ChatbotAPIs
{
    private string baseUrl = ApiConfig.BaseUrl;

    // ===== POST CHAT BOT =====
    public IEnumerator ChatbotRequest(string message, Action<string> onSuccess, Action<string> onError)
    {
        string url = baseUrl + "/chat-bot/chat";

        string jsonBody = JsonUtility.ToJson(new Chatbot(message));
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
            string responseText = json.data.response;
            onSuccess?.Invoke(responseText);
        }
    }

}
public class Chatbot
{
    public string text;

    public Chatbot(string text)
    {
        this.text = text;
    }
}