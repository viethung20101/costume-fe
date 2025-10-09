using UnityEngine;
using System.Collections.Generic;
public class ProductCategoryManager : MonoBehaviour
{
    public static ProductCategoryManager Instance;
    public List<ProductCategoryModel> productCategories = new List<ProductCategoryModel>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
