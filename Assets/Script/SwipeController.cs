using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using DG.Tweening;
public class SwipeController : MonoBehaviour, IEndDragHandler
{
         [SerializeField] private int maxPage;
    private int currentPage;
    private Vector3 targetPos;
    [SerializeField] private Vector3 pageStep;
    [SerializeField] private RectTransform levelPagesRect;
    [SerializeField] private float tweenTime = 0.3f;
    [SerializeField] private LeanTweenType tweenType = LeanTweenType.easeOutCubic;

    [Header("UI Indicators")]
    [SerializeField] private Image[] imageBar;
    [SerializeField] private Transform[] imageScale;
    [SerializeField] Button[] buttonBar;

    [Header("Scale Settings")]
    [SerializeField] private float bigScale = 1.2f;
    [SerializeField] private float smallScale = 0.8f;
    [SerializeField] private float scaleDuration = 0.2f;

    private float dragThreshold;
    [Header("Veritcal Scroll")]
    [SerializeField] GameObject[] ScrollViewCharacter;
     [SerializeField] GameObject[] CharacterName;

    private void Awake()
    {
        currentPage = 2; // index bắt đầu từ 0
        targetPos = levelPagesRect.localPosition;
        dragThreshold = Screen.width / 15f;

       // UpdateBar();
        UpdateScale();
        UpdateButton();
          UpdateScrollViewVertical();
           UpdateParentCharacter();
    }
    private void Start()
    {
      
        

    }
    public void Next()
    {
        if (currentPage < maxPage - 1)
        {
            currentPage++;
            targetPos += pageStep;
            MovePage();
        }
    }

    public void Previous()
    {
        if (currentPage > 0)
        {
            currentPage--;
            targetPos -= pageStep;
            MovePage();
        }
    }

    void MovePage()
    {
        levelPagesRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
       // UpdateBar();
        UpdateScale();
        UpdateButton();
        UpdateScrollViewVertical();
        UpdateParentCharacter();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float delta = eventData.position.x - eventData.pressPosition.x;

        if (Mathf.Abs(delta) > dragThreshold)
        {
            if (delta > 0) Previous();
            else Next();
        }
        else
        {
            MovePage();
        }
    }

    void UpdateBar()
    {
        if (imageBar == null || imageBar.Length == 0)
            return;

        for (int i = 0; i < imageBar.Length; i++)
        {
            imageBar[i].color = (i == currentPage) ? Color.red : Color.white;
        }
    }

    void UpdateScale()
    {
        if (imageScale == null || imageScale.Length == 0)
            return;

        for (int i = 0; i < imageScale.Length; i++)
        {
            float target = (i == currentPage) ? bigScale : smallScale;
            imageScale[i].DOScale(Vector3.one * target, scaleDuration).SetEase(Ease.OutBack);
        }
    }
    void UpdateButton()
    {
       if (buttonBar == null || buttonBar.Length == 0)
            return;
      
       for(int i = 0; i < buttonBar.Length; i++)
       {
         buttonBar[i].interactable = false;
       }
        buttonBar[currentPage].interactable = true;
    }
    void UpdateParentCharacter()
    {
         if (CharacterName == null || CharacterName.Length == 0)
            return;
      for(int i = 0; i < CharacterName.Length; i++)
      {
        CharacterName[i].SetActive(false);
      }
      CharacterName[currentPage].SetActive(true);
    }
    void UpdateScrollViewVertical()
    {
         if (ScrollViewCharacter == null || ScrollViewCharacter.Length == 0)
            return;
        for(int i = 0; i < ScrollViewCharacter.Length; i++)
        {
            ScrollViewCharacter[i].SetActive(false);
        }
         ScrollViewCharacter[currentPage].SetActive(true);
    }   
        
    
    
}
