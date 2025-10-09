using System.Collections.Generic;

[System.Serializable]
public class EraModel
{
    public string id;
    public string name;
    public string period;
    public string description;
    public List<MediaModel> media;
}

[System.Serializable]
public class EraResponse
{
    public List<EraModel> data;
    public int statusCode;
}
