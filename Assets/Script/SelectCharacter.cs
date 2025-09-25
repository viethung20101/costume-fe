using UnityEngine;

public class SelectCharacter : MonoBehaviour
{
    [SerializeField] IndexSelectCharacter selectCharacter;
    void Start()
    {
        Selected();
    }

    // Update is called once per frame
    void Update()
    {
        float PreviousSelect = selectCharacter.Select;
        if(PreviousSelect != selectCharacter.Select)
        {
            Selected();
        }
    }
    public void Selected()
    {
        int i = 0;
        foreach(Transform character in transform)
        {
            if(i == selectCharacter.Select)
            {
                character.gameObject.SetActive(true);
            }
            else
            {
                character.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
