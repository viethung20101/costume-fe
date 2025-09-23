using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class ScriptSourceScene : MonoBehaviour
{
    static public int _Source;
    public int CurrentSource;
    public static ScriptSourceScene Instance;
    private void Awake()
    {
     _Source =  CurrentSource;
        if(PlayerPrefs.HasKey("HighSource"))
        {
           Source = PlayerPrefs.GetInt("HighSource");
       
        }
        else
        {
           PlayerPrefs.SetInt("HighSource",Source);
         
        }
    }
    static public int Source
  {
     
        get{return _Source;}
        private set{
         _Source = value;
        PlayerPrefs.SetInt("HighSource", value);
        }
  }
    
  
}
