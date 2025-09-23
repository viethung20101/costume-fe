using UnityEngine;
using UnityEngine.UI;
public class RandomImageBackGround : MonoBehaviour
{
    [SerializeField] GameObject[] Image_BackGround;
    void Start()
    {
        for(int i = 0; i < Image_BackGround.Length; i++)
        {
            Image_BackGround[i].SetActive(false);
        }
        int RandomImage = Random.Range(0, Image_BackGround.Length);
        Image_BackGround[RandomImage].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
