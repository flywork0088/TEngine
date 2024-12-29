using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace HsCore.UI
{
    public class HsOutlineButton : Button
    {
        [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private Sprite shadowSprite;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private bool enablePressedColor = false;
        [SerializeField] private Color pressedColor = new Color(0.18f, 0.737f, 0.067f);  // #2EBC11
        [SerializeField] private bool enablePressedTextColor = false;
        [SerializeField] private Color pressedTextColor = Color.white;
        [SerializeField] private Color disabledColor = new Color(0.898f, 0.898f, 0.898f);  // #E5E5E5
        [SerializeField] private Color disabledTextColor = new Color(0.663f, 0.663f, 0.663f);  // #A9A9A9
        [SerializeField] private Color outlineColor = new Color(0.6667f, 0.7176f, 0.8157f, 1f); // #AAB7D0 边框颜色

        [SerializeField] private float outlineThickness = 0f; // 边框厚度
        [SerializeField] private float shadowOffset = 12f;

        private Image buttonBackground;
        private Image shadowImage;
        private TextMeshProUGUI buttonText;
        private RectTransform backgroundRect;
        private RectTransform shadowRect;
        private RectTransform textRect;
        private Vector2 originalBackgroundPosition;
        private Vector2 originalTextPosition;
        private Color originalTextColor;
        private Color normalBackgroundColor;

        protected override void Awake()
        {
            base.Awake();
            InitializeComponents();
            SetupButton();
        }

        private void InitializeComponents()
        {
            normalBackgroundColor = normalColor;
            buttonBackground = transform.Find("Background")?.GetComponent<Image>();
            shadowImage = transform.Find("Shadow")?.GetComponent<Image>();
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

            if (buttonBackground) backgroundRect = buttonBackground.GetComponent<RectTransform>();
            if (shadowImage) shadowRect = shadowImage.GetComponent<RectTransform>();
            if (buttonText)
            {
                textRect = buttonText.GetComponent<RectTransform>();
                
                originalTextColor = buttonText.color;
            }

            var defaultImage = GetComponent<Image>();
            if (defaultImage) defaultImage.enabled = false;

            targetGraphic = buttonBackground;
        }

        private void SetupButton()
        {
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
                shadowImage.color = outlineColor; // 使用 outlineColor
                shadowImage.type = Image.Type.Sliced;
                SetupOutline(false);
            }

            if (textRect)
            {
                originalTextPosition = textRect.anchoredPosition;
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

                // 禁用状态隐藏阴影
                shadowImage.gameObject.SetActive(IsInteractable());
                SetupOutline(false);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable())
                return;

            base.OnPointerDown(eventData);

            if (backgroundRect)
            {
                backgroundRect.anchoredPosition = new Vector2(0, -shadowOffset);

                if (textRect != null)
                {
                    textRect.anchoredPosition = new Vector2(0, -shadowOffset);
                }

                SetupOutline(true); // 调用按下状态

                if (enablePressedColor)
                {
                    buttonBackground.color = pressedColor;
                }
                if (enablePressedTextColor && buttonText)
                {
                    buttonText.color = pressedTextColor;
                }

                // 点击时强制显示阴影
                if (shadowImage) shadowImage.gameObject.SetActive(true);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!IsInteractable())
                return;

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

                if (textRect != null)
                {
                    textRect.anchoredPosition = Vector2.zero;
                }

                SetupOutline(false); // 恢复正常状态

                if (IsInteractable())
                {
                    buttonBackground.color = normalBackgroundColor;
                    if (buttonText) 
                        buttonText.color = originalTextColor;
                    if (shadowImage) shadowImage.gameObject.SetActive(true);
                }
                else
                {
                    buttonBackground.color = disabledColor;
                    if (buttonText) buttonText.color = disabledTextColor;
                    if (shadowImage) shadowImage.gameObject.SetActive(false); // 禁用状态隐藏阴影
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
                    if (buttonText && Application.isPlaying) 
                        buttonText.color = originalTextColor;
                    if (shadowImage) shadowImage.gameObject.SetActive(true);
                    SetupOutline(false);
                    break;

                case SelectionState.Disabled:
                    if (buttonBackground) buttonBackground.color = disabledColor;
                    if (buttonText) 
                        buttonText.color = disabledTextColor;
                    if (shadowImage) shadowImage.gameObject.SetActive(false); // 禁用状态隐藏阴影
                    break;
            }
        }
        
    }
}
