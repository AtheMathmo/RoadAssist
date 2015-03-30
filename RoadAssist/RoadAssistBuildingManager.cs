using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.Math;
using ColossalFramework.Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
namespace RoadAssist
{
    public class RoadAssistBuildingManager : BuildingManager
    {
        private static bool renderBuildings = true;

        public static bool RenderBuildings
        {
            get
            {
                return renderBuildings;
            }
            set
            {
                renderBuildings = value;
            }
        }

        protected override void EndRenderingImpl(RenderManager.CameraInfo cameraInfo)
        {
            if (renderBuildings)
            {
                FastList<RenderGroup> renderedGroups = Singleton<RenderManager>.instance.m_renderedGroups;
                for (int i = 0; i < renderedGroups.m_size; i++)
                {
                    RenderGroup renderGroup = renderedGroups.m_buffer[i];
                    int num = renderGroup.m_layersRendered & ~(1 << Singleton<NotificationManager>.instance.m_notificationLayer);
                    if (renderGroup.m_instanceMask != 0)
                    {
                        num &= ~renderGroup.m_instanceMask;
                        int num2 = renderGroup.m_x * 270 / 45;
                        int num3 = renderGroup.m_z * 270 / 45;
                        int num4 = (renderGroup.m_x + 1) * 270 / 45 - 1;
                        int num5 = (renderGroup.m_z + 1) * 270 / 45 - 1;
                        for (int j = num3; j <= num5; j++)
                        {
                            for (int k = num2; k <= num4; k++)
                            {
                                int num6 = j * 270 + k;
                                ushort num7 = this.m_buildingGrid[num6];
                                int num8 = 0;
                                while (num7 != 0)
                                {
                                    this.m_buildings.m_buffer[(int)num7].RenderInstance(cameraInfo, num7, renderGroup.m_instanceMask);
                                    num7 = this.m_buildings.m_buffer[(int)num7].m_nextGridBuilding;
                                    if (++num8 >= 32768)
                                    {
                                        CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (num != 0)
                    {
                        int num9 = renderGroup.m_z * 45 + renderGroup.m_x;
                        ushort num10 = this.m_buildingGrid2[num9];
                        int num11 = 0;
                        while (num10 != 0)
                        {
                            this.m_buildings.m_buffer[(int)num10].RenderInstance(cameraInfo, num10, num);
                            num10 = this.m_buildings.m_buffer[(int)num10].m_nextGridBuilding2;
                            if (++num11 >= 32768)
                            {
                                CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                break;
                            }
                        }
                    }
                }
                int num12 = PrefabCollection<BuildingInfo>.PrefabCount();
                for (int l = 0; l < num12; l++)
                {
                    BuildingInfo prefab = PrefabCollection<BuildingInfo>.GetPrefab((uint)l);
                    if (prefab != null)
                    {
                        prefab.UpdatePrefabInstances();
                        if (prefab.m_lodCount != 0)
                        {
                            Building.RenderLod(cameraInfo, prefab);
                        }
                        if (prefab.m_subMeshes != null)
                        {
                            for (int m = 0; m < prefab.m_subMeshes.Length; m++)
                            {
                                BuildingInfoBase subInfo = prefab.m_subMeshes[m].m_subInfo;
                                if (subInfo.m_lodCount != 0)
                                {
                                    Building.RenderLod(cameraInfo, subInfo);
                                }
                            }
                        }
                    }
                }
                if (this.m_common != null && this.m_common.m_subInfos != null)
                {
                    for (int n = 0; n < this.m_common.m_subInfos.Length; n++)
                    {
                        BuildingInfoBase buildingInfoBase = this.m_common.m_subInfos[n];
                        if (buildingInfoBase.m_lodCount != 0)
                        {
                            Building.RenderLod(cameraInfo, buildingInfoBase);
                        }
                    }
                }
            }
        }

    }

}

