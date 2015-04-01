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

        private UILabelledBox showGridBox;
        private UILabelledBox showFogBox;
        private UILabelledBox showBuildingsBox;
        private UILabelledBox showZonesBox;
        private UILabelledBox clampNodeBox;
        private UIDoubleButton segmentButtons;

        #region "Component access"
        public UISliderInput GridSizeSlider
        {
            get { return gridSizeSlider; }
        }

        public UISliderInput GridAngleSlider
        {
            get { return gridAngleSlider; }
        }

        public UILabelledBox ShowGridBox
        {
            get { return showGridBox; }
        }

        public UILabelledBox ShowFogBox
        {
            get { return showFogBox; }
        }

        public UILabelledBox ShowBuildingsBox
        {
            get { return showBuildingsBox; }
        }

        public UILabelledBox ShowZonesBox
                {
                    get { return showZonesBox; }
                }
        public UILabelledBox ClampNodeBox
        {
            get { return clampNodeBox; }
        }

        public UIDoubleButton SegmentButtons
        {
            get { return segmentButtons; }
        }
        #endregion

        public override void Awake()
        {
            base.Awake();

            titlePanel = AddUIComponent<UITitlePanel>();
            gridSizeSlider = AddUIComponent<UISliderInput>();
            gridAngleSlider = AddUIComponent<UISliderInput>();
            showGridBox = AddUIComponent<UILabelledBox>();
            showZonesBox = AddUIComponent<UILabelledBox>();
            showFogBox = AddUIComponent<UILabelledBox>();
            showBuildingsBox = AddUIComponent<UILabelledBox>();
            clampNodeBox = AddUIComponent<UILabelledBox>();
            segmentButtons = AddUIComponent<UIDoubleButton>();

            #region "Bidirectional Binding"
            gridSizeSlider.Slider.eventValueChanged += delegate(UIComponent sender, float value)
            {
                GridRenderManager.GridSize = value;
            };

            gridAngleSlider.Slider.eventValueChanged += delegate(UIComponent sender, float value)
            {
                GridRenderManager.Rotation = Quaternion.AngleAxis(value, new Vector3(0, 1, 0));
            };

            showGridBox.CheckBox.eventCheckChanged += delegate(UIComponent sender, bool value)
            {
                GridRenderManager.RenderGrid = value;
            };

            showZonesBox.CheckBox.eventCheckChanged += delegate(UIComponent sender, bool value)
            {
                TerrainManager.instance.RenderZones = value;
            };

            showFogBox.CheckBox.eventCheckChanged += delegate(UIComponent sender, bool value)
            {
                Utils.SetFogRender(value);
                GridTrigger.RenderFog = value;
            };

            showBuildingsBox.CheckBox.eventCheckChanged += delegate(UIComponent sender, bool value)
            {
                Utils.SetBuildingRender(value);
                GridTrigger.RenderBuildings = value;
            };

            segmentButtons.LeftButton.eventClick += delegate(UIComponent component, UIMouseEventParameter eventParam)
            {
                if (GridRenderManager.IsClamped)
                {
                    NetNode node = GridRenderManager.ClampedNode;
                    int segmentCount = node.CountSegments();
                    GridRenderManager.ClampedSegment++;
                    if (GridRenderManager.ClampedSegment >= segmentCount)
                    {
                        GridRenderManager.ClampedSegment = 0;
                    }

                    NetSegment segment = NetManager.instance.m_segments.m_buffer[node.GetSegment(GridRenderManager.ClampedSegment)];
                    GridRenderManager.Rotation = Utils.GetRotationMapBetweenVecs(new Vector3(1, 0, 0), segment.m_startDirection);
                }

            };

            segmentButtons.RightButton.eventClick += delegate(UIComponent component, UIMouseEventParameter eventParam)
            {
                if (GridRenderManager.IsClamped)
                {
                    NetNode node = GridRenderManager.ClampedNode;
                    int segmentCount = node.CountSegments();
                    GridRenderManager.ClampedSegment--;
                    if (GridRenderManager.ClampedSegment < 0)
                    {
                        GridRenderManager.ClampedSegment = segmentCount-1;
                    }

                    NetSegment segment = NetManager.instance.m_segments.m_buffer[node.GetSegment(GridRenderManager.ClampedSegment)];
                    GridRenderManager.Rotation = Utils.GetRotationMapBetweenVecs(new Vector3(1, 0, 0), segment.m_startDirection);
                }
                    
            };
            #endregion

        }
        public override void Start()
        {
            // Set visuals for panel
            this.backgroundSprite = "MenuPanel2";
            this.width = 450;
            this.height = 350;
            this.transformPosition = new Vector3(-1.3f, 0.9f);

            SetupControls();
        }

        private void SetupControls()
        {

            #region "Top Bar"
            titlePanel.Parent = this;
            titlePanel.relativePosition = Vector3.zero;
            titlePanel.IconSprite = "ToolbarIconRoads";
            titlePanel.TitleText = "Road Assist";
            #endregion

            #region "Sliders"
            gridSizeSlider.Parent = this;
            gridSizeSlider.relativePosition = new Vector3(0,50);
            gridSizeSlider.MinValue = 0f;
            gridSizeSlider.MaxValue = 2000f;
            gridSizeSlider.StepSize = 10f;
            gridSizeSlider.LabelText = "Grid Size";
            gridSizeSlider.SliderValue = 1000f;
 
            gridAngleSlider.Parent = this;
            gridAngleSlider.relativePosition = new Vector3(0, 100);
            gridAngleSlider.MinValue = 0f;
            gridAngleSlider.MaxValue = 360f;
            gridAngleSlider.SliderValue = 0f;
            gridAngleSlider.LabelText = "Grid Angle";
            #endregion

            #region "CheckBoxes"
            showGridBox.Parent = this;
            showGridBox.relativePosition = new Vector3(0, 150);
            showGridBox.LabelText = "Show Grid:";
 
            showZonesBox.Parent = this;
            showZonesBox.relativePosition = new Vector3(210, 150);
            showZonesBox.LabelText = "Show Zones:";
       
            showFogBox.Parent = this;
            showFogBox.relativePosition = new Vector3(0, 200);
            showFogBox.LabelText = "Show Fog:";
            showFogBox.CheckBox.isChecked = true;

            showBuildingsBox.Parent = this;
            showBuildingsBox.relativePosition = new Vector3(210, 200);
            showBuildingsBox.LabelText = "Show Buildings:";
            showBuildingsBox.CheckBox.isChecked = true;

            clampNodeBox.Parent = this;
            clampNodeBox.relativePosition = new Vector3(0, 250);
            clampNodeBox.LabelText = "Node Clamped:";
            clampNodeBox.CheckBox.readOnly = true;
            #endregion

            #region "Segment buttons"
            segmentButtons.Parent = this;
            segmentButtons.relativePosition = new Vector3(0,300);
            segmentButtons.LabelText = "Snap to segments:";
            #endregion


        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();

        }

        protected override void OnResolutionChanged(Vector2 previousResolution, Vector2 currentResolution)
        {
            base.OnResolutionChanged(previousResolution, currentResolution);
        }
    }
}
