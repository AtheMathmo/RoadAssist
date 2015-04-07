using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ColossalFramework.IO;
using ColossalFramework.Math;
using ColossalFramework.Steamworks;
using ColossalFramework.Threading;
using ICities;
using UnityEngine;

namespace RoadAssist
{
    public class GridTrigger : ThreadingExtensionBase
    {
        private float angleRot = 0f;
        private float moveSpeed = 1f;

        private static Mesh boxMesh;

        private static bool renderBuildings = true;
        private static bool renderFog = true;

        #region "GetSets"
        public static bool RenderBuildings
        {
            get { return renderBuildings; }
            set { renderBuildings = value; }
        }

        public static bool RenderFog
        {
            get { return renderFog; }
            set { renderFog = value; }
        }

        public static Mesh BoxMesh
        {
            get { return boxMesh; }
            set { boxMesh = value; }
        }
        #endregion

        public override void OnCreated(IThreading threading)
        {
            boxMesh = CustomOverlayEffect.CreateBoxMesh();            
        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            #region "Toggles"
            // J for panel
            if (Event.current.alt && Input.GetKeyDown(KeyCode.J))
            {
                RoadAssistPanel panel = GameObject.FindObjectOfType<RoadAssistPanel>();
                panel.isVisible = !panel.isVisible;
            }

            // H to remove buildings
            if (Event.current.alt && Input.GetKeyDown(KeyCode.H))
            {
                renderBuildings = !renderBuildings;
                Utils.SetBuildingRender(renderBuildings);
                RoadAssistPanel panel = GameObject.FindObjectOfType<RoadAssistPanel>();
                panel.ShowBuildingsBox.CheckBox.isChecked = renderBuildings;
            }

            if (Event.current.alt && Input.GetKeyDown(KeyCode.F))
            {
                renderFog = !renderFog;
                Utils.SetFogRender(renderFog);
                RoadAssistPanel panel = GameObject.FindObjectOfType<RoadAssistPanel>();
                panel.ShowFogBox.CheckBox.isChecked = renderFog;

            }

            // Z for zones
            if (Event.current.alt && Input.GetKeyDown(KeyCode.Z))
            {
                TerrainManager.instance.RenderZones = !TerrainManager.instance.RenderZones;
                RoadAssistPanel panel = GameObject.FindObjectOfType<RoadAssistPanel>();
                panel.ShowZonesBox.CheckBox.isChecked = TerrainManager.instance.RenderZones;
            }

            // G for grid
            if (Event.current.alt && Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log("[RoadAssist] Alt+G pressed. Render is " + (!GridRenderManager.RenderGrid ? "on" : "off"));
                GridRenderManager.RenderGrid = !GridRenderManager.RenderGrid;
                RoadAssistPanel panel = GameObject.FindObjectOfType<RoadAssistPanel>();
                panel.ShowGridBox.CheckBox.isChecked = GridRenderManager.RenderGrid;
            }
            #endregion

            #region "RayCast"
            if (Input.GetMouseButtonDown(0) && !ToolsModifierControl.toolController.IsInsideUI)
            {                
                // Reflection to get RayCast method from ToolBase.
                MethodInfo rayCast = Utils.GetPrivateMethod(typeof(ToolBase), "RayCast");

                // Gets mouse ray
                Vector3 mousePosition = Input.mousePosition;
                Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);
                float mouseRayLength = Camera.main.farClipPlane;

                // Sets parameters for the RayCast
                ToolBase.RaycastInput input = new ToolBase.RaycastInput(mouseRay, mouseRayLength);
                input.m_ignoreTerrain = false;
                input.m_ignoreNodeFlags = NetNode.Flags.None;

                // Invoking method.
                object[] parameters = new object[] {input, null};
                object result = rayCast.Invoke(null, parameters);


                if ((bool) result)
                {
                    ToolBase.RaycastOutput output = (ToolBase.RaycastOutput) parameters[1];

                    // Get patch inside click.
                    TerrainPatch patch;
                    if (GridRenderManager.gridMethod.Equals(GridRenderManager.GridMethod.WholePatch) && Utils.FindPatchByPoint(output.m_hitPos, out patch))
                    {
                        GridRenderManager.ClampedPatch = patch;
                    }

                    // Get node that was clicked
                    if (GridRenderManager.gridMethod.Equals(GridRenderManager.GridMethod.ManualLines) && (int)output.m_netNode != 0)
                    {
                        NetNode node = NetManager.instance.m_nodes.m_buffer[(int)output.m_netNode];
                        GridRenderManager.ClampedNode = node;
                        GridRenderManager.IsNodeClamped = true;
                        GridRenderManager.GridCenter = node.m_position;

                        RoadAssistPanel panel = GameObject.FindObjectOfType<RoadAssistPanel>();
                        panel.ClampNodeBox.CheckBox.isChecked = true;
                        panel.SegmentButtons.LeftButton.Enable();
                        panel.SegmentButtons.RightButton.Enable();
                    }

                }
            }
            #endregion

            #region "Rotate Grid"
            if (Event.current.alt && Input.GetKey(KeyCode.RightArrow))
            {
                angleRot = (angleRot + 1f) % 360;

                RoadAssistPanel panel = GameObject.FindObjectOfType<RoadAssistPanel>();
                panel.GridAngleSlider.Slider.value = angleRot;

                GridRenderManager.Rotation = Quaternion.AngleAxis(angleRot, new Vector3(0, 1, 0));
                
            }

            if (Event.current.alt && Input.GetKey(KeyCode.LeftArrow))
            {
                angleRot = (angleRot - 1f) % 360;

                if (angleRot < 0)
                {
                    angleRot = 360f + angleRot;
                }

                RoadAssistPanel panel = GameObject.FindObjectOfType<RoadAssistPanel>();
                panel.GridAngleSlider.Slider.value = angleRot;

                GridRenderManager.Rotation = Quaternion.AngleAxis(angleRot, new Vector3(0, 1, 0));
            }
            #endregion

            #region"Snap to segment"
            if (GridRenderManager.IsNodeClamped)
            {
                NetNode node = GridRenderManager.ClampedNode;
                int segmentCount = node.CountSegments();
                if (Input.GetKeyDown(KeyCode.Period))
                {
                    GridRenderManager.ClampedSegment++;
                    if (GridRenderManager.ClampedSegment >= segmentCount)
                    {
                        GridRenderManager.ClampedSegment = 0;
                    }

                    NetSegment segment = NetManager.instance.m_segments.m_buffer[node.GetSegment(GridRenderManager.ClampedSegment)];
                    GridRenderManager.Rotation = Utils.GetRotationMapBetweenVecs(new Vector3(1, 0, 0), segment.m_startDirection, out angleRot);

                    //RoadAssistPanel panel = GameObject.FindObjectOfType<RoadAssistPanel>();
                    //panel.GridAngleSlider.Slider.value = angleRot;
                }

                if (Input.GetKeyDown(KeyCode.Comma))
                {
                    GridRenderManager.ClampedSegment--;
                    if (GridRenderManager.ClampedSegment < 0)
                    {
                        GridRenderManager.ClampedSegment = segmentCount-1;
                    }

                    NetSegment segment = NetManager.instance.m_segments.m_buffer[node.GetSegment(GridRenderManager.ClampedSegment)];
                    GridRenderManager.Rotation = Utils.GetRotationMapBetweenVecs(new Vector3(1, 0, 0), segment.m_startDirection, out angleRot);

                    //RoadAssistPanel panel = GameObject.FindObjectOfType<RoadAssistPanel>();
                    //panel.GridAngleSlider.Slider.value = angleRot; 
                }
            }
            #endregion

            #region "Resize Grid"
            if (Event.current.alt && Input.GetKey(KeyCode.UpArrow))
            {
                GridRenderManager.GridSize += 10f;

                if (GridRenderManager.GridSize > 2000f)
                {
                    GridRenderManager.GridSize = 2000f;
                }


                RoadAssistPanel panel = GameObject.FindObjectOfType<RoadAssistPanel>();

                panel.GridSizeSlider.Slider.value = GridRenderManager.GridSize;
            }

            if (Event.current.alt && Input.GetKey(KeyCode.DownArrow))
            {
                GridRenderManager.GridSize -= 10f;
                if (GridRenderManager.GridSize <= 0)
                {
                    GridRenderManager.GridSize = 0;
                }

                RoadAssistPanel panel = GameObject.FindObjectOfType<RoadAssistPanel>();

                panel.GridSizeSlider.Slider.value = GridRenderManager.GridSize;

            }
            #endregion

            #region"Move Grid"
            
            if (!Event.current.alt && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)))
            {
                // Accelerate grid
                if (moveSpeed <= 20f)
                {
                    moveSpeed += 0.1f;
                }

                // Remove clamp - notify panel.
                if (GridRenderManager.IsNodeClamped)
                {
                    GridRenderManager.IsNodeClamped = false;
                    GridRenderManager.ClampedSegment = 0;
                    RoadAssistPanel panel = GameObject.FindObjectOfType<RoadAssistPanel>();
                    panel.ClampNodeBox.CheckBox.isChecked = false;
                    panel.SegmentButtons.LeftButton.Disable();
                    panel.SegmentButtons.RightButton.Disable();
                }

                // Set rotation using camera.
                Quaternion rotation = Utils.GetCameraRotationAboutYAxis(Singleton<RenderManager>.instance.CurrentCameraInfo);

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    Vector3 pushDirection = new Vector3(1, 0, 0);
                    pushDirection = rotation * pushDirection;
                    GridRenderManager.GridCenter += pushDirection * moveSpeed;
                }

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    Vector3 pushDirection = new Vector3(-1, 0, 0);
                    pushDirection = rotation * pushDirection;
                    GridRenderManager.GridCenter += pushDirection * moveSpeed;
                }

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    Vector3 pushDirection = new Vector3(0, 0, 1);
                    pushDirection = rotation * pushDirection;
                    GridRenderManager.GridCenter += pushDirection * moveSpeed;
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    Vector3 pushDirection = new Vector3(0, 0, -1);
                    pushDirection = rotation * pushDirection;
                    GridRenderManager.GridCenter += pushDirection * moveSpeed;
                }
            }
            else
            {
                //Default move speed
                moveSpeed = 1f;
            }
            #endregion
        }
    }
}
