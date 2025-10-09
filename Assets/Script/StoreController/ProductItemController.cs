using UnityEngine;
using System.Globalization;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class ProductItemController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Setup(ProductModel product)
    {
        var info = transform.Find("Info");

        if (info == null)
        {
            Debug.LogError("No 'info' child found in ProductItemPrefab.");
            return;
        }
        else
        {
            var ProductName = info.Find("ProductName").GetComponent<TMPro.TMP_Text>();
            var ProductPrice = info.Find("ProductPrice").GetComponentInChildren<TMPro.TMP_Text>();
            var productImage = transform.Find("ProductImage").GetComponent<Image>();

            ProductName.text = product.name;
            ProductPrice.text = product.price.ToString("C0", CultureInfo.GetCultureInfo("vi-VN"));

            if (product.media != null && product.media.Count > 0)
            {
                var imageMedia = product.media.FirstOrDefault(m =>
                    !string.IsNullOrEmpty(m.url) &&
                    m.type != null &&
                    m.type.Equals("IMAGE", System.StringComparison.OrdinalIgnoreCase)
                );

                if (imageMedia != null)
                {
                    StartCoroutine(ApiConfig.LoadImageFromUrl(imageMedia.url, productImage));
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy media nào có type = IMAGE trong product.media");
                }
            }
            else
            {
                Debug.LogWarning("Product không có media hoặc danh sách media trống.");
            }

        }
    }

}
