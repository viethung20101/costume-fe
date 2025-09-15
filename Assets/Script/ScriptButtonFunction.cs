using UnityEngine;
using UnityEngine.UI; 
public class ScriptButtonFunction : MonoBehaviour
{
    [SerializeField] Button button;
    void Start()
    {
     
    }
   public void On_Button()
   {
    button.interactable = true; 
   }
   public void Off_Button()
   {
    button.interactable = false; 
   }
}
