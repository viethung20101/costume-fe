using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Globalization;
using UnityEngine.Networking;
using System.Collections;

public class ProductDetailController : MonoBehaviour
{
    [Header("Product Detail UI Elements")]
    public TMP_Text productNameText;
    public TMP_Text productDescriptionText;
    public TMP_Text productShortDescriptionText;
    public TMP_Text productPriceText;
    public Image productImage;

    void Start()
    {
        CheckAuthentication();

        var selectedProductJson = PlayerPrefs.GetString("selectedProduct", "");
        if (string.IsNullOrEmpty(selectedProductJson))
        {
            Debug.LogWarning("⚠️ No product selected, redirecting to Store scene.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Store");
        }
        else
        {
            try
            {
                SetInfomationProduct(selectedProductJson);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error parsing product data: {ex.Message}");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Store");
            }
        }
    }

    void CheckAuthentication()
    {
        string accessToken = PlayerPrefs.GetString("accessToken", "");
        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogWarning("⚠️ No access token found, redirecting to Login scene.");
            PlayerPrefs.DeleteAll();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Auth");
        }
    }

    void SetInfomationProduct(string productJson)
    {
        var product = JsonConvert.DeserializeObject<ProductModel>(productJson);
        if (product == null)
        {
            Debug.LogError("Product data is null!");
            return;
        }

        productNameText.text = product.name;
        productDescriptionText.text = product.description;
        productShortDescriptionText.text = product.shortDescription;
        productPriceText.text = product.price.ToString("C0", CultureInfo.GetCultureInfo("vi-VN"));


        // Load product image
        string imageUrl = null;
        if (product.media != null && product.media.Count > 0)
        {
            for (int i = 0; i < product.media.Count; i++)
            {
                var mediaItem = product.media[i];
                if (mediaItem != null &&
                    !string.IsNullOrEmpty(mediaItem.url) &&
                    !string.IsNullOrEmpty(mediaItem.type) &&
                    mediaItem.type.Equals("IMAGE", System.StringComparison.OrdinalIgnoreCase))
                {
                    imageUrl = mediaItem.url;
                    break;
                }
            }
        }
        if (!string.IsNullOrEmpty(imageUrl))
        {
            StartCoroutine(ApiConfig.LoadImageFromUrl(imageUrl, productImage));
        }
        else
        {
            Debug.LogWarning("No valid image media found in product data.");
        }
    }
    public void OnBackButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Store");
    }
}
