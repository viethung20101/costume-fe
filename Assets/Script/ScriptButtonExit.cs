using UnityEngine;
using UnityEngine.SceneManagement;
public class ScriptButtonExit : MonoBehaviour
{
    public string nameScene;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ButtonExitAndMenu()
    {
        SceneManager.LoadScene(nameScene);
    }
}
