using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Globalization;
using UnityEngine.Networking;
using System.Collections;
using QRCodeShareMain;

public class ProductDetailController : MonoBehaviour
{
    [Header("Product Detail UI Elements")]
    public TMP_Text productNameText;
    public TMP_Text productDescriptionText;
    public TMP_Text productShortDescriptionText;
    public TMP_Text productPriceText;
    public Image productImage;

    [Header("QR Code Section")]
    public Image qrCodeImage;

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
                OnGenerateQRCode();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error parsing product data: {ex.Message}");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Store");
            }
        }
    }

    private void OnGenerateQRCode()
    {
        string content = "Ahihi đồ ngu!";
        var basicQRCode = BasicQRCode(content);
        if (basicQRCode != null && qrCodeImage != null)
        {
            ShowImage(qrCodeImage, basicQRCode);
        }
        else
        {
            Debug.LogError("❌ Failed to generate or display QR code.");
        }

    }

    private void ShowImage(Image showImage, Texture2D image)
    {
        showImage.sprite = ImageProcessing.ConvertTexture2DToSprite(image);
        float imageSize = Mathf.Max(showImage.GetComponent<RectTransform>().sizeDelta.x, showImage.GetComponent<RectTransform>().sizeDelta.y);

        showImage.GetComponent<RectTransform>().sizeDelta = image.width <= image.height ?
            new Vector2(imageSize / image.height * image.width, imageSize) :
            new Vector2(imageSize, imageSize * image.height / image.width);
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

    private Texture2D BasicQRCode(string content)
    {
        QRImageProperties properties = new QRImageProperties(500, 500, 50);
        Texture2D QRCodeImage = QRCodeShare.CreateQRCodeImage(content, properties);
        return QRCodeImage;
    }

    public void OnBackButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Store");
    }
}
