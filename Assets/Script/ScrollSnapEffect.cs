using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ScrollSnapEffect : MonoBehaviour, IEndDragHandler
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public float scaleOffset = 0.5f;
    public float scaleSpeed = 10f;
    public float fadeOffset = 0.5f;
    public float snapSpeed = 10f; // tốc độ snap

    private List<RectTransform> items = new List<RectTransform>();
    private Vector2 center;
    private bool isSnapping = false;
    private Vector3 targetPosition;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        items.Clear();
        targetPosition = Vector3.zero;
        isSnapping = false;
        foreach (Transform child in content)
        {
            items.Add(child.GetComponent<RectTransform>());
        }

        center = scrollRect.viewport.position;
    }

    public void SnapToItem(RectTransform targetItem)
    {
        if (targetItem == null) return;

        // Lấy vị trí "center" của item trong local space
        Vector3 itemLocalPos = content.InverseTransformPoint(targetItem.position);
        Vector3 centerLocalPos = content.InverseTransformPoint(scrollRect.viewport.position);

        // Tính offset từ item đến giữa viewport
        float offset = centerLocalPos.x - itemLocalPos.x;

        // Set targetPosition
        targetPosition = content.localPosition + new Vector3(offset, 0f, 0f);
        isSnapping = true;
    }



    void Update()
    {
        // Hiệu ứng scale + alpha
        foreach (var item in items)
        {
            if (item == null) continue;

            float distance = Mathf.Abs(center.x - item.position.x);

            // Scale
            float scale = 1f + scaleOffset * (1f - Mathf.Clamp01(distance / 300f));
            item.localScale = Vector3.Lerp(item.localScale, new Vector3(scale, scale, 1f), Time.deltaTime * scaleSpeed);

            // Alpha
            float alpha = 1f - fadeOffset * Mathf.Clamp01(distance / 300f);
            SetAlpha(item, alpha);
        }
        // Snap content về targetPosition
        if (isSnapping)
        {
            content.localPosition = Vector3.Lerp(content.localPosition, targetPosition, Time.deltaTime * snapSpeed);
            if (Vector3.Distance(content.localPosition, targetPosition) < 0.1f)
            {
                content.localPosition = targetPosition;
                isSnapping = false;
            }
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        RectTransform closest = null;
        float minDistance = float.MaxValue;
        foreach (var item in items)
        {
            float distance = Mathf.Abs(center.x - item.position.x);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = item;
            }
        }

        if (closest != null)
        {
            float offset = center.x - closest.position.x;
            targetPosition = content.localPosition + new Vector3(offset, 0f, 0f);
            isSnapping = true;
        }
    }

    void SetAlpha(RectTransform item, float alpha)
    {
        CanvasGroup cg = item.GetComponent<CanvasGroup>();
        if (cg == null) cg = item.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = alpha;
    }
}
