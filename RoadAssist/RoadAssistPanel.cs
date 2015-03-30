using System;
using System.Collections.Generic;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace RoadAssist
{
    public class RoadAssistPanel : UIPanel
    {
        private UITitlePanel titlePanel;

        private UISliderInput sliderInput;

        public override void Start()
        {
            // Set visuals for panel
            this.backgroundSprite = "MenuPanel2";
            this.width = 450;
            this.height = 350;
            this.transformPosition = new Vector3(-1.0f, 0.9f);

            SetupControls();
        }

        private void SetupControls()
        {

            #region "Top Bar"
            titlePanel = AddUIComponent<UITitlePanel>();
            titlePanel.Parent = this;

            titlePanel.IconSprite = "ToolbarIconRoads";
            titlePanel.TitleText = "Road Assist";
            #endregion

            #region "Sliders"
            sliderInput = AddUIComponent<UISliderInput>();
            sliderInput.Parent = this;
            sliderInput.relativePosition = new Vector3(5,100);

            #endregion




        }

        protected override void OnResolutionChanged(Vector2 previousResolution, Vector2 currentResolution)
        {
            base.OnResolutionChanged(previousResolution, currentResolution);
        }

        #region "Button installation"
        private UIButton AddNewButton(string buttonText)
        {
            UIButton newButton = this.AddUIComponent<UIButton>();

            SetDefaultButton(newButton, buttonText);

            return newButton;
        }

        private void SetDefaultButton(UIButton button, string buttonText)
        {
            button.text = buttonText;
            button.width = this.width - this.autoLayoutPadding.left * 2;
            button.height = 25;

            button.normalBgSprite = "ButtonMenu";
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.focusedBgSprite = "ButtonMenuFocused";
            button.pressedBgSprite = "ButtonMenuPressed";
            button.textColor = new Color32(255, 255, 255, 255);
            button.disabledTextColor = new Color32(7, 7, 7, 255);
            button.hoveredTextColor = new Color32(7, 132, 255, 255);
            button.focusedTextColor = new Color32(255, 255, 255, 255);
            button.pressedTextColor = new Color32(30, 30, 44, 255);
        }
        #endregion

        #region "Button Clicks"
        private void LabelClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (eventParam.buttons == UIMouseButton.Left && ChirpPanel.instance != null)
            {
                this.Hide();
            }
        }

        #endregion

    }
}
