using System.Collections.Generic;

[System.Serializable]
public class CostumeModel
{
    public string id;
    public string name;
    public string description;
    public string shortDescription;
    public string status;
    public bool isPopular;
    public string eraId;
    public EraModel era;
    public List<MediaModel> media;
}

[System.Serializable]
public class CostumeResponse
{
    public List<CostumeModel> data;
    public int statusCode;
}
