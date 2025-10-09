using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Newtonsoft.Json;


public class CategoryAPIs
{
    private string baseUrl = ApiConfig.BaseUrl;

    // ===== GET ALL CATEGORIES =====
    public IEnumerator GetAllCategoriesRequest(Action<string> onSuccess, Action<string> onError)
    {
        string url = baseUrl + "/categories/get-all";

        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(www.error);
        }
        else
        {
            string result = www.downloadHandler.text;
            var json = JsonConvert.DeserializeObject<CategoryResponse>(result);
            onSuccess?.Invoke(result);
        }
    }

}