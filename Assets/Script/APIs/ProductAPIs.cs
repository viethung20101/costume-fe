using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Newtonsoft.Json;


public class ProductAPIs
{
    private string baseUrl = ApiConfig.BaseUrl;

    // ===== GET ALL CATEGORIES =====
    public IEnumerator GetAllProductCategoriesRequest(Action<string> onSuccess, Action<string> onError)
    {
        string url = baseUrl + "/product-categories/get-all";

        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(www.error);
        }
        else
        {
            string result = www.downloadHandler.text;
            var json = JsonConvert.DeserializeObject<ProductCategoryResponse>(result);
            ProductCategoryManager.Instance.productCategories = json.data;
            onSuccess?.Invoke(result);
        }
    }

    // ===== GET PRODUCTS BY CATEGORY ID =====
    public IEnumerator GetProductsByCategoryIdRequest(string categoryId, Action<string> onSuccess, Action<string> onError)
    {
        string url = baseUrl + $"/products/category/{categoryId}";

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

    // ===== GET ALL PRODUCTS =====
    public IEnumerator GetAllProductsRequest(Action<string> onSuccess, Action<string> onError)
    {
        string url = baseUrl + "/products/get-all";

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