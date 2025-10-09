using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Newtonsoft.Json;


public class EraAPIs
{
    private string baseUrl = ApiConfig.BaseUrl;

    // ===== GET ALL ERAS =====
    public IEnumerator GetAllErasRequest(Action<string> onSuccess, Action<string> onError)
    {
        string url = baseUrl + "/eras/active";

        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(www.error);
        }
        else
        {
            string result = www.downloadHandler.text;
            var json = JsonConvert.DeserializeObject<EraResponse>(result);
            EraManager.Instance.eras = json.data;
            onSuccess?.Invoke(result);
        }
    }

}