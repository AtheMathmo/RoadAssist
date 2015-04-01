using System;
using System.Collections.Generic;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace RoadAssist
{
    /// <summary>
    /// UIComponent inheriting from UIPanel. Creates slider with label and text field. Requires the following to be set:
    /// 
    /// LabelText - Text to display above slider.
    /// MinValue - Min value of slider.
    /// MaxValue - Max value of slider.
    /// </summary>
    public class UISliderInput : UIPanel
    {
        private UILabel label;
        private UISlider slider;
        private UITextField textField;



        public string LabelText
        {
            get { return label.text; }
            set { label.text = value; }
        }

        public UISlider Slider
        {
            get { return slider; }
        }

        public float MinValue
        {
            get { return slider.minValue; }
            set { slider.minValue = value; }
        }

        public float MaxValue
        {
            get { return slider.maxValue; }
            set { slider.maxValue = value; }
        }

        public float SliderValue
        {
            get { return slider.value; }
            set { slider.value = value; }
        }

        public float StepSize
        {
            get { return slider.stepSize; }
            set { slider.stepSize = value; }
        }

        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        public UIPanel Parent { get; set; }

        public override void Awake()
        {
            base.Awake();

            label = AddUIComponent<UILabel>();
            slider = AddUIComponent<UISlider>();
            textField = AddUIComponent<UITextField>();

            LabelText = "(None)";

            height = 40;
            width = 450;

            this.slider.eventValueChanged += delegate(UIComponent sender, float value)
            {
                //value = slider.value;
                textField.text = Mathf.CeilToInt(value).ToString();
            };

            // Check if the text field changed, and update the slider value.
            this.textField.eventTextSubmitted += delegate(UIComponent sender, string s)
            {
                int num;
                if (int.TryParse(s, out num))
                {
                    if (num > slider.maxValue)
                    {
                        textField.text = slider.maxValue.ToString();
                    }
                    else if (num < slider.minValue)
                    {
                        textField.text = slider.minValue.ToString();
                    }
                    slider.value = num;
                }
                else
                {
                    textField.text = slider.value.ToString();
                }
            };
        }
        public override void Start()
        {
            base.Start();

            if (Parent == null)
            {
                UnityEngine.Debug.Log(String.Format("Parent not set in {0}", this.GetType().Name));
                return;
            }

            width = Parent.width;
            isVisible = true;
            canFocus = true;
            isInteractive = true;

            label.relativePosition = new Vector3(10, 0);
            label.textScale = 1.0f;
            label.text = LabelText;

            slider.width = width - 150;
            slider.height = 15;
            slider.backgroundSprite = "BudgetSlider";
            slider.relativePosition = new Vector3(10,20);
            slider.minValue = MinValue;
            slider.maxValue = MaxValue;
            slider.value = SliderValue;
            
            UISlicedSprite tracSprite = slider.AddUIComponent<UISlicedSprite>();
            tracSprite.relativePosition = Vector2.zero;
            tracSprite.autoSize = true;
            tracSprite.size = tracSprite.parent.size;
            tracSprite.fillDirection = UIFillDirection.Horizontal;
            tracSprite.spriteName = "";

            UISlicedSprite thumbSprite = tracSprite.AddUIComponent<UISlicedSprite>();
            thumbSprite.relativePosition = Vector2.zero;
            thumbSprite.fillDirection = UIFillDirection.Vertical;
            thumbSprite.autoSize = true;
            thumbSprite.spriteName = "SliderBudget";

            slider.thumbObject = thumbSprite;
            slider.fillIndicatorObject = tracSprite;            

            textField.width = 100;
            textField.relativePosition = new Vector3(320, 20);
            textField.normalBgSprite = "TextFieldPanel";
            textField.hoveredBgSprite = "TextFieldPanelHovered";
            textField.focusedBgSprite = "TextFieldUnderline";
            textField.text = slider.value.ToString("0.");
            textField.isInteractive = true;
            textField.enabled = true;
            textField.readOnly = false;
            textField.builtinKeyNavigation = true; 


        }
    }
}
