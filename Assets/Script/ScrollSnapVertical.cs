using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ScrollSnapVertical
/// - Hiển thị list item dọc
/// - Item ở trung tâm sẽ được chọn (scale lớn hơn, alpha rõ hơn)
/// - Các item càng xa center càng nhỏ và mờ dần
/// - Cho phép drag để chọn item
/// - Cho phép chọn item bằng hàm SetItemSelected(index)
/// </summary>
public class ScrollSnapVertical : MonoBehaviour, IEndDragHandler
{
    [Header("References")]
    public ScrollRect scrollRect;
    public RectTransform content;

    [Header("Effect Settings")]
    public float scaleOffset = 0.5f;         // Item ở center sẽ to hơn
    public float scaleSpeed = 10f;           // Tốc độ scale
    public float fadeOffset = 0.5f;          // Item xa thì mờ dần
    public float snapSmoothTime = 0.15f;     // Thời gian smooth khi snap
    public float snapThreshold = 5f;         // Ngưỡng snap (pixel)

    [Header("Events")]
    public Action<int, RectTransform> OnItemSelected;

    private readonly List<RectTransform> items = new List<RectTransform>();
    private Vector2 velocity;
    private Vector2 targetAnchoredPos;
    private bool isSnapping = false;
    private int currentIndex = -1;

    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Lấy danh sách item từ content
    /// </summary>
    public void Initialize()
    {
        items.Clear();
        foreach (Transform child in content)
        {
            if (child is RectTransform rt)
                items.Add(rt);
        }

        currentIndex = -1;
        isSnapping = false;
        velocity = Vector2.zero;
    }

    void Update()
    {
        UpdateItemEffects();
        HandleSnapping();
    }

    /// <summary>
    /// Scale + Alpha của item theo khoảng cách tới center
    /// </summary>
    private void UpdateItemEffects()
    {
        foreach (var item in items)
        {
            if (item == null) continue;

            // vị trí item trong local space của viewport
            Vector3 itemLocal = scrollRect.viewport.InverseTransformPoint(item.position);
            float distance = Mathf.Abs(itemLocal.y);

            // scale
            float scale = 1f + scaleOffset * (1f - Mathf.Clamp01(distance / 300f));
            item.localScale = Vector3.Lerp(item.localScale, new Vector3(scale, scale, 1f), Time.deltaTime * scaleSpeed);

            // alpha
            float alpha = 1f - fadeOffset * Mathf.Clamp01(distance / 300f);
            SetAlpha(item, alpha);
        }
    }

    /// <summary>
    /// Smooth snap tới targetAnchoredPos
    /// </summary>
    private void HandleSnapping()
    {
        if (!isSnapping) return;

        content.anchoredPosition = Vector2.SmoothDamp(
            content.anchoredPosition,
            targetAnchoredPos,
            ref velocity,
            snapSmoothTime
        );

        bool reachedTarget = (content.anchoredPosition - targetAnchoredPos).sqrMagnitude <= snapThreshold * snapThreshold;
        if (reachedTarget || velocity.sqrMagnitude < 0.01f)
        {
            content.anchoredPosition = targetAnchoredPos;
            isSnapping = false;
            velocity = Vector2.zero;

            SelectClosestItem();
        }
    }

    /// <summary>
    /// Khi drag xong → snap tới item gần center
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        SnapToClosestItem();
        SelectClosestItem();
    }

    private void SnapToClosestItem()
    {
        RectTransform closest = GetClosestItem();
        if (closest == null) return;

        Vector3 itemLocal = scrollRect.viewport.InverseTransformPoint(closest.position);
        float offset = -itemLocal.y;

        targetAnchoredPos = content.anchoredPosition + new Vector2(0, offset);
        isSnapping = true;
    }

    private void SelectClosestItem()
    {
        RectTransform closest = GetClosestItem();
        int index = items.IndexOf(closest);

        if (index != -1 && index != currentIndex)
        {
            currentIndex = index;
            OnItemSelected?.Invoke(index, closest);
        }
    }

    private RectTransform GetClosestItem()
    {
        RectTransform closest = null;
        float minDistance = float.MaxValue;

        foreach (var item in items)
        {
            Vector3 itemLocal = scrollRect.viewport.InverseTransformPoint(item.position);
            float distance = Mathf.Abs(itemLocal.y);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = item;
            }
        }
        return closest;
    }

    /// <summary>
    /// Chọn item theo index (snap tới center)
    /// </summary>
    public void SetItemSelected(int index)
    {
        if (index < 0 || index >= items.Count) return;

        RectTransform targetItem = items[index];
        Vector3 itemLocal = scrollRect.viewport.InverseTransformPoint(targetItem.position);
        float offset = -itemLocal.y;

        targetAnchoredPos = content.anchoredPosition + new Vector2(0, offset);
        isSnapping = true;

        // Gọi callback ngay lập tức
        if (index != currentIndex)
        {
            currentIndex = index;
            OnItemSelected?.Invoke(index, targetItem);
        }
    }

    private void SetAlpha(RectTransform item, float alpha)
    {
        CanvasGroup cg = item.GetComponent<CanvasGroup>();
        if (cg == null) cg = item.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = alpha;
    }
}