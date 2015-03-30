using System;
using System.Collections.Generic;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace RoadAssist
{
    class UISliderInput : UIPanel
    {
        private UISlider slider;
        private UITextField textField;

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

            slider = AddUIComponent<UISlider>();
            textField = AddUIComponent<UITextField>();

            height = 40;
            width = 450;
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
            relativePosition = Vector3.zero;
            isVisible = true;
            canFocus = true;
            isInteractive = true;

            slider.width = width - 150;
            slider.relativePosition = Vector3.zero;
            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.value = 50f;
            

            textField.width = 100;
            slider.relativePosition = new Vector3(320,0);
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
