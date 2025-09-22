using UnityEngine;
using UnityEngine.SceneManagement;
public class ScriptChoose : MonoBehaviour
{
   [SerializeField]  private string nameScene;

    void Start()
    {
        
    }

   public void Choose()
   {
    SceneManager.LoadScene(nameScene);
   }
}
