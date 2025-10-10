using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
public class TextDialogue_VN : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextComponent;
    [SerializeField] private float Speed;
    [TextArea(3,100)]
    [SerializeField] string[] Lines;
    [SerializeField] private int Index;
    void Start()
    {
      TextComponent.text = string.Empty;
      StartDialogue();        
    }
   private void StartDialogue()
   {
    Index = 0;
    StartCoroutine(TypeLines());
   }
   private void NextLines()
   {
    if(Index < Lines.Length - 1)
    {
        Index++;
      
        TextComponent.text = string.Empty;
          StartCoroutine(TypeLines());
    }
    else
    {
       gameObject.SetActive(true);
    }
   }
   IEnumerator TypeLines()
   {
    foreach(char C in Lines[Index].ToCharArray())
    {
        TextComponent.text += C;
       yield return new WaitForSeconds(Speed);
    }
   }
    // Update is called once per frame
    public void Update_Text(int Index)
    {
        if (TextComponent.text == Lines[Index])
        {
             NextLines();
        }
        else
        {
            StopAllCoroutines();
            TextComponent.text = Lines[Index];
        }
    }
}
