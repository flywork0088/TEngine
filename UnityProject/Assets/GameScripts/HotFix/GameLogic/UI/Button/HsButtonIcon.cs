using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class HsButtonIcon : Button
{
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Sprite shadowSprite;
    [SerializeField] private Sprite iconSprite;
    [SerializeField] private Color normalColor = new Color(0.3216f, 0.5216f, 0.9647f, 1f);
    [SerializeField] private bool enablePressedColor = false;
    [SerializeField] private Color pressedColor = new Color(0.18f, 0.737f, 0.067f, 1f); // #2EBC11
    [SerializeField] private bool enablePressedIconColor = false;
    [SerializeField] private Color pressedIconColor = Color.white;
    [SerializeField] private Color shadowColor = new Color(0.2549f, 0.4f, 0.7765f, 1f); // #4166C6
    [SerializeField] private Color disabledColor = new Color(0.898f, 0.898f, 0.898f, 1f); // #E5E5E5
    [SerializeField] private Color disabledIconColor = new Color(0.6f, 0.6f, 0.6f, 1f); // #999999
    [SerializeField] private Color iconColor = Color.white;
    [SerializeField] private float shadowOffset = 15f;


    private Image buttonBackground;
    private Image shadowImage;
    private Image iconImage;
    private RectTransform backgroundRect;
    private RectTransform shadowRect;
    private RectTransform iconRect;
    private Vector2 originalBackgroundPosition;
    private Vector2 originalIconPosition;
    private Color originalIconColor;
    private Color normalBackgroundColor; // 记录正常状态的背景色
    private Color normalIconColor; // 记录正常状态的图标色

    protected override void Awake()
    {
        base.Awake();
        InitializeComponents();
        SetupButton();
        //ResetPosition();
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
            originalIconColor = iconImage.color;
        }

        var defaultImage = GetComponent<Image>();
        if (defaultImage) defaultImage.enabled = false;

        targetGraphic = buttonBackground;
    }

    private void SetupButton()
    {
        normalBackgroundColor = normalColor;
        normalIconColor = iconColor;

        if (buttonBackground)
        {
            buttonBackground.sprite = backgroundSprite;
            buttonBackground.color = normalColor;
            buttonBackground.type = Image.Type.Sliced;
            originalBackgroundPosition = backgroundRect.anchoredPosition;
        }

        if (shadowImage)
        {
            shadowImage.sprite = shadowSprite;
            shadowImage.color = shadowColor;
            shadowImage.type = Image.Type.Sliced;
            UpdateShadowPosition();
        }

        if (iconRect)
        {
            iconImage.sprite = iconSprite;
            iconImage.color = iconColor;
            originalIconPosition = iconRect.anchoredPosition;
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
            shadowImage.color = IsInteractable() ? shadowColor : disabledIconColor; ;
        }

        if (iconImage)
        {
            iconImage.sprite = iconSprite;
            normalIconColor = iconColor;
            iconImage.color = IsInteractable() ? normalIconColor : disabledIconColor;
        }
    }


    public void UpdateShadowPosition()
    {
        if (shadowRect)
        {
            shadowRect.anchoredPosition = new Vector2(0, -shadowOffset);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable() || !IsActive())
            return;

        base.OnPointerDown(eventData);

        if (backgroundRect)
        {
            Vector2 pressedPosition = originalBackgroundPosition + new Vector2(0, -shadowOffset);
            backgroundRect.anchoredPosition = pressedPosition;

            if (iconRect != null)
            {
                iconRect.anchoredPosition = originalIconPosition + new Vector2(0, -shadowOffset);
            }

            if (enablePressedColor)
            {
                buttonBackground.color = pressedColor;
            }

            if (enablePressedIconColor && iconImage)
            {
                iconImage.color = pressedIconColor;
            }

            //shadowImage.gameObject.SetActive(false);
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
            backgroundRect.anchoredPosition = originalBackgroundPosition;
            if (iconRect != null)
            {
                iconRect.anchoredPosition = originalIconPosition;
            }

            // 根据当前状态设置正确的颜色
            if (IsInteractable())
            {
                buttonBackground.color = normalBackgroundColor;
                
                if (iconImage)
                    iconImage.color = normalIconColor;
                
                if (shadowImage)
                    shadowImage.color = shadowColor;
                
                //if (shadowImage) shadowImage.gameObject.SetActive(true);
            }
            else
            {
                buttonBackground.color = disabledColor;
                
                if (iconImage) 
                    iconImage.color = disabledIconColor;
                
                if (shadowImage)
                    shadowImage.color = disabledIconColor;
                
                //if (shadowImage) shadowImage.gameObject.SetActive(false);
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
                if (shadowImage)
                    shadowImage.color = shadowColor;
                //if (shadowImage) shadowImage.gameObject.SetActive(true);
                break;

            case SelectionState.Disabled:
                if (buttonBackground) buttonBackground.color = disabledColor;
                if (iconImage) iconImage.color = disabledIconColor;
                if (shadowImage)
                    shadowImage.color = disabledIconColor;
                //if (shadowImage) shadowImage.gameObject.SetActive(false);
                
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
