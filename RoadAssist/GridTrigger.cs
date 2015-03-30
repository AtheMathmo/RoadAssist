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

        public override void OnCreated(IThreading threading)
        {

        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {

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
            }

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

        }


        private T GetPrivateVariable<T>(object obj, string fieldName)
        {
            return (T)obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }
    }
}
