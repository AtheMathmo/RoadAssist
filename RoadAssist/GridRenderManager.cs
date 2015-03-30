﻿using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.Math;
using ColossalFramework.Steamworks;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace RoadAssist
{
    public class GridRenderManager : IRenderableManager
    {
        private DrawCallData m_calldata;
        private static bool renderGrid = false;

        private static float gridGap = ZoneManager.ZONEGRID_CELL_SIZE/8;
        private static float gridSize = 1000f;

        private static Quaternion rotation = Quaternion.identity;

        public static bool RenderGrid
        {
            get
            {
                return renderGrid;
            }
            set
            {
                renderGrid = value;
            }
        }

        public static float GridDist
        {
            get
            {
                return gridGap;
            }
            set
            {
                gridGap = value;
            }
        }

        public static float GridSize
        {
            get
            {
                return gridSize;
            }
            set
            {
                gridSize = value;
            }
        }

        public static Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public void BeginOverlay(RenderManager.CameraInfo cameraInfo)
        {
            
        }

        public void BeginRendering(RenderManager.CameraInfo cameraInfo)
        {
        }

        public bool CalculateGroupData(int groupX, int groupZ, int layer, ref int vertexCount, ref int triangleCount, ref int objectCount, ref RenderGroup.VertexArrays vertexArrays)
        {
            return false;
        }

        public void EndOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (renderGrid)
            {
                TerrainPatch patch = TerrainManager.instance.m_patches[40];
                //RenderPatchGrid(cameraInfo, patch);
                RenderGridManual(cameraInfo, patch.m_bounds.center, gridSize);
            }
        }

        public void EndRendering(RenderManager.CameraInfo cameraInfo)
        {

        }

        public DrawCallData GetDrawCallData()
        {
            return m_calldata;
        }

        public string GetName()
        {
            return "GridRenderManager";
        }

        public void InitRenderData()
        {
            m_calldata.m_batchedCalls = 0;
            m_calldata.m_defaultCalls = 0;
            m_calldata.m_lodCalls = 0;
            m_calldata.m_overlayCalls = 0;
        }

        public void PopulateGroupData(int groupX, int groupZ, int layer, ref int vertexIndex, ref int triangleIndex, Vector3 groupPosition, RenderGroup.MeshData data, ref Vector3 min, ref Vector3 max, ref float maxRenderDistance, ref float maxInstanceDistance, ref bool requireSurfaceMaps)
        {
        }

        public void UndergroundOverlay(RenderManager.CameraInfo cameraInfo)
        {
        }

        private Material GetMaterial(string matName)
        {
            Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
            foreach (Material material in materials)
            {
                if (material.name.Equals(matName))
                {
                    return material;
                }
            }
            return null;
        }

        private T GetPrivateVariable<T>(object obj, string fieldName)
        {
            return (T)obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }

        /// <summary>
        /// Renders a grid over a given TerrainPatch - covers entire TerrainPatch.
        /// Currently used only for debugging
        /// </summary>
        /// <param name="cameraInfo"> The RenderManagers camera.</param>
        /// <param name="patch">The TerrainPatch to be rendered over</param>
        private void RenderPatchGrid(RenderManager.CameraInfo cameraInfo, TerrainPatch patch)
        {
            

            if (cameraInfo.Intersect(patch.m_bounds))
            {
                Bounds bounds = patch.m_bounds;

                Vector3 xVec = new Vector3(1f, 0f, 0f);
                Vector3 zVec = new Vector3(0f, 0f, 1f);

                xVec = rotation * xVec;
                zVec = rotation * zVec;

                // Currently only draws a square - but could easily change shape by changing size in each direction.
                int xLineCount = (int)Math.Floor(bounds.size.x / gridGap);
                int zLineCount = (int)Math.Floor(bounds.size.z / gridGap);

                for (int i = 0; i < xLineCount; i++)
                {
                    Quad3 quad = default(Quad3);
                    quad.a = bounds.center - xVec * bounds.extents.x + zVec * (i * gridGap - bounds.extents.z);
                    quad.b = bounds.center - xVec * bounds.extents.x + zVec * (i * gridGap - bounds.extents.z);
                    quad.c = bounds.center + xVec * bounds.extents.x + zVec * (i * gridGap - bounds.extents.z);
                    quad.d = bounds.center + xVec * bounds.extents.x + zVec * (i * gridGap - bounds.extents.z);
                    Color color = (i % 5 == 0 ? Color.red : Color.white);
                    RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, color, quad, -1f, 1025f, false, true);
                }

                for (int i = 0; i < zLineCount; i++)
                {
                    Quad3 quad = default(Quad3);
                    quad.a = bounds.center - zVec * bounds.extents.x + xVec * (i * gridGap - bounds.extents.z);
                    quad.b = bounds.center - zVec * bounds.extents.x + xVec * (i * gridGap - bounds.extents.z);
                    quad.c = bounds.center + zVec * bounds.extents.x + xVec * (i * gridGap - bounds.extents.z);
                    quad.d = bounds.center + zVec * bounds.extents.x + xVec * (i * gridGap - bounds.extents.z);

                    Color color = (i % 5 == 0 ? Color.red : Color.white);
                    RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, color, quad, -1f, 1025f, false, true);
                }


            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cameraInfo"></param>
        /// <param name="center"> The center of the grid to be drawn.</param>
        /// <param name="size"> Edge to edge length of grid to be drawn.</param>
        private void RenderGridManual(RenderManager.CameraInfo cameraInfo, Vector3 center, float size)
        {
            Vector3 sizeVec = new Vector3(size, size, size);
            Bounds gridBounds = new Bounds(center, sizeVec);

            if (cameraInfo.Intersect(gridBounds))
            {
                Vector3 xVec = new Vector3(1f, 0f, 0f);
                Vector3 zVec = new Vector3(0f, 0f, 1f);

                xVec = rotation * xVec;
                zVec = rotation * zVec;

                // Currently only draws a square - but could easily change shape by changing size in each direction.
                int xLineCount = (int)Math.Floor(size / gridGap);
                int zLineCount = (int)Math.Floor(size / gridGap);

                for (int i = 0; i < xLineCount; i++)
                {
                    Quad3 quad = default(Quad3);
                    quad.a = center - xVec * (size / 2) + zVec * (i * gridGap - (size / 2));
                    quad.b = center - xVec * (size / 2) + zVec * (i * gridGap - (size / 2));
                    quad.c = center + xVec * (size / 2) + zVec * (i * gridGap - (size / 2));
                    quad.d = center + xVec * (size / 2) + zVec * (i * gridGap - (size / 2));
                    Color color = (i % 5 == 0 ? Color.red : Color.white);
                    RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, color, quad, -1f, 1025f, false, true);
                }

                for (int i = 0; i < zLineCount; i++)
                {
                    Quad3 quad = default(Quad3);
                    quad.a = center - zVec * (size / 2) + xVec * (i * gridGap - (size / 2));
                    quad.b = center - zVec * (size / 2) + xVec * (i * gridGap - (size/2));
                    quad.c = center + zVec * (size / 2) + xVec * (i * gridGap - (size/2));
                    quad.d = center + zVec * (size / 2) + xVec * (i * gridGap - (size / 2));

                    Color color = (i % 5 == 0 ? Color.red : Color.white);
                    RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, color, quad, -1f, 1025f, false, true);
                }


            }
        }


    }
}