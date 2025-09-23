using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ScriptImageLogo : MonoBehaviour
{
    [SerializeField] Image ImageLogo;
    [SerializeField] ScriptSourceScene scriptSourceScene;
    void Start()
    {
        ImageLogo.DOColor(new Color(1,1,1,0f), 3);
    }

    // Update is called once per frame
    void Update()
    {
        ImageLogo.DOColor(new Color(1,1,1,1), 3);
        SceneManager.LoadScene(scriptSourceScene.CurrentSource);
    }
}
