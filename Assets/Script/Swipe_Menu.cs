using UnityEngine;
using UnityEngine.UI;
public class Swipe_Menu : MonoBehaviour
{
    public GameObject scrollbar;
    private float scroll_pos = 10;
    float[] pos;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            pos [i] = distance * i;
        }
        if(Input.GetMouseButton(0))
        {
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
        {
           if(scroll_pos < pos[i] + (distance/1) && scroll_pos > pos[i] - (distance/1)){
            scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
           }
        }
        }
        for (int i = 0; i < pos.Length; i++)
        {
           if(scroll_pos < pos[i] + (distance/3) && scroll_pos > pos[i] - (distance/3)){
            transform.GetChild ( i).localScale = Vector3.Lerp(transform.GetChild(i).localScale, new Vector3(2,7,15), 0.1f);
            for (int a = 0; a < pos.Length; a++)
            {
                if(a != i)
                {
                    transform.GetChild(a).localScale = Vector3.Lerp(transform.GetChild(a).localScale, new Vector3(1.252015f, 6.069648f,10), 0.1f);
                }
            }
           }
        }
    }
}
