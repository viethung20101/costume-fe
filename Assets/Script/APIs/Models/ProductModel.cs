using System.Collections.Generic;

[System.Serializable]
public class ProductCategoryModel
{
    public string id;
    public string name;
    public string description;
    public int _countComponents;
}

[System.Serializable]
public class ProductCategoryResponse
{
    public List<ProductCategoryModel> data;
    public int statusCode;
}


[System.Serializable]
public class ProductModel
{
    public string id;
    public string name;
    public string description;
    public string shortDescription;
    public float price;
    public float originalPrice;
    public string categoryId;
    public string sku;
    public float stock;
    public bool isActive;
    public bool isFeatured;
    public ProductCategoryModel category;
    public List<MediaModel> media;
}

[System.Serializable]
public class ProductResponse
{
    public List<ProductModel> data;
    public int statusCode;
}
