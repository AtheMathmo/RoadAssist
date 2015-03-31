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
        private static Mesh boxMesh;

        private bool renderBuildings = true;

        public static Mesh BoxMesh
        {
            get { return boxMesh; }
            set { boxMesh = value; }
        }

        public override void OnCreated(IThreading threading)
        {
            boxMesh = CustomOverlayEffect.CreateBoxMesh();

            //fogDensity = GameObject.FindObjectOfType<RenderProperties>().m_volumeFogDensity;

            
        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (Event.current.alt && Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("[RoadAssist] Alt+H pressed.");
                renderBuildings = !renderBuildings;
                Utils.TogglePlanningMode(renderBuildings);
                
            }
            if (Event.current.alt && Input.GetKeyDown(KeyCode.Z))
            {
                // Turn on zones
                TerrainManager.instance.RenderZones = !TerrainManager.instance.RenderZones;
                Debug.Log("[RoadAssist] Alt+Z pressed.");
            }

            if (Event.current.alt && Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log("[RoadAssist] Alt+G pressed. Render is " + (!GridRenderManager.RenderGrid ? "on" : "off"));
                GridRenderManager.RenderGrid = !GridRenderManager.RenderGrid;

                if (!GridRenderManager.RenderGrid)
                {
                    //GridRenderManager.GridCenter.
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Left mouse clicked");
                
                MethodInfo rayCast = Utils.GetPrivateMethod(typeof(ToolBase), "RayCast");

                Vector3 mousePosition = Input.mousePosition;
                Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);
                float mouseRayLength = Camera.main.farClipPlane;

                ToolBase.RaycastInput input = new ToolBase.RaycastInput(mouseRay, mouseRayLength);
                input.m_ignoreTerrain = true;
                input.m_ignoreNodeFlags = NetNode.Flags.None;


                object[] parameters = new object[] {input, null};
                object result = rayCast.Invoke(null, parameters);


                if ((bool) result)
                {
                    ToolBase.RaycastOutput output = (ToolBase.RaycastOutput) parameters[1];
                    Debug.Log(output.m_netNode);
                    if ((int)output.m_netNode != 0)
                    {
                        NetNode node = NetManager.instance.m_nodes.m_buffer[(int)output.m_netNode];
                        GridRenderManager.GridCenter = node.m_position;
                    }

                }
            }
            #region "Rotate Grid"
            if (Event.current.alt && Input.GetKey(KeyCode.RightArrow))
            {
                angleRot = (angleRot - 1f) % 360 ;
                GridRenderManager.Rotation = Quaternion.AngleAxis(angleRot, new Vector3(0, 1, 0));
                
            }

            if (Event.current.alt && Input.GetKey(KeyCode.LeftArrow))
            {
                angleRot = (angleRot + 1f) % 360;
                GridRenderManager.Rotation = Quaternion.AngleAxis(angleRot, new Vector3(0, 1, 0));
            }
            #endregion

            #region "Resize Grid"
            if (Event.current.alt && Input.GetKey(KeyCode.UpArrow))
            {
                GridRenderManager.GridSize += 10f;
            }

            if (Event.current.alt && Input.GetKey(KeyCode.DownArrow))
            {
                GridRenderManager.GridSize -= 10f;
                if (GridRenderManager.GridSize <= 0)
                {
                    GridRenderManager.GridSize = 0;
                }
            }
            #endregion

            #region"Move Grid"     
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Quaternion rotation = Utils.GetCameraRotationAboutYAxis(Singleton<RenderManager>.instance.CurrentCameraInfo);
                Vector3 pushDirection = new Vector3(1, 0, 0);
                pushDirection = rotation*pushDirection;
                GridRenderManager.GridCenter += pushDirection * 20f;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Quaternion rotation = Utils.GetCameraRotationAboutYAxis(Singleton<RenderManager>.instance.CurrentCameraInfo);
                Vector3 pushDirection = new Vector3(-1, 0, 0);
                pushDirection = rotation * pushDirection;
                GridRenderManager.GridCenter += pushDirection * 20f;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                Quaternion rotation = Utils.GetCameraRotationAboutYAxis(Singleton<RenderManager>.instance.CurrentCameraInfo);
                Vector3 pushDirection = new Vector3(0, 0, 1);
                pushDirection = rotation * pushDirection;
                GridRenderManager.GridCenter += pushDirection * 20f;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                Quaternion rotation = Utils.GetCameraRotationAboutYAxis(Singleton<RenderManager>.instance.CurrentCameraInfo);
                Vector3 pushDirection = new Vector3(0, 0, -1);
                pushDirection = rotation * pushDirection;
                GridRenderManager.GridCenter += pushDirection * 20f;
            }
            #endregion
        }


        private T GetPrivateVariable<T>(object obj, string fieldName)
        {
            return (T)obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }
    }
}
