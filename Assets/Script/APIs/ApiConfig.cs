using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
public static class ApiConfig
{
    // URL gốc của API
    // public static string BaseUrl = "http://88.222.242.207:9944/api"; //Server
    public static string BaseUrl = "http://10.220.19.71:5566/api"; // Local
    public static string BaseImageUrl = "http://10.220.19.71:5566"; // Local

    //Common Function to load image from URL
    public static IEnumerator LoadImageFromUrl(string imageUrl, Image productImage)
    {
        var url = BaseImageUrl + imageUrl;
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                productImage.sprite = sprite;
            }
            else
            {
                Debug.LogError($"❌ Failed to load image: {request.error}");
            }
        }
    }
}