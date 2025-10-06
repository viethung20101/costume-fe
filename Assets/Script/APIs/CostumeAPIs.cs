using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class CostumeAPIs
{
    private string baseUrl = ApiConfig.BaseUrl;

    // ===== GET COSTUMES BY ERA ID =====
    public IEnumerator GetCostumesByEraIdRequest(string eraId, Action<string> onSuccess, Action<string> onError)
    {
        string url = baseUrl + "/costumes/era/" + eraId;
      
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(www.error);
        }
        else
        {
            string result = www.downloadHandler.text;

            onSuccess?.Invoke(result);
        }
    }

}