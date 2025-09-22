using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ScriptScrollAreaVertical : MonoBehaviour
{
    [SerializeField]private string Name;
    [Header("Model")]
   [SerializeField] GameObject[] Model_Character;
    [SerializeField] private Vector3 InitalScale;
    [SerializeField] Button button;
     private void Awake()
    {
        InitalScale = transform.localScale;
    }
   public void On_Button()
   {
    InCreateScale(true);
      button.interactable = false; 
 
    for(int i = 0; i < Model_Character.Length; i++)
    {
        Model_Character[i].SetActive(false);
       
    }
    Model_Character[0].SetActive(true);
  
   }
   public void Off_Button()
   {
    InCreateScale(false);
       button.interactable = true; 
   }
     private void InCreateScale(bool isScale)
   {
    Vector3 finalScale = InitalScale;
    if(isScale)
    finalScale = InitalScale * 2f;
     transform.DOScale(finalScale, 0.15f);
   }
}
