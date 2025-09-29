using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using DG.Tweening;
public class SwipeVerticalController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
 [SerializeField] private RectTransform levelPagesRect;  // Content
    [SerializeField] private RectTransform viewport;        // Viewport
    [SerializeField] private Transform[] items;             // Items
    [SerializeField] public Button[] button;
    

    [Header("Tween Settings")]
    [SerializeField] private float tweenTime = 0.3f;

    [Header("Scale Settings")]
    [SerializeField] private float bigScale = 1.2f;
    [SerializeField] private float smallScale = 0.8f;
    [SerializeField] private float scaleDuration = 0.2f;

    private bool isDragging;
    private int currentPage;
    private Vector2 dragStartPos;
    private Vector3 contentStartPos;
    public Vector3 ScaleButton;
    public float dragThreshold;
    public int Index;
    [SerializeField] GameObject[] Character_Model;
    [SerializeField] GameObject[] CharacterName;
    private void Awake()
    {
        dragThreshold = Screen.height / 15f;
        
      
    }

    private void Start()
    {
        SnapToItem(Index);
        Character_Model[0].SetActive(true);
      
    }

    void LateUpdate()
    {
        if (isDragging) return;
        int nearestIndex = GetNearestIndex();
        UpdateScale(nearestIndex);
        UpdateColorButton(nearestIndex);
       
    }

    private int GetNearestIndex()
    {
        float nearest = float.MaxValue;
        int nearestIndex = 0;

        float viewportCenterY = viewport.position.y;

        for (int i = 0; i < items.Length; i++)
        {
            float distance = Mathf.Abs(viewportCenterY - items[i].position.y);
            if (distance < nearest)
            {
                nearest = distance;
                nearestIndex = i;
            }
        }
        return nearestIndex;
    }

    private void SnapToItem(int index)
    {
        if (index < 0 || index >= items.Length) return;

        currentPage = index;

        float viewportCenterY = viewport.position.y;
        float itemY = items[index].position.y;
        float offset = viewportCenterY - itemY;

        Vector3 targetPos = levelPagesRect.localPosition + new Vector3(0, offset, 0);

        // Clamp dựa trên content size
        float contentHeight = levelPagesRect.rect.height;
        float viewportHeight = viewport.rect.height;
        float minY = -(contentHeight - viewportHeight) / 2f;
        float maxY = (contentHeight - viewportHeight) / 2f;

        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        levelPagesRect.DOLocalMove(targetPos, tweenTime).SetEase(Ease.OutCubic);

        UpdateScale(index);
        UpdateColorButton(index);
        UpdateCharacterModel(index);
    }

    private void UpdateScale(int activeIndex)
    {
        for (int i = 0; i < items.Length; i++)
        {
          Vector3 targetScale = (i == activeIndex) ? new Vector3(1f, bigScale, 1f) : ScaleButton;
          items[i].DOScale(targetScale, scaleDuration).SetEase(Ease.OutBack);
        }
    }

    private void UpdateColorButton(int index)
    {
        for (int i = 0; i < button.Length; i++)
        {
           button[i].interactable = false;
            button[i].image.raycastTarget = false;
        }
        button[index].interactable = true;
         button[index].image.raycastTarget = true;
    }
   private void UpdateCharacterModel(int index)
   {
       for(int i = 0; i < Character_Model.Length; i++)
       {
        Character_Model[i].SetActive(false);
       }
       Character_Model[index].SetActive(true);
   }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        dragStartPos = eventData.position;
        contentStartPos = levelPagesRect.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - dragStartPos;
        Vector3 newPos = contentStartPos + new Vector3(0, delta.y, 0);

        // Clamp content khi kéo
        float contentHeight = levelPagesRect.rect.height;
        float viewportHeight = viewport.rect.height;
        float minY = -(contentHeight - viewportHeight) / 2f;
        float maxY = (contentHeight - viewportHeight) / 2f;

        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        levelPagesRect.localPosition = newPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        int nearestIndex = GetNearestIndex();
        SnapToItem(nearestIndex);
    }
}
