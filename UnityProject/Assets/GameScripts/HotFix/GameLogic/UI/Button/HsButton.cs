using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace HsCore.UI
{
    public class HsButton : Button
    {
        [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private Sprite shadowSprite;
        [SerializeField] private Color normalColor = new Color(0.18f, 0.737f, 0.067f, 1f);  // #2EBC11
        [SerializeField] private bool enablePressedColor = false;
        [SerializeField] private Color pressedColor = new Color(0.18f, 0.737f, 0.067f, 1f);  // #2EBC11
        [SerializeField] private bool enablePressedTextColor = false;
        [SerializeField] private Color pressedTextColor = Color.white;
        [SerializeField] private Color shadowColor = new Color(0.059f, 0.567f, 0.035f, 1f);  // #0F9109
        [SerializeField] private Color disabledColor = new Color(0.898f, 0.898f, 0.898f, 1f);  // #E5E5E5
        [SerializeField] private Color disabledTextColor = new Color(0.663f, 0.663f, 0.663f, 1f);  // #A9A9A9
        [SerializeField] private float shadowOffset = 15f;

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
        
        // protected override void Start()
        // {
        //     base.Start();
        //     InitializeComponents();
        //     SetupButton();
        // }
        protected override void Awake()
        {
            base.Awake();
            InitializeComponents();
            SetupButton();
        }
        
        public Sprite Background
        {
            set
            {
                backgroundSprite = value;
                UpdateButton();
            }
            get
            {
                return backgroundSprite;
            }
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
                shadowImage.color = shadowColor;
                shadowImage.type = Image.Type.Sliced;
                UpdateShadowPosition();
            }

            if (textRect)
            {
                originalTextPosition = textRect.anchoredPosition;
            }
        }
        
        // 更新按钮时同时更新记录的正常颜色
        public void UpdateButton()
        {
            if (buttonBackground)
            {
                buttonBackground.sprite = backgroundSprite;
                buttonBackground.type = Image.Type.Sliced;
                normalBackgroundColor = normalColor;  // 更新记录的正常颜色
                buttonBackground.color = IsInteractable() ? normalBackgroundColor : disabledColor;
            }

            if (shadowImage)
            {
                shadowImage.sprite = shadowSprite;
                shadowImage.type = Image.Type.Sliced;
                shadowImage.color = IsInteractable() ? shadowColor : disabledTextColor; 
            }
        }

        public void SetBackgroundColor(Color32 color)
        {
            normalColor = color;
            normalBackgroundColor = color;
            buttonBackground.color = normalBackgroundColor;
        }

        public void SetShadowColor(Color32 color)
        {
            shadowColor = color;
            shadowImage.color = shadowColor;
        }

        public void SetBackgroundAndShadowColor(Color32 color, Color32 shadowColor)
        {
            SetBackgroundColor(color);
            SetShadowColor(shadowColor);
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
            if (!IsInteractable()) 
                return;
            
            base.OnPointerDown(eventData);
            
            if (backgroundRect)
            {
                Vector2 pressedPosition = originalBackgroundPosition + new Vector2(0, -shadowOffset);
                backgroundRect.anchoredPosition = pressedPosition;

                if (textRect != null)
                {
                    textRect.anchoredPosition = originalTextPosition + new Vector2(0, -shadowOffset);
                }

                if (enablePressedColor)
                {
                    buttonBackground.color = pressedColor;
                }
                if (enablePressedTextColor && buttonText)
                {
                    buttonText.color = pressedTextColor;
                }
                
                //shadowImage.gameObject.SetActive(false);
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
                backgroundRect.anchoredPosition = originalBackgroundPosition;
                if (textRect != null)
                {
                    textRect.anchoredPosition = originalTextPosition;
                }

                // 根据当前状态设置正确的颜色
                if (IsInteractable())
                {
                    buttonBackground.color = normalBackgroundColor;
                    if (buttonText) buttonText.color = originalTextColor;
                    //if (shadowImage) shadowImage.gameObject.SetActive(true);
                }
                else
                {
                    buttonBackground.color = disabledColor;
                    if (buttonText) buttonText.color = disabledTextColor;
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
                    if (buttonText && Application.isPlaying) 
                        buttonText.color = originalTextColor;
                    
                    if (shadowImage)
                        shadowImage.color = shadowColor;
                    
                    //if (shadowImage) shadowImage.gameObject.SetActive(true);
                    break;

                case SelectionState.Disabled:
                    if (buttonBackground) buttonBackground.color = disabledColor;
                    if (buttonText) buttonText.color = disabledTextColor;
                    
                    if (shadowImage)
                        shadowImage.color = disabledTextColor;
                    
                    //if (shadowImage) shadowImage.gameObject.SetActive(false);
                    break;
            }
        }
        public void UpdateOriginalTextPosition()
        {
            originalTextPosition = textRect.anchoredPosition; 
        }
    }
}
