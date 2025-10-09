using System.Collections.Generic;

[System.Serializable]
public class CategoryModel
{
    public string id;
    public string name;
    public string description;
    public int _countComponents;
}

[System.Serializable]
public class CategoryResponse
{
    public List<CategoryModel> data;
    public int statusCode;
}
