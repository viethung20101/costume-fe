using UnityEngine;
using UnityEngine.UI; 
public class ScriptButtonFunction : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] GameObject[] Model_Character;
    void Start()
    {
     
    }
   public void On_Button()
   {
    button.interactable = true; 
    for(int i = 0; i < Model_Character.Length; i++)
    {
        Model_Character[i].SetActive(false);
    }
    Model_Character[0].SetActive(true);
   }
   public void Off_Button()
   {
    button.interactable = false; 
   }
}
