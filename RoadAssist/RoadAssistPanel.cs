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

        private UISliderInput gridSizeSlider;
        private UISliderInput gridAngleSlider;

        public override void Start()
        {
            // Set visuals for panel
            this.backgroundSprite = "MenuPanel2";
            this.width = 450;
            this.height = 350;
            this.transformPosition = new Vector3(-1.0f, 0.9f);

            
            // Allow automated layout
            this.autoLayoutDirection = LayoutDirection.Vertical;
            this.autoLayoutStart = LayoutStart.TopLeft;
            this.autoLayoutPadding = new RectOffset(0, 0, 10, 10);
            //this.autoLayout = true;

            SetupControls();
        }

        private void SetupControls()
        {

            #region "Top Bar"
            titlePanel = AddUIComponent<UITitlePanel>();
            titlePanel.Parent = this;
            titlePanel.relativePosition = Vector3.zero;
            titlePanel.IconSprite = "ToolbarIconRoads";
            titlePanel.TitleText = "Road Assist";
            #endregion

            #region "Sliders"
            gridSizeSlider = AddUIComponent<UISliderInput>();
            gridSizeSlider.Parent = this;
            gridSizeSlider.relativePosition = new Vector3(0,50);
            gridSizeSlider.MinValue = 0f;
            gridSizeSlider.MaxValue = 100f;
            gridSizeSlider.LabelText = "Grid Size";

            gridAngleSlider = AddUIComponent<UISliderInput>();
            gridAngleSlider.Parent = this;
            gridAngleSlider.relativePosition = new Vector3(0, 100);
            gridAngleSlider.MinValue = 0f;
            gridAngleSlider.MaxValue = 360f;
            gridAngleSlider.LabelText = "Grid Angle";
            

            #endregion

        }

        protected override void OnResolutionChanged(Vector2 previousResolution, Vector2 currentResolution)
        {
            base.OnResolutionChanged(previousResolution, currentResolution);
        }
    }
}
