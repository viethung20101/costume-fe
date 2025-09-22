using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class ScriptSourceScene : MonoBehaviour
{
    static public int _Source;
    private void Awake()
    {
       _Source = 1;
        if(PlayerPrefs.HasKey("HighSource"))
        {
           Source = PlayerPrefs.GetInt("HighSource");
        }
        else
        {
           PlayerPrefs.GetInt("HighSource",Source);
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
