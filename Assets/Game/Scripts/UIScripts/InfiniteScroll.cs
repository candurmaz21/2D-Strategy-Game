using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfiniteScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IScrollHandler
{
    [SerializeField]
    private ScrollContent scrollContent;
    [SerializeField]
    private float outOfBoundsThreshold;
    private ScrollRect scrollRect;
    private Vector2 lastDragPosition;
    private bool positiveDrag;
    private float maxScrollSpeed = 1000f;
    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.vertical = scrollContent.Vertical;
        scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
    }
    //Begin drag.
    public void OnBeginDrag(PointerEventData eventData)
    {
        lastDragPosition = eventData.position;
    }

    //OnDrag
    public void OnDrag(PointerEventData eventData)
    {

        Vector2 dragDelta = eventData.position - lastDragPosition;
        dragDelta = Vector2.ClampMagnitude(dragDelta, maxScrollSpeed);
        positiveDrag = dragDelta.y > 0;
        lastDragPosition = eventData.position;
    }
    //OnScroll
    public void OnScroll(PointerEventData eventData)
    {
        Vector2 scrollDelta = eventData.scrollDelta;
        scrollDelta = Vector2.ClampMagnitude(scrollDelta, maxScrollSpeed);
        if (scrollContent.Vertical)
        {
            positiveDrag = scrollDelta.y > 0;
        }
        else
        {
            positiveDrag = scrollDelta.x > 0;
        }
    }
    public void OnViewScroll()
    {
        HandleVerticalScroll();
    }
    //Vertical scroll handler.
    private void HandleVerticalScroll()
    {
        int currItemIndex = positiveDrag ? scrollRect.content.childCount - 1 : 0;
        var currItem = scrollRect.content.GetChild(currItemIndex) as RectTransform;

        if (!ReachedThreshold(currItem))
        {
            return;
        }

        int endItemIndex = positiveDrag ? 0 : scrollRect.content.childCount - 1;
        var endItem = scrollRect.content.GetChild(endItemIndex) as RectTransform;
        Vector2 newPos = endItem.anchoredPosition;

        float totalItemSpacing = (scrollContent.ChildHeight + scrollContent.ItemSpacing);

        if (positiveDrag)
        {
            newPos.y = endItem.anchoredPosition.y - totalItemSpacing;
        }
        else
        {
            newPos.y = endItem.anchoredPosition.y + totalItemSpacing;
        }

        currItem.anchoredPosition = newPos;
        currItem.SetSiblingIndex(endItemIndex);
    }
    private bool ReachedThreshold(Transform item)
    {
        if (scrollContent.Vertical)
        {
            float posYThreshold = transform.position.y + scrollContent.Height * 0.5f + outOfBoundsThreshold;
            float negYThreshold = transform.position.y - scrollContent.Height * 0.5f - outOfBoundsThreshold;
            return positiveDrag ? item.position.y - scrollContent.ChildWidth * 0.5f > posYThreshold :
                item.position.y + scrollContent.ChildWidth * 0.5f < negYThreshold;
        }
        else
        {
            float posXThreshold = transform.position.x + scrollContent.Width * 0.5f + outOfBoundsThreshold;
            float negXThreshold = transform.position.x - scrollContent.Width * 0.5f - outOfBoundsThreshold;
            return positiveDrag ? item.position.x - scrollContent.ChildWidth * 0.5f > posXThreshold :
                item.position.x + scrollContent.ChildWidth * 0.5f < negXThreshold;
        }
    }
}