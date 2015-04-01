using System;
using System.Collections.Generic;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace RoadAssist
{
    public class UIDoubleButton : UIPanel
    {
        private UILabel label;
        private UIButton leftButton;
        private UIButton rightButton;

        public string LabelText
        {
            get { return label.text; }
            set { label.text = value; }
        }

        public UIButton LeftButton
        {
            get { return leftButton; }
        }

        public UIButton RightButton
        {
            get { return rightButton; }
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
            leftButton = AddUIComponent<UIButton>();
            rightButton = AddUIComponent<UIButton>();

            height = 40;
            width = 450;

            LabelText = "(None)";

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

            leftButton.text = "<";
            leftButton.size = new Vector2(20, 20);
            leftButton.relativePosition = new Vector3(170,0);

            leftButton.normalBgSprite = "ButtonMenu";
            leftButton.disabledBgSprite = "ButtonMenuDisabled";
            leftButton.hoveredBgSprite = "ButtonMenuHovered";
            leftButton.focusedBgSprite = "ButtonMenuFocused";
            leftButton.pressedBgSprite = "ButtonMenuPressed";
            
            leftButton.textColor = new Color32(255, 255, 255, 255);
            leftButton.disabledTextColor = new Color32(7, 7, 7, 255);
            leftButton.hoveredTextColor = new Color32(7, 132, 255, 255);
            leftButton.focusedTextColor = new Color32(255, 255, 255, 255);
            leftButton.pressedTextColor = new Color32(30, 30, 44, 255);

            rightButton.text = ">";
            rightButton.size = new Vector2(20, 20);
            rightButton.relativePosition = new Vector3(200, 0);

            rightButton.normalBgSprite = "ButtonMenu";
            rightButton.disabledBgSprite = "ButtonMenuDisabled";
            rightButton.hoveredBgSprite = "ButtonMenuHovered";
            rightButton.focusedBgSprite = "ButtonMenuFocused";
            rightButton.pressedBgSprite = "ButtonMenuPressed";

            rightButton.textColor = new Color32(255, 255, 255, 255);
            rightButton.disabledTextColor = new Color32(7, 7, 7, 255);
            rightButton.hoveredTextColor = new Color32(7, 132, 255, 255);
            rightButton.focusedTextColor = new Color32(255, 255, 255, 255);
            rightButton.pressedTextColor = new Color32(30, 30, 44, 255);

            leftButton.Disable();
            rightButton.Disable();
        }
    }
}
