using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
public class ScriptChoose : MonoBehaviour
{
   [SerializeField]  private string nameScene;
   [SerializeField] private Vector3 InitalScale;
   [SerializeField] private float Range;
   [SerializeField] Transform FeetPos;
   [SerializeField] Button button;
      private void Awake()
    {
        InitalScale = transform.localScale;
    }
    void Start()
    {
        
    }

   public void Choose()
   {
      InCreateScale(true);
      StartCoroutine(StopTemporary());
      button.interactable = false;
   }
    public void Off_Button()
   {
    InCreateScale(false);
    button.interactable = true;
   }
   IEnumerator StopTemporary()
   {
      yield return new WaitForSeconds(Range);
       SceneManager.LoadScene(nameScene);
   }
    private void InCreateScale(bool isScale)
   {
    Vector3 finalScale = InitalScale;
    if(isScale)
    finalScale = InitalScale * 2f;
     FeetPos.DOScale(finalScale, 0.15f);
   }
}
