using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HsFlatButtonIcon : Button
{
    [SerializeField] private Sprite iconSprite;
    [SerializeField] private Color iconColor = Color.white;
    [SerializeField] private Color disabledIconColor = new Color(0.663f, 0.663f, 0.663f, 1f);
    [SerializeField] private Vector2 hitAreaSize = new Vector2(132f, 132f);
    [SerializeField] private Vector2 iconSize = new Vector2(108f, 108f);

    private Image iconImage;
    private Image backgroundImage;
    private RectTransform iconRect;
    private RectTransform rectTransform;
    private const float ScaleFactor = 1.2f;
    private const float AnimationDuration = 0.1f;

    protected override void Awake()
    {
        base.Awake();
        InitializeComponents();
        SetupButton();
    }

    private void InitializeComponents()
    {
        rectTransform = GetComponent<RectTransform>();
        backgroundImage = transform.Find("Background")?.GetComponent<Image>();
        if (backgroundImage)
        {
            backgroundImage.type = Image.Type.Simple;
            backgroundImage.color = new Color(0, 0, 0, 0);
            var backgroundRect = backgroundImage.GetComponent<RectTransform>();
            backgroundRect.sizeDelta = hitAreaSize;
        }
        
        iconImage = transform.Find("Icon")?.GetComponent<Image>();
        if (iconImage)
        {
            iconRect = iconImage.GetComponent<RectTransform>();
            iconImage.type = Image.Type.Simple;
            iconRect.sizeDelta = iconSize;
        }

        transition = Transition.None;
        targetGraphic = null;
    }

    private void SetupButton()
    {
        if (iconImage)
        {
            iconImage.sprite = iconSprite;
            iconImage.color = IsInteractable() ? iconColor : disabledIconColor;
        }
    }

    public void UpdateHitArea()
    {
        if (backgroundImage)
        {
            var backgroundRect = backgroundImage.GetComponent<RectTransform>();
            backgroundRect.sizeDelta = hitAreaSize;
            rectTransform.sizeDelta = hitAreaSize;
        }
    }

    public void UpdateIconSize()
    {
        if (!iconImage || !iconRect) return;

        if (iconSprite != null)
        {
            iconImage.SetNativeSize();
            iconSize = iconRect.sizeDelta;
            hitAreaSize = new Vector2
            (
                Mathf.RoundToInt(iconSize.x * 1.22f),
                Mathf.RoundToInt(iconSize.y * 1.22f)
            );
            UpdateHitArea();
        }
        else
        {
            iconRect.sizeDelta = iconSize;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        
        base.OnPointerDown(eventData);
        if (iconRect)
        {
            iconRect.DOKill();
            iconRect.DOScale(ScaleFactor, AnimationDuration).SetEase(Ease.OutQuad);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        
        base.OnPointerUp(eventData);
        RestoreScale();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        
        base.OnPointerExit(eventData);
        RestoreScale();
    }

    private void RestoreScale()
    {
        if (iconRect)
        {
            iconRect.DOKill();
            iconRect.DOScale(1f, AnimationDuration).SetEase(Ease.OutQuad);
        }
    }

    protected override void OnDestroy()
    {
        if (iconRect)
        {
            iconRect.DOKill();
        }
        base.OnDestroy();
    }

    public void SetIconSize(Vector2 size)
    {
        iconSize = size;
        if (iconRect)
        {
            iconRect.sizeDelta = iconSize;
        }
    }

    public void SetHitAreaSize(Vector2 size)
    {
        hitAreaSize = size;
        UpdateHitArea();
    }
    
    public void SetIcon(Sprite icon, Color? color = null)
    {
        iconSprite = icon;
        if (color.HasValue)
        {
            iconColor = color.Value;
        }

        if (iconImage)
        {
            iconImage.sprite = iconSprite;
            iconImage.color = IsInteractable() ? iconColor : disabledIconColor;
            UpdateIconSize();
        }
    }

    public void SetIconColor(Color color)
    {
        iconColor = color;
        iconImage.color = color;
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (!gameObject.activeInHierarchy)
            return;

        switch (state)
        {
            case SelectionState.Normal:
                if (iconImage) iconImage.color = iconColor;
                break;

            case SelectionState.Disabled:
                if (iconImage) iconImage.color = disabledIconColor;
                break;
        }
    }
}
