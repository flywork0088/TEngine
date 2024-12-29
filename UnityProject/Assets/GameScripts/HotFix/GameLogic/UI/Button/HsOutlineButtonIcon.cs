using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class HsOutlineButtonIcon : Button
{
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Sprite shadowSprite;
    [SerializeField] private Sprite iconSprite;
    [SerializeField] private Color normalColor = new Color(0.18f, 0.737f, 0.067f, 1f);
    [SerializeField] private bool enablePressedColor = false;
    [SerializeField] private Color pressedColor = new Color(0.18f, 0.737f, 0.067f, 1f);
    [SerializeField] private bool enablePressedIconColor = false;
    [SerializeField] private Color pressedIconColor = Color.white;
    [SerializeField] private Color outlineColor = new Color(0.059f, 0.567f, 0.035f, 1f);
    [SerializeField] private Color disabledColor = new Color(0.898f, 0.898f, 0.898f, 1f);
    [SerializeField] private Color disabledIconColor = new Color(0.663f, 0.663f, 0.663f, 1f);
    [SerializeField] private Color iconColor = Color.white;
    [SerializeField] private float outlineThickness = 4f;
    [SerializeField] private float shadowOffset = 9f;

    private Image buttonBackground;
    private Image shadowImage;
    private Image iconImage;
    private RectTransform backgroundRect;
    private RectTransform shadowRect;
    private RectTransform iconRect;
    private Color normalBackgroundColor;
    private Color normalIconColor;

    protected override void Start()
    {
        base.Start();
        InitializeComponents();
        SetupButton();
    }

    private void InitializeComponents()
    {
        buttonBackground = transform.Find("Background")?.GetComponent<Image>();
        shadowImage = transform.Find("Shadow")?.GetComponent<Image>();
        iconImage = transform.Find("Icon")?.GetComponent<Image>();

        if (buttonBackground) backgroundRect = buttonBackground.GetComponent<RectTransform>();
        if (shadowImage) shadowRect = shadowImage.GetComponent<RectTransform>();
        if (iconImage)
        {
            iconRect = iconImage.GetComponent<RectTransform>();
        }

        var defaultImage = GetComponent<Image>();
        if (defaultImage) defaultImage.enabled = false;

        targetGraphic = buttonBackground;
    }

    public void SetupButton()
    {
        normalBackgroundColor = normalColor;
        normalIconColor = iconColor;

        if (buttonBackground)
        {
            buttonBackground.sprite = backgroundSprite;
            buttonBackground.color = normalColor;
            buttonBackground.type = Image.Type.Sliced;
            backgroundRect.anchoredPosition = Vector2.zero;
        }

        if (shadowImage)
        {
            shadowImage.sprite = shadowSprite;
            shadowImage.color = outlineColor;
            shadowImage.type = Image.Type.Sliced;
            SetupOutline(false);
        }

        if (iconRect)
        {
            iconImage.sprite = iconSprite;
            iconImage.color = iconColor;
            iconRect.anchoredPosition = Vector2.zero;
        }
    }

    private void SetupOutline(bool isPressed)
    {
        if (shadowRect)
        {
            float sideWidth = outlineThickness;

            if (isPressed)
            {
                // 按下状态：上边下移，其他边保持不变
                shadowRect.offsetMin = new Vector2(-sideWidth, -(sideWidth + shadowOffset)); // 左、底
                shadowRect.offsetMax = new Vector2(sideWidth, sideWidth - shadowOffset); // 右、上
            }
            else
            {
                // 正常状态：只有底部延伸
                shadowRect.offsetMin = new Vector2(-sideWidth, -(sideWidth + shadowOffset)); // 左、底
                shadowRect.offsetMax = new Vector2(sideWidth, sideWidth); // 右、上
            }
    
            //Log.InfoGreen($"shadowRect.offsetMin: {shadowRect.offsetMin}, shadowRect.offsetMax: {shadowRect.offsetMax}");
            //shadowRect.anchoredPosition = Vector2.zero;
        }
    }
    




    public void UpdateButton()
    {
        if (buttonBackground)
        {
            buttonBackground.sprite = backgroundSprite;
            buttonBackground.type = Image.Type.Sliced;
            normalBackgroundColor = normalColor;
            buttonBackground.color = IsInteractable() ? normalBackgroundColor : disabledColor;
        }

        if (shadowImage)
        {
            shadowImage.sprite = shadowSprite;
            shadowImage.type = Image.Type.Sliced;
            shadowImage.color = outlineColor;
            SetupOutline(false);
        }

        if (iconImage)
        {
            iconImage.sprite = iconSprite;
            normalIconColor = iconColor;
            iconImage.color = IsInteractable() ? normalIconColor : disabledIconColor;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable() || !IsActive())
            return;

        base.OnPointerDown(eventData);

        if (backgroundRect)
        {
            backgroundRect.anchoredPosition = new Vector2(0, -shadowOffset);
            if (iconRect != null)
            {
                iconRect.anchoredPosition = new Vector2(0, -shadowOffset);
            }

            SetupOutline(true);

            if (enablePressedColor)
            {
                buttonBackground.color = pressedColor;
            }

            if (enablePressedIconColor && iconImage)
            {
                iconImage.color = pressedIconColor;
            }
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        base.OnPointerUp(eventData);
        ResetPosition();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        ResetPosition();
    }

    private void ResetPosition()
    {
        if (!IsActive())
            return;

        if (backgroundRect)
        {
            backgroundRect.anchoredPosition = Vector2.zero;
            if (iconRect != null)
            {
                iconRect.anchoredPosition = Vector2.zero;
            }

            SetupOutline(false);

            if (IsInteractable())
            {
                buttonBackground.color = normalBackgroundColor;
                if (iconImage) iconImage.color = normalIconColor;
            }
            else
            {
                buttonBackground.color = disabledColor;
                if (iconImage) iconImage.color = disabledIconColor;
            }
        }
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (!gameObject.activeInHierarchy)
            return;

        switch (state)
        {
            case SelectionState.Normal:
                if (buttonBackground) buttonBackground.color = normalBackgroundColor;
                if (iconImage) iconImage.color = normalIconColor;
                SetupOutline(false);
                break;

            case SelectionState.Disabled:
                if (buttonBackground) buttonBackground.color = disabledColor;
                if (iconImage) iconImage.color = disabledIconColor;
                SetupOutline(false);
                break;
        }
    }

    public void SetIcon(Sprite icon, Color? color = null)
    {
        iconSprite = icon;
        if (color.HasValue)
        {
            iconColor = color.Value;
            normalIconColor = color.Value;
        }

        if (iconImage)
        {
            iconImage.sprite = iconSprite;
            iconImage.color = IsInteractable() ? normalIconColor : disabledIconColor;
        }
    }
}
