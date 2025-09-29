using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
public class ScriptImageLogo : MonoBehaviour
{
    [SerializeField] Image ImageLogo;
    [SerializeField] SourceScene scriptSourceScene;
    [SerializeField] private float Range;
    [SerializeField] private string[] nameScene;
    void Start()
    {
        ImageLogo.DOColor(new Color(1,1,1,0f), 3);
    }

    // Update is called once per frame
    void Update()
    {
        ImageLogo.DOColor(new Color(1,1,1,1), 3);
        StartCoroutine(StopTemporary());
    }
    IEnumerator StopTemporary()
    {
        yield return new WaitForSeconds(Range);
        SceneManager.LoadScene(nameScene[scriptSourceScene.Source]);
    }
}
