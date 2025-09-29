using UnityEngine;

public class CanvaPlayer : MonoBehaviour
{
    [SerializeField] GameObject[] Model_Character;
    [SerializeField] GameObject[] ScrollAreaVertical;
    void Start()
    {
        for(int i =0; i < ScrollAreaVertical.Length; i++)
        {
            ScrollAreaVertical[i].SetActive(false);
        }
        for(int i = 0; i < Model_Character.Length; i++)
        {
            Model_Character[i].SetActive(false);
        }
          ScrollAreaVertical[0].SetActive(true);
            Model_Character[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
