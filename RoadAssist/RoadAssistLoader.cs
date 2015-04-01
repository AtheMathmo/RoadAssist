using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace RoadAssist
{
    public class RoadAssistLoader : LoadingExtensionBase
    {
        GridRenderManager renderManager;
        GameObject roadAssistPanelObject;

        private static FastList<IRenderableManager> RenderManagers
        {
            get
            {
                // Thanks ccaat!
                return (FastList<IRenderableManager>)typeof(RenderManager).GetField("m_renderables", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {

            renderManager = new GridRenderManager();
            RenderManagers.Add(renderManager);

            roadAssistPanelObject = new GameObject("RoadAssistPanel", typeof(RoadAssistPanel));
            UIView.GetAView().AttachUIComponent(roadAssistPanelObject);
            
        }

        public override void OnLevelUnloading()
        {
            GridRenderManager.SetDefaultStatics();
            TerrainManager.instance.RenderZones = false;
            RenderManagers.Remove(renderManager);
            renderManager = null;

            GameObject.Destroy(roadAssistPanelObject);
            roadAssistPanelObject = null;

        }
    }
}
