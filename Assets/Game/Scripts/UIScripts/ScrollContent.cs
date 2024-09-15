using UnityEngine;
using UnityEngine.UI;

public class ScrollContent : MonoBehaviour
{
    //Public
    public float ItemSpacing { get { return itemSpacing; } }
    public float VerticalMargin { get { return verticalMargin; } }
    public bool Vertical { get { return vertical; } }
    public float Width { get { return width; } }
    public float Height { get { return height; } }
    public float ChildWidth { get { return childWidth; } }
    public float ChildHeight { get { return childHeight; } }
    //Private
    private RectTransform rectTransform;
    private RectTransform[] rtChildren;
    private float width, height;
    private float childWidth, childHeight;
    [SerializeField]
    private float itemSpacing;
    [SerializeField]
    private float horizontalMargin, verticalMargin;
    [SerializeField]
    private bool horizontal, vertical;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void Initialize()
    {
        UpdateLayout();
    }
    public void UpdateLayout()
    {
        rtChildren = new RectTransform[rectTransform.childCount];

        for (int i = 0; i < rectTransform.childCount; i++)
        {
            rtChildren[i] = rectTransform.GetChild(i) as RectTransform;
        }

        width = rectTransform.rect.width - (2 * horizontalMargin);
        height = rectTransform.rect.height - (2 * verticalMargin);

        if (rtChildren.Length > 0)
        {
            childWidth = rtChildren[0].rect.width;
            childHeight = rtChildren[0].rect.height;
        }

        if (vertical)
            InitializeContentVertical();
        else
            InitializeContentHorizontal();
    }
    private void InitializeContentHorizontal()
    {
        float originX = -width * 0.5f; // Start from the left side
        float posOffset = childWidth * 0.5f;

        for (int i = 0; i < rtChildren.Length; i++)
        {
            Vector2 childPos = rtChildren[i].localPosition;
            childPos.x = originX + posOffset + i * (childWidth + itemSpacing);
            childPos.y = 0; // Center vertically
            rtChildren[i].localPosition = childPos;
        }
    }
    private void InitializeContentVertical()
    {
        float originY = -height * 0.5f;
        float posOffset = childHeight * 0.5f;

        for (int i = 0; i < rtChildren.Length; i++)
        {
            Vector2 childPos = rtChildren[i].localPosition;
            childPos.y = originY + posOffset + i * (childHeight + itemSpacing);
            childPos.x = 0;
            rtChildren[i].localPosition = childPos;
        }
        AdjustContentHeight();
    }
    private void AdjustContentHeight()
    {
        float totalHeight = rtChildren.Length * (childHeight + itemSpacing) - itemSpacing;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }
    public void UpdateAfterAddingChildren()
    {
        UpdateLayout();
        ForceLayoutRebuild();
    }
    public void ForceLayoutRebuild()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}