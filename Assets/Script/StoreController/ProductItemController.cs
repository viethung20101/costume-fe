using UnityEngine;
using System.Globalization;
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

            ProductName.text = product.name;
            ProductPrice.text =  product.price.ToString("C0", CultureInfo.GetCultureInfo("vi-VN"));
        }
    }
}
