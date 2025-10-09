using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class StoreController : MonoBehaviour
{
    [Header("Product Category")]
    public Sprite ProductCategoryBg;
    public Sprite ProductCategoryBgActive;
    public Transform ProductCategoryContent;
    public GameObject ProductCategoryItemPrefab;

    private Coroutine currentProductRequest;
    [Header("Product List")]
    public Transform productListContent;
    public GameObject ProductItemPrefab;



    void Start()
    {
        CheckAuthentication();
        if (ProductCategoryManager.Instance.productCategories == null || ProductCategoryManager.Instance.productCategories.Count == 0)
        {
            GetAllProductCategories();
        }
        else
        {
            GenerateProductCategoryMenu(ProductCategoryManager.Instance.productCategories);
        }
    }
    void CheckAuthentication()
    {
        string accessToken = PlayerPrefs.GetString("accessToken", "");
        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogWarning("No access token found, redirecting to Login scene.");
            PlayerPrefs.DeleteAll();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Auth");
        }
    }

    #region API Calls
    void GetAllProductCategories()
    {
        StartCoroutine(new ProductAPIs().GetAllProductCategoriesRequest(
            (result) =>
            {
                var json = JsonConvert.DeserializeObject<ProductCategoryResponse>(result);
                GenerateProductCategoryMenu(json.data);
            },
            (error) =>
            {
                Debug.LogError("Error fetching active product categories: " + error);
            }
        ));
    }

    void GetProductByCategoryId(string categoryId)
    {

        if (currentProductRequest != null)
        {
            StopCoroutine(currentProductRequest);
            currentProductRequest = null;
        }

        currentProductRequest = StartCoroutine(new ProductAPIs().GetProductsByCategoryIdRequest(
             categoryId,
             (result) =>
             {
                 var json = JsonConvert.DeserializeObject<ProductResponse>(result);
                 GenerateListProduct(json.data);
             },
             (error) =>
             {
                 Debug.LogError("Error fetching active product categories: " + error);
             }
         ));
    }

    void GetAllProduct()
    {

        if (currentProductRequest != null)
        {
            StopCoroutine(currentProductRequest);
            currentProductRequest = null;
        }

        currentProductRequest = StartCoroutine(new ProductAPIs().GetAllProductsRequest(
             (result) =>
             {
                 var json = JsonConvert.DeserializeObject<ProductResponse>(result);
                 GenerateListProduct(json.data);
             },
             (error) =>
             {
                 Debug.LogError("Error fetching active product categories: " + error);
             }
         ));
    }
    #endregion


    #region UI Generation
    void GenerateProductCategoryMenu(List<ProductCategoryModel> categories)
    {
        if (!categories.Any(c => c.id == "all"))
        {
            categories.Insert(0, new ProductCategoryModel { id = "all", name = "Tất cả" });
        }

        foreach (var category in categories)
        {
            GameObject categoryItem = Instantiate(ProductCategoryItemPrefab, ProductCategoryContent);
            categoryItem.GetComponentInChildren<TMP_Text>().text = category.name;
            categoryItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                ChangeBackgroundColor(categoryItem);
                if (category.id == "all")
                {
                    GetAllProduct();
                }
                else
                {
                    GetProductByCategoryId(category.id);
                }
            });
        }
        // Default select first category
        if (ProductCategoryContent.GetChild(0).gameObject != null && categories.Count > 0)
        {
            ProductCategoryContent.GetChild(0).gameObject.GetComponent<Button>().onClick.Invoke();
        }
    }

    void GenerateListProduct(List<ProductModel> products)
    {

        foreach (Transform child in productListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (var product in products)
        {
            GameObject productItem = Instantiate(ProductItemPrefab, productListContent);
            productItem.GetComponent<ProductItemController>().Setup(product);
            productItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                var json = JsonConvert.SerializeObject(product);
                PlayerPrefs.SetString("selectedProduct", json);
                PlayerPrefs.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("ProductDetail");
            });
        }

    }
    #endregion



    void ChangeBackgroundColor(GameObject selectedCategory)
    {
        foreach (Transform child in ProductCategoryContent)
        {
            var image = child.GetComponent<UnityEngine.UI.Image>();
            if (child.gameObject == selectedCategory)
            {
                image.sprite = ProductCategoryBgActive;
            }
            else
            {
                image.sprite = ProductCategoryBg;
            }
        }
    }

    public void OnBackButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Auth");
    }
}
